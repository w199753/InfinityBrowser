using System;
using Infinity.Graphics;
using Infinity.Mathmatics;

namespace Infinity.Rendering
{
    public class HybridRenderPipeline : RenderPipeline
    {
        RHIGraphicsPassColorAttachment[] m_ColorAttachments;

        public HybridRenderPipeline(string pipelineName) : base(pipelineName) 
        {

        }

        public override void Init(RenderContext renderContext)
        {
            Console.WriteLine("Init RenderPipeline");

            m_ColorAttachments = new RHIGraphicsPassColorAttachment[1];
            {
                m_ColorAttachments[0].clearValue = new float4(1, 0.25f, 0, 1);
                m_ColorAttachments[0].loadOp = ELoadOp.Clear;
                m_ColorAttachments[0].storeOp = EStoreOp.Store;
                m_ColorAttachments[0].resolveTarget = null;
            }
        }

        public override void Render(RenderContext renderContext)
        {
            RHICommandBuffer m_CommandBuffer = renderContext.GetCommandBuffer(EContextType.Graphics);

            using (m_CommandBuffer.BeginScoped())
            {
                m_ColorAttachments[0].view = renderContext.BackBufferView;

                RHIBlitEncoder blitEncoder = m_CommandBuffer.GetBlitEncoder();
                RHIGraphicsEncoder graphicsEncoder = m_CommandBuffer.GetGraphicsEncoder();

                using (blitEncoder.BeginScopedPass())
                {
                    blitEncoder.ResourceBarrier(RHIBarrier.Transition(renderContext.BackBuffer, ETextureState.Present, ETextureState.RenderTarget));
                }

                RHIGraphicsPassBeginInfo graphicsPassBeginInfo = new RHIGraphicsPassBeginInfo();
                graphicsPassBeginInfo.colorAttachments = new Memory<RHIGraphicsPassColorAttachment>(m_ColorAttachments);
                graphicsPassBeginInfo.depthStencilAttachment = null;
                using (graphicsEncoder.BeginScopedPass(graphicsPassBeginInfo))
                {
                    //graphicsEncoder.SetPipeline(pipeline);
                    graphicsEncoder.SetScissor(0, 0, 1280, 720);
                    graphicsEncoder.SetViewport(0, 0, 1280, 720, 0, 1);
                    //graphicsEncoder.SetPrimitiveTopology(EPrimitiveTopology.TriangleList);
                    //graphicsEncoder.SetVertexBuffer(0, vertexBufferView);
                    //graphicsEncoder.Draw(3, 1, 0, 0);
                }

                using (blitEncoder.BeginScopedPass())
                {
                    blitEncoder.ResourceBarrier(RHIBarrier.Transition(renderContext.BackBuffer, ETextureState.RenderTarget, ETextureState.Present));
                }
            }

            m_CommandBuffer.Commit();
        }

        protected override void Release()
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