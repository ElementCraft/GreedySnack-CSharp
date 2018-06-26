using System;

namespace GreedySnack.Utils
{
    /// <summary>
    /// 时钟计时类
    /// </summary>
    public class Clock
    {
        private static DateTime nowTick;
        private static DateTime preTick;
        private static TimeSpan passTick;
        private static double totalTick;

        /// <summary>
        /// 初始化时钟
        /// </summary>
        public static void Init()
        {
            preTick = DateTime.Now;
            totalTick = 0;
        }

        /// <summary>
        /// 计时打点
        /// </summary>
        /// <returns>距上一次打点过去的时间 毫秒</returns>
        public static float Tick()
        {
            nowTick = DateTime.Now;
            passTick = nowTick.Subtract(preTick);
            preTick = nowTick;

            double passMS = passTick.TotalMilliseconds;
            totalTick += passMS;

            return (float)passMS;
        }

        /// <summary>
        /// 从初始化开始总共经过的时间，毫秒
        /// </summary>
        /// <returns></returns>
        public static double GetTotalTick()
        {
            return totalTick;
        }
    }
}
