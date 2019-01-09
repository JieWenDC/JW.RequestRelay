using System.Timers;

namespace JW.RequestRelay.Util
{
    /// <summary>
    /// 全局定时器帮助类
    /// </summary>
    public class GlobalTimerHelper
    {
        private static string LOCK = string.Empty;
        public System.Timers.Timer TIMER { get; private set; }

        private static GlobalTimerHelper _instance;
        public static GlobalTimerHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (LOCK)
                    {
                        if (_instance == null)
                        {
                            _instance = new GlobalTimerHelper();
                        }
                    }
                }
                return _instance;
            }
        }

        public GlobalTimerHelper()
        {
            TIMER = new System.Timers.Timer();
            TIMER.Interval = 1000 * 60;
            TIMER.AutoReset = true;
            TIMER.Enabled = true;
            TIMER.Start();
        }

        public void AddEvent(ElapsedEventHandler action)
        {
            TIMER.Elapsed += action;
        }

    }
}
