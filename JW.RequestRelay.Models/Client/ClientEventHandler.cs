using JW.RequestRelay.Models;
using JW.RequestRelay.Models.Client;
using JW.RequestRelay.Socket.Client;

namespace System
{
    /// <summary>
    /// 尝试与服务端建立连接，失败则Exception未异常西南西，成功Exception为NULL
    /// </summary>
    /// <param name="clientModel"></param>
    /// <param name="socketClient"></param>
    /// <param name="exception"></param>
    public delegate void ConnectCallbackEventHandler(ClientModel clientModel, SocketClient socketClient, Exception exception);

    /// <summary>
    /// 发送数据发生异常后执行
    /// </summary>
    /// <param name="clientModel"></param>
    /// <param name="socketClient"></param>
    /// <param name="exception"></param>
    public delegate void SendExceptionCallbackEventHandler(ClientModel clientModel, SocketClient socketClient, Exception exception, string message);

    /// <summary>
    /// 接受数据时发送异常后执行
    /// </summary>
    /// <param name="clientModel"></param>
    /// <param name="socketClient"></param>
    /// <param name="exception"></param>
    public delegate void ReciveExceptionCallbackEventHandler(ClientModel clientModel, SocketClient socketClient, Exception exception);

    /// <summary>
    /// 处理过程事件
    /// </summary>
    /// <param name="clientModel"></param>
    /// <param name="socketClient"></param>
    /// <param name="log"></param>
    public delegate void ProcessCallbackEventHandler(ClientModel clientModel, SocketClient socketClient, Log log);

    /// <summary>
    /// 客户端关闭
    /// </summary>
    /// <param name="clientModel"></param>
    /// <param name="socketClient"></param>
    /// <param name="exception"></param>
    public delegate void CloseCallbackEventHandler(ClientModel clientModel, SocketClient socketClient, Exception exception);

    /// <summary>
    /// 接受消息
    /// </summary>
    /// <param name="clientModel"></param>
    /// <param name="socketClient"></param>
    /// <param name="mseeage"></param>
    public delegate void ReceiveCallbackEventHandler(ClientModel clientModel, SocketClient socketClient, string mseeage);

}
