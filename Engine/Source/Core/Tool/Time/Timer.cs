using System.Diagnostics;

namespace InfinityEngine
{
    public class Timer
    {
        public static double SecondsPerTick { get { return 1.0 / Stopwatch.Frequency; } }
        public static double MilliSecsPerTick { get { return 1000.0f / Stopwatch.Frequency; } }
        public static double MicroSecsPerTick { get { return 1000000.0f / Stopwatch.Frequency; } }

        static float m_DeltaTime;
        public static float DeltaTime { get { return m_DeltaTime; } }

        static float m_ElapsedTime = 0;
        public static float ElapsedTime { get { return m_ElapsedTime; } }

        static int m_FrameIndex = 0;
        public static int FrameIndex { get { return m_FrameIndex; } }

        public static void Tick(in float deltaTime)
        {
            ++m_FrameIndex;
            m_DeltaTime = deltaTime;
            m_ElapsedTime += m_ElapsedTime;
        }
    }
}
