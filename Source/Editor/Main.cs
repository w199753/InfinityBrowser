using System;
using Infinity.Graphics;
using Infinity.Rendering;
using Infinity.Analytics;
using Infinity.Threading;
using System.Collections;

namespace Infinity.Editor
{
    [Serializable]
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

    [Serializable]
    public class TestEntity : Entity
    {
        //FTaskHandle m_AsynTaskRef;
        private TestComponent m_Component;

        public TestEntity() : base()
        {
            m_Component = new TestComponent();
            //AddComponent(m_Component);
        }

        public TestEntity(string name) : base(name)
        {
            m_Component = new TestComponent();
            //AddComponent(m_Component);
        }

        public TestEntity(string name, Entity parent) : base(name, parent)
        {
            m_Component = new TestComponent();
            //AddComponent(m_Component);
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

    public class TestApplication : Application
    {
        private TestEntity m_Entity;

        public TestApplication(int width, int height, string name) : base(width, height, name)
        {
            m_Entity = new TestEntity("TestEntity");
        }

        protected override void Play()
        {
            m_Entity.OnEnable();
        }

        protected override void Tick()
        {
            m_Entity.OnUpdate(Timer.DeltaTime);
        }

        protected override void End()
        {
            m_Entity.OnDisable();
        }
    }

    [Serializable]
    public class MainClass
    {
        static void Main(string[] args)
        {
            TestApplication App = new TestApplication(1280, 720, "Infinity Editor");
            App.Execute();

            /*Console.WriteLine("Fuck you");
            Console.ReadKey();*/
        }
    }
}
