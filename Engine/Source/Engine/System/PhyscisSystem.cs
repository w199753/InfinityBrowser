using System.Threading;
using System.Runtime.CompilerServices;

namespace InfinityEngine.System
{
    internal class PhysicsSystem : Disposal
    {
        private bool IsLoopExit;
        internal Thread physicsThread;

        public PhysicsSystem()
        {
            this.IsLoopExit = false;
            this.physicsThread = new Thread(PhysicsFunc);
            this.physicsThread.Name = "PhyscisThread";
        }

        public void Start()
        {
            physicsThread.Start();
        }

        public void Exit()
        {
            IsLoopExit = true;
            physicsThread.Join();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PhysicsFunc()
        {
            while (!IsLoopExit)
            {

            }
        }

        protected override void Release()
        {

        }
    }
}
