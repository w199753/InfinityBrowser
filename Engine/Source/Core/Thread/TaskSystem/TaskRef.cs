using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace InfinityEngine.Threading
{
    public struct TaskRef
    {
        internal Task task;

        public TaskRef(Task task)
        {
            this.task = task;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Complete()
        {
            return task == null ? true : task.IsCompleted;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Wait()
        {
            task.Wait();
        }
    }
}
