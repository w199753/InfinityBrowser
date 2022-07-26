using System;
using Infinity.Memory;
using Infinity.Graphics;
using Infinity.Shaderlib;
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
        //RHISampler m_ComputeSampler;
        RHITextureView m_ComputeTextureView;
        RHIBindGroup m_ComputeBindGroup;
        //RHIBindGroup m_GraphicsBindGroup;
        RHIBindGroupLayout m_ComputeBindGroupLayout;
        RHIBindGroupLayout m_GraphicsBindGroupLayout;
        RHIComputePipeline m_ComputePipeline;
        RHIGraphicsPipeline m_GraphicsPipeline;
        RHIColorAttachmentDescriptor[] m_ColorAttachmentDescriptors;

        public HybridRenderPipeline(string pipelineName) : base(pipelineName) 
        {
            string computeCode = new string(@"
            [[vk::binding(2, 1)]]
            RWTexture2D<float4> _ResultTexture : register(u1, space2);

            [numthreads(8, 8, 1)]
            void CSMain (uint3 id : SV_DispatchThreadID)
            {
                float2 UV = (id.xy + 0.5) / float2(1600, 900);
                _ResultTexture[id.xy] = float4(UV, 0, 1);
                //_ResultTexture[id.xy] = float4(id.x & id.y, (id.x & 15) / 15, (id.y & 15) / 15, 0.25);
            }");

            string graphicsCode = new string(
            @"
            [[vk::binding(3, 2)]]
            Texture2D _DiffuseTexture : register(t2, space3);

            [[vk::binding(3, 2)]]
            SamplerState _DiffuseSampler : register(s2, space3);

            struct Attributes
	        {
		        float4 color : COLOR1;
		        float4 vertexOS : POSITION0;
	        };

            struct Varyings
            {
	            float4 color : COLOR1;
	            float4 vertexCS : SV_POSITION;
            };

            Varyings VSMain(Attributes input)
            {
	            Varyings output = (Varyings)0;
	            output.color = input.color;
	            output.vertexCS = input.vertexOS;
	            return output;
            }

            float4 PSMain(Varyings input) : SV_Target
            {
                //return input.color;
	            return input.color + _DiffuseTexture.Sample(_DiffuseSampler, float2(0, 0));
            }");

            m_ComputeResult = Vortice.Dxc.DxcCompiler.Compile(Vortice.Dxc.DxcShaderStage.Compute, computeCode, "CSMain");
            m_ComputeBlob = m_ComputeResult.GetOutput(Vortice.Dxc.DxcOutKind.Object);

            m_VertexResult = Vortice.Dxc.DxcCompiler.Compile(Vortice.Dxc.DxcShaderStage.Vertex, graphicsCode, "VSMain");
            m_VertexBlob = m_VertexResult.GetOutput(Vortice.Dxc.DxcOutKind.Object);

            m_FragmentResult = Vortice.Dxc.DxcCompiler.Compile(Vortice.Dxc.DxcShaderStage.Pixel, graphicsCode, "PSMain");
            m_FragmentBlob = m_FragmentResult.GetOutput(Vortice.Dxc.DxcOutKind.Object);

            string entryName = "PSMain";
            string shaderCode = graphicsCode;
            ShaderConductorWrapper.EShaderStage shaderStage = ShaderConductorWrapper.EShaderStage.Fragment;
            string glsl = ShaderCompiler.HLSLTo(shaderCode, entryName, shaderStage, ShaderConductorWrapper.EShadingLanguage.Glsl);
            string essl = ShaderCompiler.HLSLTo(shaderCode, entryName, shaderStage, ShaderConductorWrapper.EShadingLanguage.Essl);
            string hlsl = ShaderCompiler.HLSLTo(shaderCode, entryName, shaderStage, ShaderConductorWrapper.EShadingLanguage.Hlsl);
            string dxil = ShaderCompiler.HLSLTo(shaderCode, entryName, shaderStage, ShaderConductorWrapper.EShadingLanguage.Dxil);
            string spirv = ShaderCompiler.HLSLTo(shaderCode, entryName, shaderStage, ShaderConductorWrapper.EShadingLanguage.SpirV);
            string msl_ios = ShaderCompiler.HLSLTo(shaderCode, entryName, shaderStage, ShaderConductorWrapper.EShadingLanguage.Msl_iOS);
            string msl_macOS = ShaderCompiler.HLSLTo(shaderCode, entryName, shaderStage, ShaderConductorWrapper.EShadingLanguage.Msl_macOS);
        }

        public override void Init(RenderContext renderContext)
        {
            Console.WriteLine("Init RenderPipeline");

            // Create OutputTexture
            RHITextureDescriptor textureDescriptor;
            {
                textureDescriptor.extent = new int3(renderContext.ScreenSize.xy, 1);
                textureDescriptor.samples = 1;
                textureDescriptor.mipCount = 1;
                textureDescriptor.format = EPixelFormat.RGBA8_UNorm;
                textureDescriptor.state = ETextureState.Common;
                textureDescriptor.usage = ETextureUsage.RenderTarget | ETextureUsage.UnorderedAccess;
                textureDescriptor.dimension = ETextureDimension.Texture2D;
                textureDescriptor.storageMode = EStorageMode.Default;
            }
            m_ComputeTexture = renderContext.CreateTexture(textureDescriptor);

            RHITextureViewDescriptor outputViewDescriptor;
            {
                outputViewDescriptor.mipCount = textureDescriptor.mipCount;
                outputViewDescriptor.baseMipLevel = 0;
                outputViewDescriptor.arrayLayerCount = 1;
                outputViewDescriptor.baseArrayLayer = 0;
                outputViewDescriptor.format = EPixelFormat.RGBA8_UNorm;
                outputViewDescriptor.viewType = ETextureViewType.UnorderedAccessView;
                outputViewDescriptor.dimension = ETextureViewDimension.Texture2D;
            }
            m_ComputeTextureView = m_ComputeTexture.CreateTextureView(outputViewDescriptor);

            // Create ComputeBindGroupLayout
            RHIBindGroupLayoutElement[] computeBindGroupLayoutElements = new RHIBindGroupLayoutElement[1];
            {
                computeBindGroupLayoutElements[0].slot = 1;
                computeBindGroupLayoutElements[0].count = 1;
                computeBindGroupLayoutElements[0].bindType = EBindType.StorageTexture;
                computeBindGroupLayoutElements[0].shaderStage = EShaderStage.Compute;
            }
            RHIBindGroupLayoutDescriptor computeBindGroupLayoutDescriptor;
            {
                computeBindGroupLayoutDescriptor.index = 2;
                computeBindGroupLayoutDescriptor.elements = new Memory<RHIBindGroupLayoutElement>(computeBindGroupLayoutElements);
            }
            m_ComputeBindGroupLayout = renderContext.CreateBindGroupLayout(computeBindGroupLayoutDescriptor);

            // Create ComputeBindGroup
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

            // Create ComputePipeline
            RHIShaderDescriptor computeShaderDescriptor;
            {
                computeShaderDescriptor.size = m_ComputeBlob.BufferSize;
                computeShaderDescriptor.byteCode = m_ComputeBlob.BufferPointer;
                computeShaderDescriptor.entryName = "Main";
                computeShaderDescriptor.shaderStage = EShaderStage.Compute;
            }
            m_ComputeShader = renderContext.CreateShader(computeShaderDescriptor);

            RHIComputePipelineDescriptor computePipelineDescriptor;
            {
                computePipelineDescriptor.threadSize = new uint3(8, 8, 1);
                computePipelineDescriptor.computeShader = m_ComputeShader;
                computePipelineDescriptor.bindGroupLayouts = new RHIBindGroupLayout[] { m_ComputeBindGroupLayout };
            }
            m_ComputePipeline = renderContext.CreateComputePipeline(computePipelineDescriptor);

            // Create Sampler
            //RHISamplerDescriptor samplerDescriptor;
            //m_ComputeSampler = renderContext.CreateSampler(samplerDescriptor);

            // Create UniformBuffer
            /*RHIBufferViewDescriptor bufferViewDescriptor = new RHIBufferViewDescriptor();
            bufferViewDescriptor.offset = 0;
            bufferViewDescriptor.count = vertices.Length;
            bufferViewDescriptor.type = EBufferViewType.UniformBuffer;
            bufferViewDescriptor.stride = (bufferDescriptor.size + 255) & ~255;
            RHIBufferView uniformBufferView = vertexBuffer.CreateBufferView(bufferViewDescriptor);*/

            // Create IndexBuffer
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

            // Create VertexBuffer
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
                vertexAttributeDescriptors[0].index = 1;
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
                blendStateDescriptor.blendDescriptor0.blendEnable = false;
                blendStateDescriptor.blendDescriptor0.blendOpColor = EBlendOp.Add;
                blendStateDescriptor.blendDescriptor0.blendOpAlpha = EBlendOp.Add;
                blendStateDescriptor.blendDescriptor0.colorWriteChannel = EColorWriteChannel.All;
                blendStateDescriptor.blendDescriptor0.srcBlendColor = EBlendMode.One;
                blendStateDescriptor.blendDescriptor0.srcBlendAlpha = EBlendMode.One;
                blendStateDescriptor.blendDescriptor0.dstBlendColor = EBlendMode.Zero;
                blendStateDescriptor.blendDescriptor0.dstBlendAlpha = EBlendMode.Zero;
                blendStateDescriptor.blendDescriptor1 = blendStateDescriptor.blendDescriptor0;
                blendStateDescriptor.blendDescriptor2 = blendStateDescriptor.blendDescriptor0;
                blendStateDescriptor.blendDescriptor3 = blendStateDescriptor.blendDescriptor0;
                blendStateDescriptor.blendDescriptor4 = blendStateDescriptor.blendDescriptor0;
                blendStateDescriptor.blendDescriptor5 = blendStateDescriptor.blendDescriptor0;
                blendStateDescriptor.blendDescriptor6 = blendStateDescriptor.blendDescriptor0;
                blendStateDescriptor.blendDescriptor7 = blendStateDescriptor.blendDescriptor0;
            }

            RHIRasterizerStateDescriptor rasterizerStateDescriptor;
            {
                rasterizerStateDescriptor.CullMode = ECullMode.Back;
                rasterizerStateDescriptor.FillMode = EFillMode.Solid;
                rasterizerStateDescriptor.depthBias = 0;
                rasterizerStateDescriptor.depthBiasClamp = 0;
                rasterizerStateDescriptor.slopeScaledDepthBias = 0;
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
                depthStencilStateDescriptor.stencilEnable = false;
                depthStencilStateDescriptor.stencilReference = 5;
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
                renderStateDescriptor.sampleMask = null;
                renderStateDescriptor.blendStateDescriptor = blendStateDescriptor;
                renderStateDescriptor.rasterizerStateDescriptor = rasterizerStateDescriptor;
                renderStateDescriptor.depthStencilStateDescriptor = depthStencilStateDescriptor;
            }

            // Create GraphicsBindGroupLayout
            RHIBindGroupLayoutElement[] graphicsBindGroupLayoutElements = new RHIBindGroupLayoutElement[2];
            {
                graphicsBindGroupLayoutElements[0].slot = 2;
                graphicsBindGroupLayoutElements[0].count = 1;
                graphicsBindGroupLayoutElements[0].bindType = EBindType.Texture;
                graphicsBindGroupLayoutElements[0].shaderStage = EShaderStage.Fragment;

                graphicsBindGroupLayoutElements[1].slot = 2;
                graphicsBindGroupLayoutElements[1].count = 1;
                graphicsBindGroupLayoutElements[1].bindType = EBindType.Sampler;
                graphicsBindGroupLayoutElements[1].shaderStage = EShaderStage.Fragment;
            }
            RHIBindGroupLayoutDescriptor graphicsBindGroupLayoutDescriptor;
            {
                graphicsBindGroupLayoutDescriptor.index = 3;
                graphicsBindGroupLayoutDescriptor.elements = new Memory<RHIBindGroupLayoutElement>(graphicsBindGroupLayoutElements);
            }
            m_GraphicsBindGroupLayout = renderContext.CreateBindGroupLayout(graphicsBindGroupLayoutDescriptor);

            // Create GraphicsBindGroup
            /*RHIBindGroupElement[] graphicsBindGroupElements = new RHIBindGroupElement[2];
            {
                graphicsBindGroupElements[0].textureView = m_ComputeTextureView;
                graphicsBindGroupElements[1].textureSampler = textureSampler;
            }
            RHIBindGroupDescriptor graphicsBindGroupDescriptor = new RHIBindGroupDescriptor();
            {
                graphicsBindGroupDescriptor.layout = m_GraphicsBindGroupLayout;
                graphicsBindGroupDescriptor.elements = new Memory<RHIBindGroupElement>(graphicsBindGroupElements);           
            }
            RHIBindGroup m_GraphicsBindGroup = renderContext.CreateBindGroup(graphicsBindGroupDescriptor);*/

            // Create GraphicsPipeline
            RHIShaderDescriptor vertexShaderDescriptor;
            {
                vertexShaderDescriptor.size = m_VertexBlob.BufferSize;
                vertexShaderDescriptor.byteCode = m_VertexBlob.BufferPointer;
                vertexShaderDescriptor.entryName = "Vertex";
                vertexShaderDescriptor.shaderStage = EShaderStage.Vertex;
            }
            m_VertexShader = renderContext.CreateShader(vertexShaderDescriptor);

            RHIShaderDescriptor fragmentShaderDescriptor;
            {
                fragmentShaderDescriptor.size = m_FragmentBlob.BufferSize;
                fragmentShaderDescriptor.byteCode = m_FragmentBlob.BufferPointer;
                fragmentShaderDescriptor.entryName = "Fragment";
                fragmentShaderDescriptor.shaderStage = EShaderStage.Fragment;
            }
            m_FragmentShader = renderContext.CreateShader(fragmentShaderDescriptor);

            RHIGraphicsPipelineDescriptor graphicsPipelineDescriptor;
            {
                graphicsPipelineDescriptor.vertexShader = m_VertexShader;
                graphicsPipelineDescriptor.fragmentShader = m_FragmentShader;
                graphicsPipelineDescriptor.bindGroupLayouts = new RHIBindGroupLayout[] { m_GraphicsBindGroupLayout };
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
                    computeEncoder.PushDebugGroup("GenereteUV");
                    computeEncoder.SetPipeline(m_ComputePipeline);
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
                    graphicsEncoder.PushDebugGroup("DrawTriange");
                    graphicsEncoder.SetPipeline(m_GraphicsPipeline);
                    //graphicsEncoder.SetBindGroup(m_GraphicsBindGroup);
                    graphicsEncoder.SetVertexBuffer(m_VertexBuffer);
                    graphicsEncoder.SetIndexBuffer(m_IndexBuffer, EIndexFormat.UInt16);
                    graphicsEncoder.SetBlendFactor(1);
                    graphicsEncoder.DrawIndexed(3, 1, 0, 0, 0);
                    graphicsEncoder.PopDebugGroup();
                }

                using (blitEncoder.BeginScopedPass())
                {
                    blitEncoder.ResourceBarrier(RHIBarrier.Transition(m_ComputeTexture, ETextureState.UnorderedAccess, ETextureState.Common));
                    blitEncoder.ResourceBarrier(RHIBarrier.Transition(renderContext.BackBuffer, ETextureState.RenderTarget, ETextureState.Present));
                }
            }

            renderContext.ExecuteCommandBuffer(cmdBuffer);
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
            //m_ComputeSampler.Dispose();
            //m_GraphicsUniformView.Dispose();
            m_VertexShader.Dispose();
            m_FragmentShader.Dispose();
            m_ComputeShader.Dispose();
            m_GraphicsPipeline.Dispose();
            //m_GraphicsBindGroup.Dispose();
            m_GraphicsBindGroupLayout.Dispose();
            m_ComputePipeline.Dispose();
            m_ComputeBindGroup.Dispose();
            m_ComputeBindGroupLayout.Dispose();
            Console.WriteLine("Release RenderPipeline");
        }
    }
}