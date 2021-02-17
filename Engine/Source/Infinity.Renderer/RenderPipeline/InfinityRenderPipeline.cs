﻿using System;
using InfinityEngine.Graphics.RHI;

namespace InfinityEngine.Renderer.RenderPipeline
{
    public class FInfinityRenderPipeline : FRenderPipeline
    {
        public FInfinityRenderPipeline(string PipelineName) : base(PipelineName)
        {

        }

        public override void Init(FRHIRenderContext RenderContext)
        {

        }

        public override void Render(FRHIRenderContext RenderContext)
        {
            Console.WriteLine("Rendering");
            //ResourceBind Example
            /*FRHIBuffer Buffer = RenderContext.CreateBuffer(16, 4, EUseFlag.CPUWrite, EBufferType.Structured);

            FRHIShaderResourceView SRV = RenderContext.CreateShaderResourceView(Buffer);
            FRHIUnorderedAccessView UAV = RenderContext.CreateUnorderedAccessView(Buffer);

            FRHIResourceViewRange ResourceViewRange = RenderContext.CreateResourceViewRange(2);
            ResourceViewRange.SetShaderResourceView(0, SRV);
            ResourceViewRange.SetUnorderedAccessView(1, UAV);*/


            //ASyncCompute Example
            /*FRHIFence ComputeFence = RenderContext.CreateGPUFence();
            FRHIFence GraphicsFence = RenderContext.CreateGPUFence();

            //Pass-A in GraphicsQueue
            CmdBuffer.DrawPrimitiveInstance(null, null, PrimitiveTopology.TriangleList, 0, 0);
            RenderContext.ExecuteCmdBuffer(EContextType.Graphics, CmdBuffer);
            RenderContext.WritFence(EContextType.Graphics, GraphicsFence);

            //Pass-B in GraphicsQueue
            CmdBuffer.DrawPrimitiveInstance(null, null, PrimitiveTopology.TriangleList, 0, 0);
            RenderContext.ExecuteCmdBuffer(EContextType.Graphics, CmdBuffer);

            //Pass-C in ComputeQueue and Wait Pass-A
            RenderContext.WaitFence(EContextType.Compute, GraphicsFence);
            CmdBuffer.DispatchCompute(null, 16, 16, 1);
            RenderContext.ExecuteCmdBuffer(EContextType.Compute, CmdBuffer);
            RenderContext.WritFence(EContextType.Compute, ComputeFence);

            //Pass-D in ComputeQueue
            CmdBuffer.DispatchCompute(null, 16, 16, 1);
            RenderContext.ExecuteCmdBuffer(EContextType.Compute, CmdBuffer);

            //Pass-E in GraphicsQueue and Wait Pass-C
            RenderContext.WaitFence(EContextType.Graphics, ComputeFence);
            CmdBuffer.DrawPrimitiveInstance(null, null, PrimitiveTopology.TriangleList, 128, 16);
            RenderContext.ExecuteCmdBuffer(EContextType.Graphics, CmdBuffer);*/

            //Submit Context
            //RenderContext.Submit();
        }

        protected override void Disposed()
        {
            base.Disposed();
        }
    }
}
