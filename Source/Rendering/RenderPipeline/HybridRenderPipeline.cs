using System;
using Infinity.Memory;
using Infinity.Graphics;
using Infinity.Mathmatics;
using System.Runtime.InteropServices;

namespace Infinity.Rendering
{
    internal struct Vertex
    {
        public float4 color;
        public float4 position;
    };

    public unsafe class HybridRenderPipeline : RenderPipeline
    {
        RHIShader m_VertexShader;
        RHIShader m_FragmentShader;
        RHIShader m_ComputeShader;
        RHIBuffer m_VertexBuffer;
        RHITexture m_ComputeTexture;
        RHITextureView m_ComputeTextureView;
        RHIBindGroup m_ComputeBindGroup;
        RHIComputePipeline m_ComputePipeline;
        RHIGraphicsPipeline m_GraphicsPipeline;
        RHIPipelineLayout m_ComputePipelineLayout;
        RHIPipelineLayout m_GraphicsPipelineLayout;
        RHIBindGroupLayout m_ComputeBindGroupLayout;
        RHIBindGroupLayout m_GraphicsBindGroupLayout;
        Vortice.Dxc.IDxcBlob m_VertexBlob;
        Vortice.Dxc.IDxcBlob m_FragmentBlob;
        Vortice.Dxc.IDxcBlob m_ComputeBlob;
        Vortice.Dxc.IDxcResult m_VertexResult;
        Vortice.Dxc.IDxcResult m_FragmentResult;
        Vortice.Dxc.IDxcResult m_ComputeResult;
        RHIGraphicsPassColorAttachment[] m_ColorAttachments;

        public HybridRenderPipeline(string pipelineName) : base(pipelineName) 
        {
            string computeCode = new string(@"
            RWTexture2D<float4> ResultTexture : register(u1, space2);

            [numthreads(8, 8, 1)]
            void Main (uint3 id : SV_DispatchThreadID)
            {
                float2 UV = (id.xy + 0.5) / float2(1600, 900);
                ResultTexture[id.xy] = float4(UV, 0, 1);
                //ResultTexture[id.xy] = float4(id.x & id.y, (id.x & 15) / 15, (id.y & 15) / 15, 0.25);
            }");

            string graphicsCode = new string(
            @"
            Texture2D<float4> _DiffuseTexture : register(t2, space1);
            SamplerState _DiffuseTextureSampler : register(s2, space1);

            struct Attributes
	        {
		        float4 color : COLOR;
		        float4 position : POSITION;
	        };

            struct Varyings
            {
	            float4 color : COLOR;
	            float4 position : SV_POSITION;
            };

            Varyings Vertex(Attributes input)
            {
	            Varyings output = (Varyings)0;
	            output.color = input.color;
	            output.position = input.position;
	            return output;
            }

            float4 Fragment(Varyings input) : SV_Target
            {
                return input.color;
	            //return _DiffuseTexture.Sample(Sampler_DiffuseTexture, input.uv * Tiling);
            }");

            m_ComputeResult = Vortice.Dxc.DxcCompiler.Compile(Vortice.Dxc.DxcShaderStage.Compute, computeCode, "Main");
            m_ComputeBlob = m_ComputeResult.GetOutput(Vortice.Dxc.DxcOutKind.Object);

            m_VertexResult = Vortice.Dxc.DxcCompiler.Compile(Vortice.Dxc.DxcShaderStage.Vertex, graphicsCode, "Vertex");
            m_VertexBlob = m_VertexResult.GetOutput(Vortice.Dxc.DxcOutKind.Object);

            m_FragmentResult = Vortice.Dxc.DxcCompiler.Compile(Vortice.Dxc.DxcShaderStage.Pixel, graphicsCode, "Fragment");
            m_FragmentBlob = m_FragmentResult.GetOutput(Vortice.Dxc.DxcOutKind.Object);
        }

        public override void Init(RenderContext renderContext)
        {
            Console.WriteLine("Init RenderPipeline");

            // CreateOutputTexture
            RHITextureCreateInfo textureCreateInfo;
            {
                textureCreateInfo.extent = new int3(1600, 900, 1);
                textureCreateInfo.samples = 1;
                textureCreateInfo.mipLevels = 1;
                textureCreateInfo.state = ETextureState.Common;
                textureCreateInfo.format = EPixelFormat.RGBA8_UNorm;
                textureCreateInfo.dimension = ETextureDimension.Tex2D;
                textureCreateInfo.usages = ETextureUsage.ColorAttachment | ETextureUsage.StorageResource;
            }
            m_ComputeTexture = renderContext.CreateTexture(textureCreateInfo);

            RHITextureViewCreateInfo outputViewCreateInfo;
            {
                outputViewCreateInfo.mipLevelNum = 1;
                outputViewCreateInfo.baseMipLevel = 0;
                outputViewCreateInfo.arrayLayerNum = 1;
                outputViewCreateInfo.baseArrayLayer = 0;
                outputViewCreateInfo.format = EPixelFormat.RGBA8_UNorm;
                outputViewCreateInfo.type = ETextureViewType.UnorderedAccess;
                outputViewCreateInfo.dimension = ETextureViewDimension.Tex2D;
            }
            m_ComputeTextureView = m_ComputeTexture.CreateTextureView(outputViewCreateInfo);

            // CreateComputeBindGroupLayout
            RHIBindGroupLayoutElement[] computeBindGroupLayoutElements = new RHIBindGroupLayoutElement[1];
            {
                computeBindGroupLayoutElements[0].slot = 1;
                computeBindGroupLayoutElements[0].count = 1;
                computeBindGroupLayoutElements[0].bindType = EBindType.StorageTexture;
                computeBindGroupLayoutElements[0].shaderStage = EShaderStageFlags.Compute;
            }
            RHIBindGroupLayoutCreateInfo computeBindGroupLayoutCreateInfo;
            {
                computeBindGroupLayoutCreateInfo.layoutIndex = 2;
                computeBindGroupLayoutCreateInfo.elements = new Memory<RHIBindGroupLayoutElement>(computeBindGroupLayoutElements);
            }
            m_ComputeBindGroupLayout = renderContext.CreateBindGroupLayout(computeBindGroupLayoutCreateInfo);

            // CreateComputeBindGroup
            RHIBindGroupElement[] computeBindGroupElements = new RHIBindGroupElement[1];
            {
                computeBindGroupElements[0].textureView = m_ComputeTextureView;
            }
            RHIBindGroupCreateInfo computeBindGroupCreateInfo;
            {
                computeBindGroupCreateInfo.layout = m_ComputeBindGroupLayout;
                computeBindGroupCreateInfo.elements = new Memory<RHIBindGroupElement>(computeBindGroupElements);
            }
            m_ComputeBindGroup = renderContext.CreateBindGroup(computeBindGroupCreateInfo);

            // CreateComputePipeline
            RHIShaderCreateInfo computeShaderCreateInfo;
            {
                computeShaderCreateInfo.size = m_ComputeBlob.BufferSize;
                computeShaderCreateInfo.byteCode = m_ComputeBlob.BufferPointer;
            }
            m_ComputeShader = renderContext.CreateShader(computeShaderCreateInfo);

            RHIPipelineLayoutCreateInfo computePipelienLayoutCreateInfo;
            {
                computePipelienLayoutCreateInfo.bindGroupLayouts = new RHIBindGroupLayout[] { m_ComputeBindGroupLayout };
            }
            m_ComputePipelineLayout = renderContext.CreatePipelineLayout(computePipelienLayoutCreateInfo);

            RHIComputePipelineCreateInfo computePipelineCreateInfo;
            {
                computePipelineCreateInfo.threadSize = new uint3(8, 8, 1);
                computePipelineCreateInfo.computeShader = m_ComputeShader;
                computePipelineCreateInfo.pipelineLayout = m_ComputePipelineLayout;
            }
            m_ComputePipeline = renderContext.CreateComputePipeline(computePipelineCreateInfo);

            // CreateSampler
            //RHISamplerCreateInfo samplerCreateInfo = new RHISamplerCreateInfo();
            //RHISampler textureSampler = device.CreateSampler(samplerCreateInfo);

            // CreateUniformBuffer
            /*RHIBufferViewCreateInfo bufferViewCreateInfo = new RHIBufferViewCreateInfo();
            bufferViewCreateInfo.offset = 0;
            bufferViewCreateInfo.count = vertices.Length;
            bufferViewCreateInfo.type = EBufferViewType.UniformBuffer;
            bufferViewCreateInfo.stride = (bufferCreateInfo.size + 255) & ~255;
            RHIBufferView uniformBufferView = vertexBuffer.CreateBufferView(bufferViewCreateInfo);*/

            // CreateVertexBuffer
            Vertex[] vertices = new Vertex[3];
            {
                vertices[0].color = new float4(1, 0, 0, 1);
                vertices[0].position = new float4(-0.5f, -0.5f, 0, 1);
                vertices[1].color = new float4(0, 1, 0, 1);
                vertices[1].position = new float4(0, 0.5f, 0, 1);
                vertices[2].color = new float4(0, 0, 1, 1);
                vertices[2].position = new float4(0.5f, -0.5f, 0, 1);
            }
            RHIBufferCreateInfo bufferCreateInfo;
            bufferCreateInfo.size = vertices.Length * MemoryUtility.SizeOf<Vertex>();
            bufferCreateInfo.state = EBufferState.Common;
            bufferCreateInfo.usages = EBufferUsage.Uniform | EBufferUsage.MapWrite | EBufferUsage.CopySrc;
            m_VertexBuffer = renderContext.CreateBuffer(bufferCreateInfo);

            IntPtr data = m_VertexBuffer.Map(EMapMode.Write, 0, bufferCreateInfo.size);
            GCHandle verticesHandle = GCHandle.Alloc(vertices, GCHandleType.Pinned);
            IntPtr verticesPtr = verticesHandle.AddrOfPinnedObject();
            MemoryUtility.MemCpy(verticesPtr.ToPointer(), data.ToPointer(), bufferCreateInfo.size);
            verticesHandle.Free();

            // CreateGraphicsBindGroupLayout
            RHIBindGroupLayoutElement[] graphicsBindGroupLayoutElements = new RHIBindGroupLayoutElement[2];
            {
                graphicsBindGroupLayoutElements[0].slot = 2;
                graphicsBindGroupLayoutElements[0].count = 1;
                graphicsBindGroupLayoutElements[0].bindType = EBindType.Texture;
                graphicsBindGroupLayoutElements[0].shaderStage = EShaderStageFlags.Fragment;

                graphicsBindGroupLayoutElements[1].slot = 2;
                graphicsBindGroupLayoutElements[1].count = 1;
                graphicsBindGroupLayoutElements[1].bindType = EBindType.Sampler;
                graphicsBindGroupLayoutElements[1].shaderStage = EShaderStageFlags.Fragment;
            }
            RHIBindGroupLayoutCreateInfo graphicsBindGroupLayoutCreateInfo;
            {
                graphicsBindGroupLayoutCreateInfo.layoutIndex = 1;
                graphicsBindGroupLayoutCreateInfo.elements = new Memory<RHIBindGroupLayoutElement>(graphicsBindGroupLayoutElements);
            }
            m_GraphicsBindGroupLayout = renderContext.CreateBindGroupLayout(graphicsBindGroupLayoutCreateInfo);

            // CreateGraphicsBindGroup
            /*RHIBindGroupElement[] graphicsBindGroupElements = new RHIBindGroupElement[2];
            {
                graphicsBindGroupElements[0].textureView = textureView;
                graphicsBindGroupElements[1].textureSampler = textureSampler;
            }
            RHIBindGroupCreateInfo graphicsBindGroupCreateInfo = new RHIBindGroupCreateInfo();
            graphicsBindGroupCreateInfo.layout = m_GraphicsBindGroupLayout;
            graphicsBindGroupCreateInfo.elements = new Memory<RHIBindGroupElement>(graphicsBindGroupElements);
            RHIBindGroup m_GraphicsBindGroup = device.CreateBindGroup(graphicsBindGroupCreateInfo);*/

            // CreateGraphicsPipeline
            RHIShaderCreateInfo vertexShaderCreateInfo;
            {
                vertexShaderCreateInfo.size = m_VertexBlob.BufferSize;
                vertexShaderCreateInfo.byteCode = m_VertexBlob.BufferPointer;
            }
            m_VertexShader = renderContext.CreateShader(vertexShaderCreateInfo);

            RHIShaderCreateInfo fragmentShaderCreateInfo;
            {
                fragmentShaderCreateInfo.size = m_FragmentBlob.BufferSize;
                fragmentShaderCreateInfo.byteCode = m_FragmentBlob.BufferPointer;
            }
            m_FragmentShader = renderContext.CreateShader(fragmentShaderCreateInfo);

            RHIPipelineLayoutCreateInfo graphicsPipelienLayoutCreateInfo;
            {
                graphicsPipelienLayoutCreateInfo.bindGroupLayouts = new RHIBindGroupLayout[] { m_GraphicsBindGroupLayout };
            }
            m_GraphicsPipelineLayout = renderContext.CreatePipelineLayout(graphicsPipelienLayoutCreateInfo);

            RHIOutputAttachmentDescription[] outputColorAttachmentStates = new RHIOutputAttachmentDescription[1];
            {
                outputColorAttachmentStates[0].format = EPixelFormat.RGBA8_UNorm;
                outputColorAttachmentStates[0].resolveMSAA = false;
            }

            RHIOutputAttachmentDescription outputDepthAttachmentState;
            {
                outputDepthAttachmentState.format = EPixelFormat.D32_Float;
                outputDepthAttachmentState.resolveMSAA = false;
            }

            RHIOutputState outputState;
            {
                outputState.arraySliceCount = 1;
                outputState.sampleCount = ESampleCount.None;
                outputState.depthAttachment = outputDepthAttachmentState;
                outputState.colorAttachments = new Memory<RHIOutputAttachmentDescription>(outputColorAttachmentStates);
            }

            RHIVertexAttribute[] vertexAttributes = new RHIVertexAttribute[2];
            {
                vertexAttributes[0].index = 0;
                vertexAttributes[0].offset = 0;
                vertexAttributes[0].type = ESemanticType.Color;
                vertexAttributes[0].format = ESemanticFormat.Float4;

                vertexAttributes[1].index = 0;
                vertexAttributes[1].offset = 16;
                vertexAttributes[1].type = ESemanticType.Position;
                vertexAttributes[1].format = ESemanticFormat.Float4;
            }

            RHIVertexLayout[] vertexLayouts = new RHIVertexLayout[1];
            {
                vertexLayouts[0].stride = MemoryUtility.SizeOf<Vertex>();
                vertexLayouts[0].stepMode = EVertexStepMode.PerVertex;
                vertexLayouts[0].attributes = new Memory<RHIVertexAttribute>(vertexAttributes);
            }

            RHIVertexState vertexState;
            {
                vertexState.primitiveTopology = EPrimitiveTopology.TriangleList;
                vertexState.vertexLayouts = new Memory<RHIVertexLayout>(vertexLayouts);
            }

            RHIBlendStateDescriptor blendState;
            {
                blendState.alphaToCoverage = false;
                blendState.independentBlend = false;
                blendState.attachment0.blendEnable = false;
                blendState.attachment0.blendOperationColor = EBlendOperation.Add;
                blendState.attachment0.blendOperationAlpha = EBlendOperation.Add;
                blendState.attachment0.colorWriteChannel = EColorWriteChannel.All;
                blendState.attachment0.destinationBlendColor = EBlend.Zero;
                blendState.attachment0.destinationBlendAlpha = EBlend.Zero;
                blendState.attachment0.sourceBlendColor = EBlend.One;
                blendState.attachment0.sourceBlendAlpha = EBlend.One;
                blendState.attachment1 = blendState.attachment0;
                blendState.attachment2 = blendState.attachment0;
                blendState.attachment3 = blendState.attachment0;
                blendState.attachment4 = blendState.attachment0;
                blendState.attachment5 = blendState.attachment0;
                blendState.attachment6 = blendState.attachment0;
                blendState.attachment7 = blendState.attachment0;
            }

            RHIRasterizerStateDescriptor rasterizerState;
            {
                rasterizerState.conservativeState = EConservativeState.On;
                rasterizerState.antialiasedLineEnable = false;
                rasterizerState.CullMode = ECullMode.Back;
                rasterizerState.depthBias = 0;
                rasterizerState.depthBiasClamp = 0;
                rasterizerState.depthClipEnable = true;
                rasterizerState.FillMode = EFillMode.Solid;
                rasterizerState.frontCounterClockwise = false;
                rasterizerState.scissorEnable = true;
                rasterizerState.slopeScaledDepthBias = 0;
            }

            RHIDepthStencilStateDescription depthStencilState;
            {
                depthStencilState.depthEnable = true;
                depthStencilState.depthWriteMask = true;
                depthStencilState.depthComparison = EComparison.LessEqual;
                depthStencilState.stencilEnable = true;
                depthStencilState.stencilReadMask = 255;
                depthStencilState.stencilWriteMask = 255;
                depthStencilState.backFace.stencilComparison = EComparison.Always;
                depthStencilState.backFace.stencilPassOperation = EStencilOperation.Keep;
                depthStencilState.backFace.stencilFailOperation = EStencilOperation.Keep;
                depthStencilState.backFace.stencilDepthFailOperation = EStencilOperation.Keep;
                depthStencilState.frontFace.stencilComparison = EComparison.Always;
                depthStencilState.frontFace.stencilPassOperation = EStencilOperation.Keep;
                depthStencilState.frontFace.stencilFailOperation = EStencilOperation.Keep;
                depthStencilState.frontFace.stencilDepthFailOperation = EStencilOperation.Keep;
            }

            RHIRenderState renderState;
            {
                renderState.stencilRef = 0;
                renderState.sampleMask = null;
                renderState.blendFactor = null;
                renderState.blendState = blendState;
                renderState.rasterizerState = rasterizerState;
                renderState.depthStencilState = depthStencilState;
            }

            RHIGraphicsPipelineCreateInfo graphicsPipelineCreateInfo;
            {
                graphicsPipelineCreateInfo.outputState = outputState;
                graphicsPipelineCreateInfo.renderState = renderState;
                graphicsPipelineCreateInfo.vertexState = vertexState;
                graphicsPipelineCreateInfo.vertexShader = m_VertexShader;
                graphicsPipelineCreateInfo.fragmentShader = m_FragmentShader;
                graphicsPipelineCreateInfo.pipelineLayout = m_GraphicsPipelineLayout;
            }
            m_GraphicsPipeline = renderContext.CreateGraphicsPipeline(graphicsPipelineCreateInfo);

            m_ColorAttachments = new RHIGraphicsPassColorAttachment[1];
            {
                m_ColorAttachments[0].clearValue = new float4(0.5f, 0.5f, 1, 1);
                m_ColorAttachments[0].loadOp = ELoadOp.Clear;
                m_ColorAttachments[0].storeOp = EStoreOp.Store;
                m_ColorAttachments[0].resolveTarget = null;
            }
        }

        public override void Render(RenderContext renderContext)
        {
            RHICommandBuffer cmdBuffer = renderContext.GetCommandBuffer(EContextType.Graphics);

            using (cmdBuffer.BeginScoped())
            {
                RHIBlitEncoder blitEncoder = cmdBuffer.GetBlitEncoder();
                RHIComputeEncoder computeEncoder = cmdBuffer.GetComputeEncoder();
                RHIGraphicsEncoder graphicsEncoder = cmdBuffer.GetGraphicsEncoder();

                using (blitEncoder.BeginScopedPass())
                {
                    blitEncoder.ResourceBarrier(RHIBarrier.Transition(m_ComputeTexture, ETextureState.Common, ETextureState.UnorderedAccess));
                    blitEncoder.ResourceBarrier(RHIBarrier.Transition(renderContext.BackBuffer, ETextureState.Present, ETextureState.RenderTarget));
                }

                using (computeEncoder.BeginScopedPass("ComputePass"))
                {
                    computeEncoder.SetPipelineLayout(m_ComputePipelineLayout);
                    computeEncoder.PushDebugGroup("GenereteUV");
                    computeEncoder.SetPipelineState(m_ComputePipeline);
                    computeEncoder.SetBindGroup(m_ComputeBindGroup);
                    computeEncoder.Dispatch((uint)math.ceil(1600 / 8), (uint)math.ceil(900 / 8), 1);
                    computeEncoder.PopDebugGroup();
                }

                m_ColorAttachments[0].renderTarget = renderContext.BackBufferView;
                RHIGraphicsPassBeginInfo graphicsPassBeginInfo = new RHIGraphicsPassBeginInfo();
                graphicsPassBeginInfo.name = "GraphicsPass";
                graphicsPassBeginInfo.colorAttachments = new Memory<RHIGraphicsPassColorAttachment>(m_ColorAttachments);
                graphicsPassBeginInfo.depthStencilAttachment = null;
                using (graphicsEncoder.BeginScopedPass(graphicsPassBeginInfo))
                {
                    graphicsEncoder.SetViewport(0, 0, 1600, 900, 0, 1);
                    graphicsEncoder.SetScissorRect(0, 0, 1600, 900);
                    graphicsEncoder.SetPipelineLayout(m_GraphicsPipelineLayout);
                    graphicsEncoder.PushDebugGroup("DrawTriange");
                    graphicsEncoder.SetPipelineState(m_GraphicsPipeline);
                    graphicsEncoder.SetVertexBuffer(m_VertexBuffer);
                    graphicsEncoder.Draw(3, 1, 0, 0);
                    graphicsEncoder.PopDebugGroup();
                }

                using (blitEncoder.BeginScopedPass())
                {
                    blitEncoder.ResourceBarrier(RHIBarrier.Transition(m_ComputeTexture, ETextureState.UnorderedAccess, ETextureState.Common));
                    blitEncoder.ResourceBarrier(RHIBarrier.Transition(renderContext.BackBuffer, ETextureState.RenderTarget, ETextureState.Present));
                }
            }

            cmdBuffer.Commit();
        }

        protected override void Release()
        {
            m_VertexBlob.Dispose();
            m_FragmentBlob.Dispose();
            m_ComputeBlob.Dispose();
            m_VertexResult.Dispose();
            m_FragmentResult.Dispose();
            m_ComputeResult.Dispose();
            m_VertexBuffer.Dispose();
            m_ComputeTextureView.Dispose();
            m_ComputeTexture.Dispose();
            //textureSampler.Dispose();
            //uniformBufferView.Dispose();
            m_VertexShader.Dispose();
            m_FragmentShader.Dispose();
            m_ComputeShader.Dispose();
            m_ComputePipeline.Dispose();
            m_ComputeBindGroup.Dispose();
            m_ComputeBindGroupLayout.Dispose();
            m_ComputePipelineLayout.Dispose();
            m_GraphicsPipeline.Dispose();
            //m_GraphicsBindGroup.Dispose();
            m_GraphicsPipelineLayout.Dispose();
            m_GraphicsBindGroupLayout.Dispose();
            Console.WriteLine("Release RenderPipeline");
        }
    }
}