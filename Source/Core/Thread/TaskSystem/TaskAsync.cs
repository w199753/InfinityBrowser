using System.Threading.Tasks;

namespace Infinity.Threading
{
    public interface ITaskAsync
    {
        public abstract void Execute();
    }

    public static class ITaskAsyncExtension
    {
        public static TaskRef Run<T>(this T taskData) where T : struct, ITaskAsync
        {
            return new TaskRef(Task.Factory.StartNew(taskData.Execute));
        }

        public static void Run<T>(this T taskData, ref TaskRef taskRef) where T : struct, ITaskAsync
        {
            taskRef = new TaskRef(Task.Factory.StartNew(taskData.Execute));
        }
    }
}
