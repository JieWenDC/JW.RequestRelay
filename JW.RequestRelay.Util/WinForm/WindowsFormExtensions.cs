using System.Threading.Tasks;

namespace System.Windows.Forms
{
    public static class WindowsFormException
    {
        /// <summary>
        /// 当前控件设置为不可用，并以不执行处理
        /// </summary>
        /// <param name="control"></param>
        /// <param name="action"></param>
        public static void ClickAsync(this Control control, Func<object> action, Action<object> success = null, Action<Exception> exception = null)
        {
            control.Enabled = false;
            Task.Run(() =>
            {
                try
                {
                    var ret = action();
                    if (success != null)
                    {
                        success(ret);
                    }
                }
                catch (Exception ex)
                {
                    if (exception == null)
                    {
                        MessageBox.Show(ex.ToString(), "处理异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        exception(ex);
                    }
                }
                finally
                {
                    control.Enabled = true;
                }
            });
        }

        /// <summary>
        /// 当前控件设置为不可用，并以不执行处理
        /// </summary>
        /// <param name="control"></param>
        /// <param name="action"></param>
        /// <param name="success"></param>
        /// <param name="exception"></param>
        public static void ClickAsync(this Control control, Action action, Action success = null, Action<Exception> exception = null)
        {
            control.Enabled = false;
            Task.Run(() =>
            {
                try
                {
                    action();
                    if (success != null)
                    {
                        success();
                    }
                }
                catch (Exception ex)
                {
                    if (exception == null)
                    {
                        MessageBox.Show(ex.ToString(), "处理异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        exception(ex);
                    }
                }
                finally
                {
                    control.Invoke(() => {
                        control.Enabled = true;
                    });
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="action"></param>
        public static void Invoke(this Control control,Action action)
        {
            control.Invoke(new EventHandler(delegate
            {
                action();
            }));
        }
    }
}
