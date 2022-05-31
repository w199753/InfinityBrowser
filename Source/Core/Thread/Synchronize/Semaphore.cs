using System.Threading;

namespace Infinity.Threading
{
    public class Semaphore : Disposal
    {
        public string name;
        private bool m_Completed;
        public bool IsCompleted { get { return m_Completed; } }
        private AutoResetEvent m_SignalEvent;

        public Semaphore(in bool signald = false, string name = null)
        {
            this.name = name;
            this.m_Completed = false;
            this.m_SignalEvent = new AutoResetEvent(signald);
        }

        public void Signal()
        {
            m_SignalEvent.Set();
        }

        public void Wait()
        {
            m_SignalEvent.WaitOne();
        }

        protected override void Release()
        {
            m_SignalEvent.Dispose();
        }
    }
}
