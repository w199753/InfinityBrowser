﻿using System;
using Infinity.Graphics;
using Infinity.Rendering;
using Infinity.Analytics;

namespace Infinity.Game
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
    public class TestActor : Actor
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

        public TestActor(string name, Actor parent) : base(name, parent)
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

    public class TestApplication : Application
    {
        private TestActor m_Actor;

        public TestApplication(int width, int height, string name) : base(width, height, name)
        {
            m_Actor = new TestActor("TestActor");
        }

        protected override void Play()
        {
            m_Actor.OnEnable();
        }

        protected override void Tick()
        {
            m_Actor.OnUpdate(Timer.DeltaTime);
        }

        protected override void End()
        {
            m_Actor.OnDisable();
        }
    }

    [Serializable]
    public class MainClass
    {
        static void Main(string[] args)
        {
            TestApplication App = new TestApplication(1280, 720, "InfinityExample");
            App.Execute();

            /*Console.WriteLine("Fuck you");
            Console.ReadKey();*/
        }
    }
}
