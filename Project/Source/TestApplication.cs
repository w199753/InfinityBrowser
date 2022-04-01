using System;
using InfinityEngine.Game.System;
using InfinityEngine.Graphics.RHI;
using InfinityEngine.Core.Profiler;
using InfinityEngine.Game.Application;
using InfinityEngine.Game.ActorFramework;
using InfinityEngine.Rendering.RenderLoop;

namespace ExampleProject
{
    [Serializable]
    public class TestComponent : UComponent
    {
        int numData = 100000;
        int[] readData;
        float cpuTime
        {
            get { return (float)timeProfiler.microseconds / 1000.0f; }
        }
        float gpuTime => gpuReadback.gpuTime;

        FRHIBuffer buffer
        {
            get { return bufferRef.buffer; }
        }
        FRHIBufferRef bufferRef;
        FTimeProfiler timeProfiler;
        FRHIMemoryReadback gpuReadback;

        public override void OnEnable()
        {
            Console.WriteLine("Enable Component");

            readData = new int[numData];
            timeProfiler = new FTimeProfiler();

            FGraphics.AddTask((FRenderContext renderContext) =>
            {
                FBufferDescriptor descriptor = new FBufferDescriptor(numData, 4, EUsageType.UnorderAccess, EStorageType.Default | EStorageType.Dynamic | EStorageType.Staging);
                descriptor.name = "TestBuffer";

                bufferRef = renderContext.GetBuffer(descriptor);
                gpuReadback = renderContext.CreateMemoryReadback("TestReadback", true);

                FRHICommandBuffer cmdBuffer = renderContext.GetCommandBuffer("Upload", EContextType.Copy);
                cmdBuffer.Clear();

                int[] data = new int[numData];
                for (int i = 0; i < numData; ++i) { 
                    data[i] = numData - i; 
                }

                cmdBuffer.BeginEvent("Upload");
                buffer.SetData(cmdBuffer, data);
                cmdBuffer.EndEvent();
                renderContext.ExecuteCommandBuffer(cmdBuffer);
                renderContext.ReleaseCommandBuffer(cmdBuffer);
            });
        }

        public override void OnUpdate(in float deltaTime)
        {
            FGraphics.AddTask((FRenderContext renderContext) =>
            {
                gpuReadback.EnqueueCopy(renderContext, buffer);

                timeProfiler.Start();
                gpuReadback.GetData(renderContext, buffer, readData);
                timeProfiler.Stop();

                Console.WriteLine("||");
                Console.WriteLine("CPUCopy : " + cpuTime + "ms");
                Console.WriteLine("GPUCopy : " + gpuTime + "ms");
            });
        }

        public override void OnDisable()
        {
            FGraphics.AddTask((FRenderContext renderContext) =>
            {
                gpuReadback.Dispose();
                renderContext.ReleaseBuffer(bufferRef);
                Console.WriteLine("Release RenderProxy");
            });

            Console.WriteLine("Disable Component");
        }
    }

    [Serializable]
    public class TestActor : AActor
    {
        //FTaskHandle m_AsynTaskRef;
        private TestComponent m_Component;

        public TestActor() : base()
        {
            m_Component = new TestComponent();
            //AddComponent(m_Component);
        }

        public TestActor(string name) : base(name)
        {
            m_Component = new TestComponent();
            //AddComponent(m_Component);
        }

        public TestActor(string name, AActor parent) : base(name, parent)
        {
            m_Component = new TestComponent();
            //AddComponent(m_Component);
        }

        public override void OnEnable()
        {
            base.OnEnable();
            AddComponent(m_Component);
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

    public class TestApplication : FApplication
    {
        private TestActor m_Actor;

        public TestApplication(string name, int width, int height) : base(width, height, name)
        {
            m_Actor = new TestActor("TestActor");
        }

        protected override void Play()
        {
            m_Actor.OnEnable();
        }

        protected override void Tick()
        {
            m_Actor.OnUpdate(FGameTime.DeltaTime);
        }

        protected override void End()
        {
            m_Actor.OnDisable();
        }
    }
}
