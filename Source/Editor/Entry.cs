using System;
using Infinity.Memory;
using Infinity.Engine;
using Infinity.Shaderlib;
using Infinity.Threading;
using System.Collections;

namespace Infinity.Editor
{
    public class TestComponent : Component
    {
        /*int numData = 100000;
        int[] readData;
        float cpuTime
        {
            get { return (float)timeProfiler.microseconds / 1000.0f; }
        }
        float gpuTime => gpuReadback.gpuTime;

        RHIBuffer buffer
        {
            get { return bufferRef.buffer; }
        }
        RHIBufferRef bufferRef;
        TimeProfiler timeProfiler;
        RHIMemoryReadback gpuReadback;*/

        public override void OnEnable()
        {
            Console.WriteLine("Enable Component");

            /*readData = new int[numData];
            timeProfiler = new TimeProfiler();

            GraphicsUtility.AddTask((RenderContext renderContext) =>
            {
                BufferDescriptor descriptor = new BufferDescriptor(numData, 4, EUsageType.UnorderAccess, EStorageType.Default | EStorageType.Dynamic | EStorageType.Staging);
                descriptor.name = "TestBuffer";

                bufferRef = renderContext.GetBuffer(descriptor);
                gpuReadback = renderContext.CreateMemoryReadback("TestReadback", true);

                RHICommandBuffer cmdBuffer = renderContext.GetCommandBuffer("Upload", EContextType.Copy);
                cmdBuffer.Clear();

                int[] data = new int[numData];
                for (int i = 0; i < numData; ++i)
                {
                    data[i] = numData - i;
                }

                cmdBuffer.BeginEvent("Upload");
                buffer.SetData(cmdBuffer, data);
                cmdBuffer.EndEvent();
                renderContext.ExecuteCommandBuffer(cmdBuffer);
                renderContext.ReleaseCommandBuffer(cmdBuffer);
            });*/
        }

        public override void OnUpdate(in float deltaTime)
        {
            /*GraphicsUtility.AddTask((RenderContext renderContext) =>
            {
                gpuReadback.EnqueueCopy(renderContext, buffer);

                timeProfiler.Start();
                gpuReadback.GetData(renderContext, buffer, readData);
                timeProfiler.Stop();

                Console.WriteLine("||");
                Console.WriteLine("CPUCopy : " + cpuTime + "ms");
                Console.WriteLine("GPUCopy : " + gpuTime + "ms");
            });*/
        }

        public override void OnDisable()
        {
            /*GraphicsUtility.AddTask((RenderContext renderContext) =>
            {
                gpuReadback.Dispose();
                renderContext.ReleaseBuffer(bufferRef);
                Console.WriteLine("Release RenderProxy");
            });*/

            Console.WriteLine("Disable Component");
        }
    }

    public class TestEntity : Entity
    {
        //FTaskHandle m_AsynTaskRef;
        private TestComponent m_Component;

        public TestEntity() : base()
        {
            m_Component = new TestComponent();
        }

        public TestEntity(string name) : base(name)
        {
            m_Component = new TestComponent();
        }

        public TestEntity(string name, Entity parent) : base(name, parent)
        {
            m_Component = new TestComponent();
        }

        IEnumerator TestWaitSeconds()
        {
            Console.WriteLine("Start WaitSeconds");
            yield return new WaitForSeconds(5);
            Console.WriteLine("After 5 Seconds");
            yield return new WaitForSeconds(5);
            Console.WriteLine("After 10 Seconds");
        }

        public override void OnEnable()
        {
            base.OnEnable();
            AddComponent(m_Component);
            StartCoroutine(TestWaitSeconds());
            Console.WriteLine("Enable Actor");
        }

        public override void OnUpdate(in float deltaTime)
        {
            base.OnUpdate(deltaTime);
            //Console.WriteLine("Update Actor");
            //Console.WriteLine(deltaTime);

            //Async Task
            /*Thread.Sleep(100);
            bool isReady = m_AsynTaskRef.Complete();
            if (isReady)
            {
                FAsyncTask asynTask;
                asynTask.Run(ref m_AsynTaskRef);
            }
            Console.WriteLine("Can you hear me?");*/
        }

        public override void OnDisable()
        {
            base.OnDisable();
            RemoveComponent(m_Component);
            Console.WriteLine("Disable Actor");
        }
    }

    public class EditorEntry
    {
        static void Main(string[] args)
        {
            Shaderlab shaderLab = ShaderlabUtility.ParseShaderlabFromFile("C:\\CGFile\\Engines\\Infinity\\Shader\\InfinityLit.shader");

            HeapBlock heapBlock = new HeapBlock(64);
            int outIndex;
            bool test = heapBlock.PullFreeSpaceIndex(5, out outIndex);
            test = heapBlock.PullFreeSpaceIndex(3, out outIndex);
            test = heapBlock.PullFreeSpaceIndex(6, out outIndex);
            heapBlock.PushFreeSpaceIndex(outIndex, 6);
            test = heapBlock.PullFreeSpaceIndex(2, out outIndex);
            test = heapBlock.PullFreeSpaceIndex(16, out outIndex);

            Application editorApp = new Application(1600, 900, "Infinity Editor");

            GameScene persistentScene = new GameScene();
            editorApp.GameWorld.SetPersistentScene(persistentScene);

            TestEntity testEntity = new TestEntity("TestEntity");
            editorApp.GameWorld.PersistentScene.AddEntity(testEntity);

            editorApp.Run();

            /*Console.WriteLine("Fuck you");
            Console.ReadKey();*/
        }
    }
}
