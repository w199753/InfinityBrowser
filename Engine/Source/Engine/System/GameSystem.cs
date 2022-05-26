﻿using System;
using System.Threading;
using Infinity.Analytics;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Semaphore = Infinity.Threading.Semaphore;

namespace Infinity.System
{
    internal delegate void FGamePlayFunc();
    internal delegate void FGameTickFunc();
    internal delegate void FGameEndFunc();

    internal class GameSystem : Disposal
    {
        private float m_DeltaTime;
        private Semaphore m_SemaphoreG2R;
        private Semaphore m_SemaphoreR2G;
        private FGameEndFunc m_GameEndFunc;
        private FGamePlayFunc m_GamePlayFunc;
        private FGameTickFunc m_GameTickFunc;
        private TimeProfiler m_TimeCounter;
        private List<float> m_LastDeltaTimes;

        public GameSystem(FGameEndFunc gameEndFunc, FGamePlayFunc gamePlayFunc, FGameTickFunc gameTickFunc, Semaphore semaphoreG2R, Semaphore semaphoreR2G)
        {
            m_GameEndFunc = gameEndFunc;
            m_GamePlayFunc = gamePlayFunc;
            m_GameTickFunc = gameTickFunc;
            m_SemaphoreG2R = semaphoreG2R;
            m_SemaphoreR2G = semaphoreR2G;
            m_TimeCounter = new TimeProfiler();
            m_LastDeltaTimes = new List<float>(64);
            Thread.CurrentThread.Name = "GameThread";
        }

        public void Start()
        {
            m_GamePlayFunc();
            m_TimeCounter.Reset();
            m_TimeCounter.Begin();
        }

        public void Exit()
        {
            m_GameEndFunc();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update()
        {
            m_TimeCounter.Start();
            m_SemaphoreR2G.Wait();
            Timer.Tick(m_DeltaTime);
            m_GameTickFunc();
            m_SemaphoreG2R.Signal();
            WaitForTargetFPS();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void WaitForTargetFPS()
        {
            long elapsed = 0;
            int deltaTimeSmoothing = 2;

            if (GameApplication.GTargetFrameRate > 0)
            {
                long targetMax = 1000000L / GameApplication.GTargetFrameRate;

                while(true)
                {
                    elapsed = m_TimeCounter.microseconds;
                    if (elapsed >= targetMax)
                        break;

                    // Sleep if 1 ms or more off the frame limiting goal
                    if (targetMax - elapsed >= 1000L)
                    {
                        int sleepTime = (int)((targetMax - elapsed) / 1000L);
                        Thread.Sleep(sleepTime);
                    }
                }
            }

            elapsed = m_TimeCounter.microseconds;
            m_TimeCounter.Start();

            // Perform timestep smoothing
            m_DeltaTime = 0.0f;
            m_LastDeltaTimes.Add(elapsed / 1000000.0f);

            if (m_LastDeltaTimes.Count > deltaTimeSmoothing)
            {
                // If the smoothing configuration was changed, ensure correct amount of samples
                m_LastDeltaTimes.RemoveRange(0, m_LastDeltaTimes.Count - deltaTimeSmoothing);
                for (int i = 0; i < m_LastDeltaTimes.Count; ++i)
                {
                    m_DeltaTime += m_LastDeltaTimes[i];
                }
                m_DeltaTime /= m_LastDeltaTimes.Count;
            } else {
                m_DeltaTime = m_LastDeltaTimes[m_LastDeltaTimes.Count - 1];
            }
        }
        
        protected override void Release()
        {
            m_TimeCounter.Stop();
        }
    }
}
