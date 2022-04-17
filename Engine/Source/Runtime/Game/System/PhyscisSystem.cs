using System;
using System.Threading;
using InfinityEngine.Core.Object;
using System.Runtime.CompilerServices;

namespace InfinityEngine.Game.System
{
    internal class FPhysicsSystem : FDisposal
    {
        private bool IsLoopExit;
        internal Thread physicsThread;

        public FPhysicsSystem()
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
