using Infinity.Container;
using System.Runtime.CompilerServices;

namespace Infinity.Rendering
{
    public delegate void FGraphicsTask(RenderContext renderContext);

    public static class GraphicsUtility
    {
        public static TArray<FGraphicsTask> GraphicsTasks = new TArray<FGraphicsTask>(64);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddTask(FGraphicsTask graphicsTask, in bool bParallel = false)
        {
            GraphicsTasks.Add(graphicsTask);
        }
    }
}
