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
        Vortice.Dxc.IDxcBlob m_VertexBlob;
        Vortice.Dxc.IDxcBlob m_FragmentBlob;
        Vortice.Dxc.IDxcBlob m_ComputeBlob;
        Vortice.Dxc.IDxcResult m_VertexResult;
        Vortice.Dxc.IDxcResult m_FragmentResult;
        Vortice.Dxc.IDxcResult m_ComputeResult;

        RHIShader m_VertexShader;
        RHIShader m_FragmentShader;
        RHIShader m_ComputeShader;
        RHIBuffer m_IndexBuffer;
        RHIBuffer m_VertexBuffer;
        RHITexture m_ComputeTexture;
        RHITextureView m_ComputeTextureView;
        RHIBindGroup m_ComputeBindGroup;
        //RHIBindGroup m_GraphicsBindGroup;
        RHIComputePipeline m_ComputePipeline;
        RHIGraphicsPipeline m_GraphicsPipeline;
        RHIPipelineLayout m_ComputePipelineLayout;
        RHIPipelineLayout m_GraphicsPipelineLayout;
        RHIBindGroupLayout m_ComputeBindGroupLayout;
        RHIBindGroupLayout m_GraphicsBindGroupLayout;
        RHIColorAttachmentDescriptor[] m_ColorAttachmentDescriptors;

        public HybridRenderPipeline(string pipelineName) : base(pipelineName) 
        {
            string computeCode = new string(@"
            RWTexture2D<float4> ResultTexture : register(u0, space0);

            [numthreads(8, 8, 1)]
            void Main (uint3 id : SV_DispatchThreadID)
            {
                float2 UV = (id.xy + 0.5) / float2(1600, 900);
                ResultTexture[id.xy] = float4(UV, 0, 1);
                //ResultTexture[id.xy] = float4(id.x & id.y, (id.x & 15) / 15, (id.y & 15) / 15, 0.25);
            }");

            string graphicsCode = new string(
            @"
            Texture2D<float4> _DiffuseTexture : register(t0, space0);
            SamplerState _DiffuseTextureSampler : register(s0, space0);

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

            string msl = Evergine.HLSLEverywhere.HLSLTranslator.HLSLTo(computeCode, Evergine.Common.Graphics.ShaderStages.Compute, Evergine.Common.Graphics.GraphicsProfile.Level_12_1, "Main", Evergine.HLSLEverywhere.ShadingLanguage.Msl_iOS);
        }

        public override void Init(RenderContext renderContext)
        {
            Console.WriteLine("Init RenderPipeline");

            // CreateOutputTexture
            RHITextureDescriptor textureDescriptor;
            {
                textureDescriptor.extent = new int3(renderContext.ScreenSize.xy, 1);
                textureDescriptor.samples = 1;
                textureDescriptor.mipLevels = 1;
                textureDescriptor.format = EPixelFormat.RGBA8_UNorm;
                textureDescriptor.state = ETextureState.Common;
                textureDescriptor.usage = ETextureUsage.RenderTarget | ETextureUsage.UnorderedAccess;
                textureDescriptor.dimension = ETextureDimension.Texture2D;
                textureDescriptor.storageMode = EStorageMode.Default;
            }
            m_ComputeTexture = renderContext.CreateTexture(textureDescriptor);

            RHITextureViewDescriptor outputViewDescriptor;
            {
                outputViewDescriptor.mipLevelNum = textureDescriptor.mipLevels;
                outputViewDescriptor.baseMipLevel = 0;
                outputViewDescriptor.arrayLayerNum = 1;
                outputViewDescriptor.baseArrayLayer = 0;
                outputViewDescriptor.format = EPixelFormat.RGBA8_UNorm;
                outputViewDescriptor.viewType = ETextureViewType.UnorderedAccessView;
                outputViewDescriptor.dimension = ETextureViewDimension.Texture2D;
            }
            m_ComputeTextureView = m_ComputeTexture.CreateTextureView(outputViewDescriptor);

            // CreateComputeBindGroupLayout
            RHIBindGroupLayoutElement[] computeBindGroupLayoutElements = new RHIBindGroupLayoutElement[1];
            {
                computeBindGroupLayoutElements[0].slot = 0;
                computeBindGroupLayoutElements[0].count = 1;
                computeBindGroupLayoutElements[0].bindType = EBindType.StorageTexture;
                computeBindGroupLayoutElements[0].shaderStage = EShaderStageFlag.Compute;
            }
            RHIBindGroupLayoutDescriptor computeBindGroupLayoutDescriptor;
            {
                computeBindGroupLayoutDescriptor.layoutIndex = 0;
                computeBindGroupLayoutDescriptor.elements = new Memory<RHIBindGroupLayoutElement>(computeBindGroupLayoutElements);
            }
            m_ComputeBindGroupLayout = renderContext.CreateBindGroupLayout(computeBindGroupLayoutDescriptor);

            // CreateComputeBindGroup
            RHIBindGroupElement[] computeBindGroupElements = new RHIBindGroupElement[1];
            {
                computeBindGroupElements[0].textureView = m_ComputeTextureView;
            }
            RHIBindGroupDescriptor computeBindGroupDescriptor;
            {
                computeBindGroupDescriptor.layout = m_ComputeBindGroupLayout;
                computeBindGroupDescriptor.elements = new Memory<RHIBindGroupElement>(computeBindGroupElements);
            }
            m_ComputeBindGroup = renderContext.CreateBindGroup(computeBindGroupDescriptor);

            // CreateComputePipeline
            RHIShaderDescriptor computeShaderDescriptor;
            {
                computeShaderDescriptor.size = m_ComputeBlob.BufferSize;
                computeShaderDescriptor.byteCode = m_ComputeBlob.BufferPointer;
            }
            m_ComputeShader = renderContext.CreateShader(computeShaderDescriptor);

            RHIPipelineLayoutDescriptor computePipelienLayoutDescriptor;
            {
                computePipelienLayoutDescriptor.bindGroupLayouts = new RHIBindGroupLayout[] { m_ComputeBindGroupLayout };
            }
            m_ComputePipelineLayout = renderContext.CreatePipelineLayout(computePipelienLayoutDescriptor);

            RHIComputePipelineDescriptor computePipelineDescriptor;
            {
                computePipelineDescriptor.threadSize = new uint3(8, 8, 1);
                computePipelineDescriptor.computeShader = m_ComputeShader;
                computePipelineDescriptor.pipelineLayout = m_ComputePipelineLayout;
            }
            m_ComputePipeline = renderContext.CreateComputePipeline(computePipelineDescriptor);

            // CreateSampler
            //RHISamplerDescriptor samplerDescriptor = new RHISamplerDescriptor();
            //RHISampler textureSampler = device.CreateSampler(samplerDescriptor);

            // CreateUniformBuffer
            /*RHIBufferViewDescriptor bufferViewDescriptor = new RHIBufferViewDescriptor();
            bufferViewDescriptor.offset = 0;
            bufferViewDescriptor.count = vertices.Length;
            bufferViewDescriptor.type = EBufferViewType.UniformBuffer;
            bufferViewDescriptor.stride = (bufferDescriptor.size + 255) & ~255;
            RHIBufferView uniformBufferView = vertexBuffer.CreateBufferView(bufferViewDescriptor);*/

            // CreateIndexBuffer
            ushort[] indexs = new ushort[3];
            {
                indexs[0] = 0;
                indexs[1] = 1;
                indexs[2] = 2;
            }
            RHIBufferDescriptor indexBufferDescriptor;
            {
                indexBufferDescriptor.size = indexs.Length * MemoryUtility.SizeOf<ushort>();
                indexBufferDescriptor.state = EBufferState.Common;
                indexBufferDescriptor.usage = EBufferUsage.IndexBuffer;
                indexBufferDescriptor.storageMode = EStorageMode.Dynamic;
            }
            m_IndexBuffer = renderContext.CreateBuffer(indexBufferDescriptor);

            IntPtr indexData = m_IndexBuffer.Map(indexBufferDescriptor.size, 0);
            GCHandle indexsHandle = GCHandle.Alloc(indexs, GCHandleType.Pinned);
            IntPtr indexsPtr = indexsHandle.AddrOfPinnedObject();
            MemoryUtility.MemCpy(indexsPtr.ToPointer(), indexData.ToPointer(), indexBufferDescriptor.size);
            indexsHandle.Free();

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
            RHIBufferDescriptor vertexBufferDescriptor;
            {
                vertexBufferDescriptor.size = vertices.Length * MemoryUtility.SizeOf<Vertex>();
                vertexBufferDescriptor.state = EBufferState.Common;
                vertexBufferDescriptor.usage = EBufferUsage.VertexBuffer;
                vertexBufferDescriptor.storageMode = EStorageMode.Dynamic;
            }
            m_VertexBuffer = renderContext.CreateBuffer(vertexBufferDescriptor);

            IntPtr vertexData = m_VertexBuffer.Map(vertexBufferDescriptor.size, 0);
            GCHandle verticesHandle = GCHandle.Alloc(vertices, GCHandleType.Pinned);
            IntPtr verticesPtr = verticesHandle.AddrOfPinnedObject();
            MemoryUtility.MemCpy(verticesPtr.ToPointer(), vertexData.ToPointer(), vertexBufferDescriptor.size);
            verticesHandle.Free();

            // CreateGraphicsBindGroupLayout
            RHIBindGroupLayoutElement[] graphicsBindGroupLayoutElements = new RHIBindGroupLayoutElement[2];
            {
                graphicsBindGroupLayoutElements[0].slot = 0;
                graphicsBindGroupLayoutElements[0].count = 1;
                graphicsBindGroupLayoutElements[0].bindType = EBindType.Texture;
                graphicsBindGroupLayoutElements[0].shaderStage = EShaderStageFlag.Fragment;

                graphicsBindGroupLayoutElements[1].slot = 0;
                graphicsBindGroupLayoutElements[1].count = 1;
                graphicsBindGroupLayoutElements[1].bindType = EBindType.Sampler;
                graphicsBindGroupLayoutElements[1].shaderStage = EShaderStageFlag.Fragment;
            }
            RHIBindGroupLayoutDescriptor graphicsBindGroupLayoutDescriptor;
            {
                graphicsBindGroupLayoutDescriptor.layoutIndex = 0;
                graphicsBindGroupLayoutDescriptor.elements = new Memory<RHIBindGroupLayoutElement>(graphicsBindGroupLayoutElements);
            }
            m_GraphicsBindGroupLayout = renderContext.CreateBindGroupLayout(graphicsBindGroupLayoutDescriptor);

            // CreateGraphicsBindGroup
            /*RHIBindGroupElement[] graphicsBindGroupElements = new RHIBindGroupElement[2];
            {
                graphicsBindGroupElements[0].textureView = textureView;
                graphicsBindGroupElements[1].textureSampler = textureSampler;
            }
            RHIBindGroupDescriptor graphicsBindGroupDescriptor = new RHIBindGroupDescriptor();
            graphicsBindGroupDescriptor.layout = m_GraphicsBindGroupLayout;
            graphicsBindGroupDescriptor.elements = new Memory<RHIBindGroupElement>(graphicsBindGroupElements);
            RHIBindGroup m_GraphicsBindGroup = device.CreateBindGroup(graphicsBindGroupDescriptor);*/

            // CreateGraphicsPipeline
            RHIShaderDescriptor vertexShaderDescriptor;
            {
                vertexShaderDescriptor.size = m_VertexBlob.BufferSize;
                vertexShaderDescriptor.byteCode = m_VertexBlob.BufferPointer;
            }
            m_VertexShader = renderContext.CreateShader(vertexShaderDescriptor);

            RHIShaderDescriptor fragmentShaderDescriptor;
            {
                fragmentShaderDescriptor.size = m_FragmentBlob.BufferSize;
                fragmentShaderDescriptor.byteCode = m_FragmentBlob.BufferPointer;
            }
            m_FragmentShader = renderContext.CreateShader(fragmentShaderDescriptor);

            RHIPipelineLayoutDescriptor graphicsPipelienLayoutDescriptor;
            {
                graphicsPipelienLayoutDescriptor.bindGroupLayouts = new RHIBindGroupLayout[] { m_GraphicsBindGroupLayout };
            }
            m_GraphicsPipelineLayout = renderContext.CreatePipelineLayout(graphicsPipelienLayoutDescriptor);

            RHIOutputAttachmentDescriptor[] outputColorAttachmentDescriptors = new RHIOutputAttachmentDescriptor[1];
            {
                outputColorAttachmentDescriptors[0].format = EPixelFormat.RGBA8_UNorm;
                outputColorAttachmentDescriptors[0].resolveMSAA = false;
            }

            RHIOutputAttachmentDescriptor outputDepthAttachmentDescriptor;
            {
                outputDepthAttachmentDescriptor.format = EPixelFormat.D32_Float;
                outputDepthAttachmentDescriptor.resolveMSAA = false;
            }

            RHIOutputStateDescriptor outputStateDescriptor;
            {
                outputStateDescriptor.sampleCount = ESampleCount.None;
                outputStateDescriptor.depthAttachmentDescriptor = outputDepthAttachmentDescriptor;
                outputStateDescriptor.colorAttachmentDescriptors = new Memory<RHIOutputAttachmentDescriptor>(outputColorAttachmentDescriptors);
            }

            RHIVertexAttributeDescriptor[] vertexAttributeDescriptors = new RHIVertexAttributeDescriptor[2];
            {
                vertexAttributeDescriptors[0].index = 0;
                vertexAttributeDescriptors[0].offset = 0;
                vertexAttributeDescriptors[0].type = ESemanticType.Color;
                vertexAttributeDescriptors[0].format = ESemanticFormat.Float4;

                vertexAttributeDescriptors[1].index = 0;
                vertexAttributeDescriptors[1].offset = 16;
                vertexAttributeDescriptors[1].type = ESemanticType.Position;
                vertexAttributeDescriptors[1].format = ESemanticFormat.Float4;
            }

            RHIVertexLayoutDescriptor[] vertexLayoutDescriptors = new RHIVertexLayoutDescriptor[1];
            {
                vertexLayoutDescriptors[0].stride = MemoryUtility.SizeOf<Vertex>();
                vertexLayoutDescriptors[0].stepMode = EVertexStepMode.PerVertex;
                vertexLayoutDescriptors[0].attributeDescriptors = new Memory<RHIVertexAttributeDescriptor>(vertexAttributeDescriptors);
            }

            RHIVertexStateDescriptor vertexStateDescriptor;
            {
                vertexStateDescriptor.primitiveTopology = EPrimitiveTopology.TriangleList;
                vertexStateDescriptor.vertexLayoutDescriptors = new Memory<RHIVertexLayoutDescriptor>(vertexLayoutDescriptors);
            }

            RHIBlendStateDescriptor blendStateDescriptor;
            {
                blendStateDescriptor.alphaToCoverage = false;
                blendStateDescriptor.independentBlend = false;
                blendStateDescriptor.attachmentDescriptor0.blendEnable = false;
                blendStateDescriptor.attachmentDescriptor0.blendOpColor = EBlendOp.Add;
                blendStateDescriptor.attachmentDescriptor0.blendOpAlpha = EBlendOp.Add;
                blendStateDescriptor.attachmentDescriptor0.colorWriteChannel = EColorWriteChannel.All;
                blendStateDescriptor.attachmentDescriptor0.srcBlendColor = EBlendMode.One;
                blendStateDescriptor.attachmentDescriptor0.srcBlendAlpha = EBlendMode.One;
                blendStateDescriptor.attachmentDescriptor0.dstBlendColor = EBlendMode.Zero;
                blendStateDescriptor.attachmentDescriptor0.dstBlendAlpha = EBlendMode.Zero;
                blendStateDescriptor.attachmentDescriptor1 = blendStateDescriptor.attachmentDescriptor0;
                blendStateDescriptor.attachmentDescriptor2 = blendStateDescriptor.attachmentDescriptor0;
                blendStateDescriptor.attachmentDescriptor3 = blendStateDescriptor.attachmentDescriptor0;
                blendStateDescriptor.attachmentDescriptor4 = blendStateDescriptor.attachmentDescriptor0;
                blendStateDescriptor.attachmentDescriptor5 = blendStateDescriptor.attachmentDescriptor0;
                blendStateDescriptor.attachmentDescriptor6 = blendStateDescriptor.attachmentDescriptor0;
                blendStateDescriptor.attachmentDescriptor7 = blendStateDescriptor.attachmentDescriptor0;
            }

            RHIRasterizerStateDescriptor rasterizerStateDescriptor;
            {
                rasterizerStateDescriptor.CullMode = ECullMode.Back;
                rasterizerStateDescriptor.FillMode = EFillMode.Solid;
                rasterizerStateDescriptor.depthBias = 0;
                rasterizerStateDescriptor.depthBiasClamp = 0;
                rasterizerStateDescriptor.slopeScaledDepthBias = 0;
                rasterizerStateDescriptor.scissorEnable = true;
                rasterizerStateDescriptor.depthClipEnable = true;
                rasterizerStateDescriptor.conservativeRaster = false;
                rasterizerStateDescriptor.antialiasedLineEnable = false;
                rasterizerStateDescriptor.frontCounterClockwise = false;
            }

            RHIDepthStencilStateDescriptor depthStencilStateDescriptor;
            {
                depthStencilStateDescriptor.depthEnable = true;
                depthStencilStateDescriptor.depthWriteMask = true;
                depthStencilStateDescriptor.comparisonMode = EComparisonMode.LessEqual;
                depthStencilStateDescriptor.stencilEnable = true;
                depthStencilStateDescriptor.stencilReadMask = 255;
                depthStencilStateDescriptor.stencilWriteMask = 255;
                depthStencilStateDescriptor.backFaceDescriptor.comparisonMode = EComparisonMode.Always;
                depthStencilStateDescriptor.backFaceDescriptor.stencilPassOp = EStencilOp.Keep;
                depthStencilStateDescriptor.backFaceDescriptor.stencilFailOp = EStencilOp.Keep;
                depthStencilStateDescriptor.backFaceDescriptor.stencilDepthFailOp = EStencilOp.Keep;
                depthStencilStateDescriptor.frontFaceDescriptor.comparisonMode = EComparisonMode.Always;
                depthStencilStateDescriptor.frontFaceDescriptor.stencilPassOp = EStencilOp.Keep;
                depthStencilStateDescriptor.frontFaceDescriptor.stencilFailOp = EStencilOp.Keep;
                depthStencilStateDescriptor.frontFaceDescriptor.stencilDepthFailOp = EStencilOp.Keep;
            }

            RHIRenderStateDescriptor renderStateDescriptor;
            {
                renderStateDescriptor.stencilRef = 0;
                renderStateDescriptor.sampleMask = null;
                renderStateDescriptor.blendFactor = null;
                renderStateDescriptor.blendStateDescriptor = blendStateDescriptor;
                renderStateDescriptor.rasterizerStateDescriptor = rasterizerStateDescriptor;
                renderStateDescriptor.depthStencilStateDescriptor = depthStencilStateDescriptor;
            }

            RHIGraphicsPipelineDescriptor graphicsPipelineDescriptor;
            {
                graphicsPipelineDescriptor.vertexShader = m_VertexShader;
                graphicsPipelineDescriptor.fragmentShader = m_FragmentShader;
                graphicsPipelineDescriptor.pipelineLayout = m_GraphicsPipelineLayout;
                graphicsPipelineDescriptor.outputStateDescriptor = outputStateDescriptor;
                graphicsPipelineDescriptor.renderStateDescriptor = renderStateDescriptor;
                graphicsPipelineDescriptor.vertexStateDescriptor = vertexStateDescriptor;
            }
            m_GraphicsPipeline = renderContext.CreateGraphicsPipeline(graphicsPipelineDescriptor);

            m_ColorAttachmentDescriptors = new RHIColorAttachmentDescriptor[1];
            {
                m_ColorAttachmentDescriptors[0].clearValue = new float4(0.5f, 0.5f, 1, 1);
                m_ColorAttachmentDescriptors[0].loadOp = ELoadOp.Clear;
                m_ColorAttachmentDescriptors[0].storeOp = EStoreOp.Store;
                m_ColorAttachmentDescriptors[0].resolveTarget = null;
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
                    computeEncoder.Dispatch((uint)math.ceil((float)renderContext.ScreenSize.x / 8), (uint)math.ceil((float)renderContext.ScreenSize.y / 8), 1);
                    computeEncoder.PopDebugGroup();
                }

                m_ColorAttachmentDescriptors[0].renderTarget = renderContext.BackBufferView;
                RHIGraphicsPassDescriptor graphicsPassDescriptor = new RHIGraphicsPassDescriptor();
                graphicsPassDescriptor.name = "GraphicsPass";
                graphicsPassDescriptor.shadingRateDescriptor = new RHIShadingRateDescriptor(EShadingRate.Rate4x4);
                graphicsPassDescriptor.colorAttachmentDescriptors = new Memory<RHIColorAttachmentDescriptor>(m_ColorAttachmentDescriptors);
                graphicsPassDescriptor.depthStencilAttachmentDescriptor = null;
                using (graphicsEncoder.BeginScopedPass(graphicsPassDescriptor))
                {
                    graphicsEncoder.SetViewport(new Viewport(0, 0, (uint)renderContext.ScreenSize.x, (uint)renderContext.ScreenSize.y, 0, 1));
                    graphicsEncoder.SetScissorRect(new Rect(0, 0, (uint)renderContext.ScreenSize.x, (uint)renderContext.ScreenSize.y));
                    graphicsEncoder.SetPipelineLayout(m_GraphicsPipelineLayout);
                    graphicsEncoder.PushDebugGroup("DrawTriange");
                    graphicsEncoder.SetPipelineState(m_GraphicsPipeline);
                    graphicsEncoder.SetVertexBuffer(m_VertexBuffer);
                    graphicsEncoder.SetIndexBuffer(m_IndexBuffer, EIndexFormat.UInt16);
                    //graphicsEncoder.Draw(3, 1, 0, 0);
                    graphicsEncoder.DrawIndexed(3, 1, 0, 0, 0);
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
            m_IndexBuffer.Dispose();
            m_VertexBuffer.Dispose();
            m_ComputeTextureView.Dispose();
            m_ComputeTexture.Dispose();
            //m_TextureSampler.Dispose();
            //m_GraphicsUniformView.Dispose();
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