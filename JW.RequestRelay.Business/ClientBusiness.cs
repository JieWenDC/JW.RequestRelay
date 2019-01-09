using JW.RequestRelay.Models;
using JW.RequestRelay.Models.Client;
using JW.RequestRelay.Models.Http;
using JW.RequestRelay.Socket;
using JW.RequestRelay.Socket.Client;
using JW.RequestRelay.Util;
using JW.RequestRelay.Util.Json;
using JW.RequestRelay.Util.Logging;
using JW.RequestRelay.Util.Threading;
using JW.RequestRelay.Util.Xml;
using Polly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JW.RequestRelay.Business
{
    public class ClientBusiness
    {
        public ClientBusiness()
        {

        }

        private  System.Collections.Concurrent.ConcurrentDictionary<string, ClientModel> _CLIENTS;
        protected  System.Collections.Concurrent.ConcurrentDictionary<string, ClientModel> CLIENTS
        {
            get
            {
                if (_CLIENTS == null)
                {
                    _CLIENTS = new System.Collections.Concurrent.ConcurrentDictionary<string, ClientModel>();
                    var xml = XmlHelper.LoadXmlDoc(ServerDataPath);
                    var list = XmlHelper.ToObject<List<ClientModel>>(xml);
                    if (list.ExistsData())
                    {
                        list.ForEach(item =>
                        {
                            _CLIENTS.TryAdd(item.Id, item);
                        });
                    }
                }
                return _CLIENTS;
            }
        }

        private string SyncLock = string.Empty;

        /// <summary>
        /// 处理完毕
        /// </summary>
        public event ProcessCallbackEventHandler ProcessedCallback;

        /// <summary>
        /// 处理过程
        /// </summary>
        public event ProcessCallbackEventHandler ProcessCallback;

        private string _ServerDataPath;
        protected string ServerDataPath
        {
            get
            {
                if (string.IsNullOrEmpty(_ServerDataPath))
                {
                    _ServerDataPath = FileHelper.GetMapPath("config/Client.xml");
                }
                return _ServerDataPath;
            }
        }

        /// <summary>
        /// 新增服务端
        /// </summary>
        /// <param name="model"></param>
        public void Add(ClientModel model)
        {
            CLIENTS.TryAdd(model.Id, model);
            Save();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        public void Delete(string id)
        {
            ClientModel server;
            CLIENTS.TryRemove(id, out server);
            Save();
        }

        /// <summary>
        /// 改
        /// </summary>
        /// <param name="model"></param>
        public void Update(ClientModel model)
        {
            var old_model = CLIENTS[model.Id];
            old_model.CheckNull("指定数据不存在");
            CLIENTS.TryUpdate(model.Id, model, old_model).CheckIsFalse("更新失败");
            Save();
        }

        /// <summary>
        /// 获取服务端列表
        /// </summary>
        /// <returns></returns>
        public List<ClientModel> GetList()
        {
            return CLIENTS.Values.ToList();
        }

        /// <summary>
        /// 获取制定客户端
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ClientModel Get(string id)
        {
            return CLIENTS[id];
        }

        /// <summary>
        /// 保存变更
        /// </summary>
        public void Save()
        {
            var xml = XmlHelper.ToXml(CLIENTS.Values.ToList());
            File.WriteAllText(ServerDataPath, xml, Encoding.UTF8);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Start(string id)
        {
            var client = CLIENTS[id];
            client.CheckNull("指定数据不存在");
            lock (SyncLock)
            {
                if (client.Status == ClientModelStatusEnum.Pause)
                {
                    client.ReceiveCallback -= ReceiveCallback;
                    client.ReceiveCallback += ReceiveCallback;
                    while (client.Sessions.Count < client.MaxSession)
                    {
                        client.CreateSession();
                    }
                    client.Status = ClientModelStatusEnum.Running;
                }
            }
        }

        /// <summary>
        /// 启动全部
        /// </summary>
        public void StartAll()
        {
            foreach (var client in GetList())
            {
                ThreadPool.QueueUserWorkItem((id) =>
                {
                    Policy.Handle<Exception>().WaitAndRetryForever(retryAttempt => TimeSpan.FromSeconds(Math.Pow(1, retryAttempt)), (policy_ex, timer) =>
                    {
                        Log4netHelper.Fatal($"连接服务端失败 timer={timer.TotalSeconds} ex={policy_ex.Message}", policy_ex);
                    }).Execute(() =>
                    {
                        Start(id.ToString());
                    });
                }, client.Id);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void Close(string id)
        {
            var clientModel = CLIENTS[id];
            clientModel.CheckNull("指定数据不存在");
            clientModel.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void CloseAll()
        {
            foreach (var clientModel in GetList().Where(row => row.Status == ClientModelStatusEnum.Running))
            {
                Close(clientModel.Id);
            }
        }

        /// <summary>
        /// 客户端接受到消息
        /// </summary>
        /// <param name="clientModel"></param>
        /// <param name="message"></param>
        private void ReceiveCallback(ClientModel clientModel, SocketClient socketClient, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            var log = new Log()
            {
                RequestDateTime = DateTime.Now,
                SourceId = clientModel.Id,
                SourceType = SourceTypeEnum.Client,
                LocalAddress = socketClient.LocalAddress,
                ReceiveMessage = message,
                Id = Guid.NewGuid().ToString(),
            };
            log.Stage = "接受到消息";
            AsyncHelper.TaskRun(() =>
            {
                ProcessCallback(clientModel, socketClient, log);
            });
            RelayAsync(clientModel, socketClient, log, message);
        }

        /// <summary>
        /// 异步转发
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        private void RelayAsync(ClientModel client, SocketClient socketClient, Log log, string message)
        {
            Task.Run(async () =>
            {
                await Relay(client, socketClient, log, message);
            });
        }

        /// <summary>
        /// 转发
        /// </summary>
        /// <returns></returns>
        private async Task Relay(ClientModel clientModel, SocketClient socketClient, Log log, string message)
        {
            RequestModel request = null;
            ResponseModel response = null;

            #region 解析消息
            try
            {
                request = JsonHelper.DeserializeObject<RequestModel>(message);
            }
            catch (Exception ex)
            {
                log.Stage = "解析请求参数异常";
                AsyncHelper.TaskRun(() =>
                {
                    ProcessCallback(clientModel, socketClient, log);
                });
                ex = Log4netHelper.GetRealException(ex);
                response = new ResponseModel() { Response = Encoding.UTF8.GetBytes(ex.Message), };
                log.Content += $"{Environment.NewLine}解析消息异常：{ex}";
                Log4netHelper.Fatal($"解析消息异常：{message}", ex);
            }
            log.Request = request;
            #endregion

            #region 转发
            if (request != null)
            {
                try
                {
                    log.StartHttpRelayTime = DateTime.Now;
                    var url = clientModel.HandleUrl;
                    url = string.Format("{0}{1}", clientModel.HandleUrl, request.UrlPathAndQuery);
                    log.Stage = "开始转发请求";
                    AsyncHelper.TaskRun(() =>
                    {
                        ProcessCallback(clientModel, socketClient, log);
                    });
                    response = await HttpRelayHelper.HttpRelayAsync(url, request);
                    log.Stage = "收到请求响应";
                    AsyncHelper.TaskRun(() =>
                    {
                        ProcessCallback(clientModel, socketClient, log);
                    });
                    log.EndHttpRelayTime = DateTime.Now;
                    log.Relay = true;
                }
                catch (Exception ex)
                {
                    log.Stage = "转发请求异常";
                    AsyncHelper.TaskRun(() =>
                    {
                        ProcessCallback(clientModel, socketClient, log);
                    });
                    response = new ResponseModel()
                    {
                        Response = Encoding.UTF8.GetBytes(ex.Message),
                        Id = request.Id,
                    };
                    log.Relay = false;
                    log.Content += $"{Environment.NewLine}转发消息异常：{ex}";
                }
            }
            #endregion

            log.Response = response;

            SocketClient reply_socket = null;
            Policy.Handle<Exception>().WaitAndRetryForever(retryAttempt => TimeSpan.FromSeconds(Math.Pow(1, retryAttempt)), (policy_ex, timer) =>
            {
                log.Stage = "回复异常";
                AsyncHelper.TaskRun(() =>
                {
                    ProcessCallback(clientModel, socketClient, log);
                });
            }).Execute(() =>
            {
                #region 确保当前客户端状态可用

                if (socketClient.IsDisposable || !socketClient.Socket.Connected)
                {
                    log.Content += $"{Environment.NewLine}客户端已被释放，开始创建新的客户端";
                    reply_socket = clientModel.GetOneFreeOnlineClient();
                    if (reply_socket != null)
                    {
                        if (reply_socket.IsDisposable || !reply_socket.Socket.Connected)
                        {
                            log.Content += $"{Environment.NewLine}客户端已被释放，开始创建新的客户端";
                            Policy.Handle<Exception>().WaitAndRetryForever(retryAttempt => TimeSpan.FromSeconds(Math.Pow(1, retryAttempt)), (policy_ex, timer) =>
                            {
                                log.Stage = "无可用客户端，创建客户端异常";
                                AsyncHelper.TaskRun(() =>
                                {
                                    ProcessCallback(clientModel, socketClient, log);
                                });
                                log.Content += $"{Environment.NewLine}创建新的客户端异常：{policy_ex}";
                            }).Execute(() =>
                            {
                                reply_socket = clientModel.CreateSession();
                            });
                        }
                    }
                    else
                    {
                        reply_socket = clientModel.CreateSession();
                    }
                }
                else
                {
                    reply_socket = socketClient;
                }

                #endregion

                #region 回复

                log.Stage = "开始回复";
                AsyncHelper.TaskRun(() =>
                {
                    ProcessCallback(clientModel, socketClient, log);
                });
                reply_socket.Send(response);
                #endregion
            });

            log.Stage = "回复完毕";
            AsyncHelper.TaskRun(() =>
            {
                ProcessCallback(clientModel, socketClient, log);
            });
            log.Reply = true;
            log.ResponseDateTime = DateTime.Now;
            if (ProcessedCallback != null)
            {
                ProcessedCallback(clientModel, reply_socket, log);
            }
        }
    }
}
