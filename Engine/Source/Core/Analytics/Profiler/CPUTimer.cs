using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Infinity.Analytics
{
    public class TimeProfiler
    {
        private Stopwatch m_Stopwatch;

        public TimeProfiler()
        {
            m_Stopwatch = new Stopwatch();
        }

        public long microseconds { get { return (long)(m_Stopwatch.ElapsedTicks * Timer.MicroSecsPerTick); } }
        public long milliseconds { get { return m_Stopwatch.ElapsedMilliseconds; } }
        public float seconds { get { return m_Stopwatch.ElapsedMilliseconds / 1000.0f; } }
        public void Reset() => m_Stopwatch.Reset();
        public void Begin() => m_Stopwatch.Start();
        public void Start() => m_Stopwatch.Restart();
        public void Stop() => m_Stopwatch.Stop();

    }

    public unsafe class CPUTimer : IDisposable
    {
        [DllImport("CPUTimer")]
        public static extern IntPtr CreateCPUTimer();

        [DllImport("CPUTimer")]
        public static extern void BeginCPUTimer(IntPtr cpuTimer);

        [DllImport("CPUTimer")]
        public static extern void EndCPUTimer(IntPtr cpuTimer);

        [DllImport("CPUTimer")]
        public static extern long GetCPUTimer(IntPtr cpuTimer);

        [DllImport("CPUTimer")]
        public static extern void ReleaseCPUTimer(IntPtr cpuTimer);

        [DllImport("CPUTimer")]
        public static extern void DoTask(int* IntArray, int BaseCount, int SecondCount);

        
        private IntPtr cpuTimer;

        public CPUTimer()
        {
            cpuTimer = CreateCPUTimer();
        }

        public void Begin()
        {
            BeginCPUTimer(cpuTimer);
        }

        public void End()
        {
            EndCPUTimer(cpuTimer);
        }

        public long GetMillisecond()
        {
            return GetCPUTimer(cpuTimer);
        }

        public void Dispose()
        {
            ReleaseCPUTimer(cpuTimer);
        }
    }
}
