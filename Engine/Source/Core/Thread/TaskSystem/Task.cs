using System.Threading.Tasks;

namespace InfinityEngine.Threading
{
    public interface ITask
    {
        public abstract void Execute();

        internal void Execute(Task dependsTask)
        {
            Execute();
        }

        internal void Execute(Task[] dependsTaskS)
        {
            Execute();
        }
    }

    public static class ITaskExtension
    {
        public static void Run<T>(this T taskData) where T : struct, ITask
        {
            taskData.Execute();
        }

        public static TaskRef Schedule<T>(this T taskData) where T : struct, ITask
        {
            return new TaskRef(Task.Factory.StartNew(taskData.Execute));
        }

        public static TaskRef Schedule<T>(this T taskData, in TaskRef depend) where T : struct, ITask
        {
            return new TaskRef(depend.task.ContinueWith(taskData.Execute));
        }

        public static TaskRef Schedule<T>(this T taskData, params TaskRef[] depends) where T : struct, ITask
        {
            Task[] dependsTask = new Task[depends.Length];
            for (int i = 0; i < depends.Length; ++i) { dependsTask[i] = depends[i].task; }
            return new TaskRef(Task.Factory.ContinueWhenAll(dependsTask, taskData.Execute));
        }
    }
}
