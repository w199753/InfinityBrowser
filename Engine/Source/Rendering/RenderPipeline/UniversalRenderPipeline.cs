using System;
using InfinityEngine.Graphics.RHI;
using InfinityEngine.Core.Mathmatics;
using InfinityEngine.Rendering.RenderLoop;

namespace InfinityEngine.Rendering.RenderPipeline
{
    public class UniversalRenderPipeline : RenderPipeline
    {
        public UniversalRenderPipeline(string pipelineName) : base(pipelineName) 
        {

        }

        public override void Init(RenderContext renderContext)
        {
            Console.WriteLine("Init RenderPipeline");
        }

        public override void Render(RenderContext renderContext)
        {
            RHICommandBuffer cmdBuffer = renderContext.GetCommandBuffer("ClearRenderTarget");
            cmdBuffer.Clear();

            cmdBuffer.BeginEvent("ClearBackBuffer");
            cmdBuffer.ClearRenderTarget(renderContext.backBufferView, new float4(1, 0.25f, 0.125f, 1));
            cmdBuffer.EndEvent();

            renderContext.ExecuteCommandBuffer(cmdBuffer);
            renderContext.ReleaseCommandBuffer(cmdBuffer);
        }

        public override void Release(RenderContext renderContext)
        {
            Console.WriteLine("Release RenderPipeline");
        }
    }
}




















//ResourceBind Example
/*RHIBuffer Buffer = context.CreateBuffer(16, 4, EUseFlag.CPUWrite, EBufferType.Structured);

RHIShaderResourceView SRV = context.CreateShaderResourceView(Buffer);
RHIUnorderedAccessView UAV = context.CreateUnorderedAccessView(Buffer);

RHIResourceViewRange ResourceViewRange = context.CreateResourceViewRange(2);
ResourceViewRange.SetShaderResourceView(0, SRV);
ResourceViewRange.SetUnorderedAccessView(1, UAV);*/

//ASyncCompute Example
/*RHIFence computeFence = context.CreateFence();
RHIFence graphicsFence = context.CreateFence();

//Pass-A in GraphicsQueue
cmdList.DrawPrimitiveInstance(null, null, PrimitiveTopology.TriangleList, 0, 0);
context.ExecuteCmdList(EContextType.Graphics, cmdList);
context.WritFence(EContextType.Graphics, graphicsFence);

//Pass-B in GraphicsQueue
cmdList.DrawPrimitiveInstance(null, null, PrimitiveTopology.TriangleList, 0, 0);
context.ExecuteCmdList(EContextType.Graphics, cmdList);

//Pass-C in ComputeQueue and Wait Pass-A
context.WaitFence(EContextType.Compute, graphicsFence);
cmdList.DispatchCompute(null, 16, 16, 1);
context.ExecuteCmdList(EContextType.Compute, cmdList);
context.WritFence(EContextType.Compute, computeFence);

//Pass-D in ComputeQueue
cmdList.DispatchCompute(null, 16, 16, 1);
context.ExecuteCmdList(EContextType.Compute, cmdList);

//Pass-E in GraphicsQueue and Wait Pass-C
context.WaitFence(EContextType.Graphics, computeFence);
cmdList.DrawPrimitiveInstance(null, null, PrimitiveTopology.TriangleList, 128, 16);
context.ExecuteCmdList(EContextType.Graphics, cmdList);*/