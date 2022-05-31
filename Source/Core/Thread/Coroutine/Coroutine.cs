using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Infinity.Threading
{
    public interface IYieldInstruction
    {
        bool Update(in float deltaTime);
    }

    public class WaitForFrames : IYieldInstruction
    {
        float m_Frames;

        public WaitForFrames(in float frames)
        {
            m_Frames = frames;
        }

        public bool Update(in float deltaTime)
        {
            m_Frames -= 1;
            return m_Frames <= 0;
        }
    }

    public class WaitForSeconds : IYieldInstruction
    {
        float m_Seconds;

        public WaitForSeconds(in float seconds)
        {
            m_Seconds = seconds;
        }

        public bool Update(in float deltaTime)
        {
            m_Seconds -= deltaTime;
            return m_Seconds <= 0;
        }
    }

    public class CoroutineProcessor
    {
        public int Count
        {
            get { return m_Coroutines.Count; }
        }

        private LinkedList<IEnumerator> m_Coroutines;

        public CoroutineProcessor()
        {
            m_Coroutines = new LinkedList<IEnumerator>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CoroutineRef Start(IEnumerator routine)
        {
            m_Coroutines.AddLast(routine);
            return new CoroutineRef(this, routine);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Stop(IEnumerator routine)
        {
            try
            {
                m_Coroutines.Remove(routine);
            }
            catch (Exception exception) 
            { 
                Console.WriteLine(exception.ToString()); 
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void StopAll()
        {
            m_Coroutines.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsRunning(IEnumerator routine)
        {
            return m_Coroutines.Contains(routine);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnUpdate(in float deltaTime)
        {
            var routine = m_Coroutines.First;
            while (routine != null)
            {
                IEnumerator ie = routine.Value;
                bool ret = true;
                if (ie.Current is IYieldInstruction)
                {
                    IYieldInstruction wait = (IYieldInstruction)ie.Current;

                    if (wait.Update(deltaTime))
                    {
                        ret = ie.MoveNext();
                    }
                }
                else
                {
                    ret = ie.MoveNext();
                }

                if (!ret)
                {
                    m_Coroutines.Remove(routine);
                }

                routine = routine.Next;
            }
        }
    }

    public struct CoroutineRef
    {
        public bool IsRunning
        {
            get 
            { 
                return enumerator != null && dispatcher.IsRunning(enumerator); 
            }
        }
        public IEnumerator enumerator
        {
            get 
            { 
                return m_Enumerator; 
            }
        }
        public CoroutineProcessor dispatcher
        {
            get 
            { 
                return m_Dispatcher; 
            }
        }

        private IEnumerator m_Enumerator;
        private CoroutineProcessor m_Dispatcher;

        public CoroutineRef(CoroutineProcessor dispatcher, IEnumerator enumerator)
        {
            m_Dispatcher = dispatcher;
            m_Enumerator = enumerator;
        }

        public IEnumerator Wait()
        {
            if (m_Enumerator != null)
                while (m_Dispatcher.IsRunning(m_Enumerator))
                    yield return null;
        }
    }

}
