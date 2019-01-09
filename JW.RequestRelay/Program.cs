using JW.RequestRelay.Util;
using JW.RequestRelay.Util.Logging;
using System;
using System.Windows.Forms;

namespace JW.RequestRelay
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Log4netHelper.Application_Start(FileHelper.GetMapPath("config/log4net.config"));
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MD());
        }
    }
}
