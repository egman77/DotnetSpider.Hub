using NLog;

namespace DotnetSpider.Enterprise.Agent
{
    public class HandlerFactory
    {
        private static readonly Logger Logger;
        private static int _runningTaskCount = 0;
        private static readonly object RunningCountLock = new object();

        static HandlerFactory()
        {
            Logger = LogManager.GetCurrentClassLogger();
        }

        public static void AddRunningCount()
        {
            lock (RunningCountLock)
            {
                ++_runningTaskCount;
            }
        }

        public static void ReduceRunningCount()
        {
            lock (RunningCountLock)
            {
                --_runningTaskCount;
            }
        }

        public static int GetRunningCount()
        {
            lock (RunningCountLock)
            {
                return _runningTaskCount;
            }
        }
    }
}