using System.Threading;
using System.Runtime.CompilerServices;

namespace Infinity.Engine
{
    internal class PhysicsModule : Disposal
    {
        private bool m_LoopExit;
        private Thread m_PhysicsThread;

        public PhysicsModule()
        {
            m_LoopExit = false;
            m_PhysicsThread = new Thread(PhysicsFunc);
            m_PhysicsThread.Name = "PhyscisThread";
        }

        public void Start()
        {
            m_PhysicsThread.Start();
        }

        public void Exit()
        {
            m_LoopExit = true;
            m_PhysicsThread.Join();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PhysicsFunc()
        {
            while (!m_LoopExit)
            {

            }
        }

        protected override void Release()
        {

        }
    }
}
