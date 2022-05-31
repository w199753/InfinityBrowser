using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Infinity.Threading
{
    public class CoroutineDispatcher
    {
        public int Count
        {
            get { return m_Dunning.Count; }
        }

        private List<float> m_Delays;
        private List<IEnumerator> m_Dunning;

        public CoroutineDispatcher()
        {
            m_Delays = new List<float>(8);
            m_Dunning = new List<IEnumerator>(8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CoroutineRef Start(IEnumerator routine, in float delay = 0)
        {
            m_Delays.Add(delay);
            m_Dunning.Add(routine);
            return new CoroutineRef(this, routine);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Stop(IEnumerator routine)
        {
            int i = m_Dunning.IndexOf(routine);
            if (i < 0)
            {
                return false;
            }

            m_Delays[i] = 0f;
            m_Dunning[i] = null;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void StopAll()
        {
            m_Delays.Clear();
            m_Dunning.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsRunning(IEnumerator routine)
        {
            return m_Dunning.Contains(routine);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool OnUpdate(in float deltaTime)
        {
            if (m_Dunning.Count > 0)
            {
                for (int i = 0; i < m_Dunning.Count; i++)
                {
                    if (m_Delays[i] > 0f)
                    {
                        m_Delays[i] -= deltaTime;
                    }
                    else if (m_Dunning[i] == null || !MoveNext(m_Dunning[i], i))
                    {
                        m_Dunning.RemoveAt(i);
                        m_Delays.RemoveAt(--i);
                    }
                }
                return true;
            }
            return false;
        }

        bool MoveNext(IEnumerator routine, in int index)
        {
            if (routine.Current is IEnumerator)
            {
                if (MoveNext((IEnumerator)routine.Current, index))
                    return true;

                m_Delays[index] = 0f;
            }

            bool result = routine.MoveNext();

            if (routine.Current is float)
                m_Delays[index] = (float)routine.Current;

            return result;
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
        public CoroutineDispatcher dispatcher
        {
            get 
            { 
                return m_Dispatcher; 
            }
        }

        private IEnumerator m_Enumerator;
        private CoroutineDispatcher m_Dispatcher;

        public CoroutineRef(CoroutineDispatcher dispatcher, IEnumerator enumerator)
        {
            m_Dispatcher = dispatcher;
            m_Enumerator = enumerator;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool Stop()
        {
            return IsRunning && m_Dispatcher.Stop(m_Enumerator);
        }

        public IEnumerator Wait()
        {
            if (m_Enumerator != null)
                while (m_Dispatcher.IsRunning(m_Enumerator))
                    yield return null;
        }
    }

}
