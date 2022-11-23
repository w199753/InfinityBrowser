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
        ShaderConductorBlob m_ComputeBlob;
        ShaderConductorBlob m_VertexBlob;
        ShaderConductorBlob m_FragmentBlob;

        RHIFunction m_VertexFunction;
        RHIFunction m_FragmentFunction;
        RHIFunction m_ComputeFunction;
        RHIBuffer m_IndexBuffer;
        RHIBuffer m_VertexBuffer;
        RHITexture m_ComputeTexture;
        //RHISampler m_ComputeSampler;
        RHITextureView m_ComputeTextureView;
        RHIBindGroup m_ComputeBindGroup;
        //RHIBindGroup m_GraphicsBindGroup;
        RHIBindGroupLayout m_ComputeBindGroupLayout;
        RHIBindGroupLayout m_GraphicsBindGroupLayout;
        RHIPipelineLayout m_ComputePipelineLayout;
        RHIPipelineLayout m_GraphicsPipelineLayout;
        RHIComputePipeline m_ComputePipelineState;
        RHIGraphicsPipeline m_GraphicsPipelineState;
        RHIColorAttachmentDescriptor[] m_ColorAttachmentDescriptors;

        public HybridRenderPipeline(string pipelineName) : base(pipelineName)
        {
            string computeCode = new string(@"
            [[vk::binding(0, 0)]]
            RWTexture2D<float4> _ResultTexture[1] : register(u0, space0);

            [numthreads(8, 8, 1)]
            void CSMain (uint3 id : SV_DispatchThreadID)
            {
                float2 UV = (id.xy + 0.5) / float2(1600, 900);
                float IDMod7 = saturate(((id.x & 7) / 7) + ((id.y & 7) / 7));
                _ResultTexture[0][id.xy] = float4(id.x & id.y, IDMod7, UV);
            }");

            string graphicsCode = new string(@"
            [[vk::binding(0, 0)]]
            Texture2D _DiffuseTexture[1] : register(t0, space0);

            [[vk::binding(1, 0)]]
            SamplerState _DiffuseSampler[1] : register(s1, space0);

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
	            return input.color + _DiffuseTexture[0].Sample(_DiffuseSampler[0], float2(0, 0));
            }");

            m_ComputeBlob = ShaderCompiler.HLSLToNativeDxil(computeCode, "CSMain", ShaderConductorWrapper.EFunctionStage.Compute);
            m_VertexBlob = ShaderCompiler.HLSLToNativeDxil(graphicsCode, "VSMain", ShaderConductorWrapper.EFunctionStage.Vertex);
            m_FragmentBlob = ShaderCompiler.HLSLToNativeDxil(graphicsCode, "PSMain", ShaderConductorWrapper.EFunctionStage.Fragment);

            //**************************************************************************************************************************************************************************
            string mslCS = ShaderCompiler.HLSLTo(computeCode, "CSMain", ShaderConductorWrapper.EFunctionStage.Compute, ShaderConductorWrapper.EShadingLanguage.Msl_macOS);
            string dxilCS = ShaderCompiler.HLSLTo(computeCode, "CSMain", ShaderConductorWrapper.EFunctionStage.Compute, ShaderConductorWrapper.EShadingLanguage.Dxil);
            string spirvCS = ShaderCompiler.HLSLTo(computeCode, "CSMain", ShaderConductorWrapper.EFunctionStage.Compute, ShaderConductorWrapper.EShadingLanguage.SpirV);
            Console.WriteLine("*************************\n" +
                              "**  Compute_MSL :      **\n" +
                              "*************************\n");
            Console.WriteLine(mslCS);
            Console.WriteLine("*************************\n" +
                              "**  Compute_Dxil :     **\n" +
                              "*************************\n");
            Console.WriteLine(dxilCS);
            Console.WriteLine("*************************\n" +
                              "**  Compute_SpirV :    **\n" +
                              "*************************\n");
            Console.WriteLine(spirvCS);

            //**************************************************************************************************************************************************************************
            string mslVS = ShaderCompiler.HLSLTo(graphicsCode, "VSMain", ShaderConductorWrapper.EFunctionStage.Vertex, ShaderConductorWrapper.EShadingLanguage.Msl_macOS);
            string dxilVS = ShaderCompiler.HLSLTo(graphicsCode, "VSMain", ShaderConductorWrapper.EFunctionStage.Vertex, ShaderConductorWrapper.EShadingLanguage.Dxil);
            string spirvVS = ShaderCompiler.HLSLTo(graphicsCode, "VSMain", ShaderConductorWrapper.EFunctionStage.Vertex, ShaderConductorWrapper.EShadingLanguage.SpirV);
            Console.WriteLine("*************************\n" +
                              "**  Vertex_MSL :       **\n" +
                              "*************************\n");
            Console.WriteLine(mslVS);
            Console.WriteLine("*************************\n" +
                              "**  Vertex_Dxil :      **\n" +
                              "*************************\n");
            Console.WriteLine(dxilVS);
            Console.WriteLine("*************************\n" +
                              "**  Vertex_SpirV :     **\n" +
                              "*************************\n");
            Console.WriteLine(spirvVS);

            //**************************************************************************************************************************************************************************
            string mslPS = ShaderCompiler.HLSLTo(graphicsCode, "PSMain", ShaderConductorWrapper.EFunctionStage.Fragment, ShaderConductorWrapper.EShadingLanguage.Msl_macOS);
            string dxilPS = ShaderCompiler.HLSLTo(graphicsCode, "PSMain", ShaderConductorWrapper.EFunctionStage.Fragment, ShaderConductorWrapper.EShadingLanguage.Dxil);
            string spirvPS = ShaderCompiler.HLSLTo(graphicsCode, "PSMain", ShaderConductorWrapper.EFunctionStage.Fragment, ShaderConductorWrapper.EShadingLanguage.SpirV);
            Console.WriteLine("*************************\n" +
                              "**  Fragment_MSL :     **\n" +
                              "*************************\n");
            Console.WriteLine(mslPS);
            Console.WriteLine("*************************\n" +
                              "**  Fragment_Dxil :    **\n" +
                              "*************************\n");
            Console.WriteLine(dxilPS);
            Console.WriteLine("*************************\n" +
                              "**  Fragment_SpirV :   **\n" +
                              "*************************\n");
            Console.WriteLine(spirvPS);

            //**************************************************************************************************************************************************************************
            Console.WriteLine("*************************\n" +
                              "**  ShaderCompile End  **\n" +
                              "*************************\n");
        }

        public override void Init(RenderContext renderContext)
        {
            Console.WriteLine("Init RenderPipeline");

            // Create OutputTexture
            RHITextureDescriptor textureDescriptor;
            {
                textureDescriptor.Extent = new int3(renderContext.ScreenSize.xy, 1);
                textureDescriptor.Samples = 1;
                textureDescriptor.MipCount = 1;
                textureDescriptor.Format = EPixelFormat.RGBA8_UNorm;
                textureDescriptor.State = ETextureState.Common;
                textureDescriptor.Usage = ETextureUsage.RenderTarget | ETextureUsage.UnorderedAccess;
                textureDescriptor.Dimension = ETextureDimension.Texture2D;
                textureDescriptor.StorageMode = EStorageMode.Default;
            }
            m_ComputeTexture = renderContext.CreateTexture(textureDescriptor);

            RHITextureViewDescriptor outputViewDescriptor;
            {
                outputViewDescriptor.MipCount = textureDescriptor.MipCount;
                outputViewDescriptor.BaseMipLevel = 0;
                outputViewDescriptor.ArrayLayerCount = 1;
                outputViewDescriptor.BaseArrayLayer = 0;
                outputViewDescriptor.Format = EPixelFormat.RGBA8_UNorm;
                outputViewDescriptor.ViewType = ETextureViewType.UnorderedAccess;
                outputViewDescriptor.Dimension = ETextureViewDimension.Texture2D;
            }
            m_ComputeTextureView = m_ComputeTexture.CreateTextureView(outputViewDescriptor);

            // Create ComputeBindGroupLayout
            RHIBindGroupLayoutElement[] computeBindGroupLayoutElements = new RHIBindGroupLayoutElement[1];
            {
                //computeBindGroupLayoutElements[0].Count = 1;
                computeBindGroupLayoutElements[0].BindSlot = 0;
                computeBindGroupLayoutElements[0].BindType = EBindType.StorageTexture;
                computeBindGroupLayoutElements[0].FunctionStage = EFunctionStage.Compute;
            }
            RHIBindGroupLayoutDescriptor computeBindGroupLayoutDescriptor;
            {
                computeBindGroupLayoutDescriptor.Index = 0;
                computeBindGroupLayoutDescriptor.Elements = new Memory<RHIBindGroupLayoutElement>(computeBindGroupLayoutElements);
            }
            m_ComputeBindGroupLayout = renderContext.CreateBindGroupLayout(computeBindGroupLayoutDescriptor);

            // Create ComputeBindGroup
            RHIBindGroupElement[] computeBindGroupElements = new RHIBindGroupElement[1];
            {
                computeBindGroupElements[0].TextureView = m_ComputeTextureView;
            }
            RHIBindGroupDescriptor computeBindGroupDescriptor;
            {
                computeBindGroupDescriptor.Layout = m_ComputeBindGroupLayout;
                computeBindGroupDescriptor.Elements = new Memory<RHIBindGroupElement>(computeBindGroupElements);
            }
            m_ComputeBindGroup = renderContext.CreateBindGroup(computeBindGroupDescriptor);

            // Create ComputePipeline
            RHIFunctionDescriptor computeFunctionDescriptor;
            {
                computeFunctionDescriptor.Size = m_ComputeBlob.Size;
                computeFunctionDescriptor.ByteCode = m_ComputeBlob.Data;
                computeFunctionDescriptor.EntryName = "CSMain";
                computeFunctionDescriptor.FunctionStage = EFunctionStage.Compute;
            }
            m_ComputeFunction = renderContext.CreateFunction(computeFunctionDescriptor);

            RHIPipelineLayoutDescriptor computePipelienLayoutDescriptor;
            {
                computePipelienLayoutDescriptor.BindGroupLayouts = new RHIBindGroupLayout[] { m_ComputeBindGroupLayout };
            }
            m_ComputePipelineLayout = renderContext.CreatePipelineLayout(computePipelienLayoutDescriptor);

            RHIComputePipelineDescriptor computePipelineDescriptor;
            {
                computePipelineDescriptor.ThreadSize = new uint3(8, 8, 1);
                computePipelineDescriptor.ComputeFunction = m_ComputeFunction;
                computePipelineDescriptor.PipelineLayout = m_ComputePipelineLayout;
            }
            m_ComputePipelineState = renderContext.CreateComputePipeline(computePipelineDescriptor);

            // Create Sampler
            //RHISamplerDescriptor samplerDescriptor;
            //m_ComputeSampler = renderContext.CreateSampler(samplerDescriptor);

            // Create UniformBuffer
            /*RHIBufferViewDescriptor bufferViewDescriptor = new RHIBufferViewDescriptor();
            bufferViewDescriptor.Offset = 0;
            bufferViewDescriptor.Count = vertices.Length;
            bufferViewDescriptor.Type = EBufferViewType.UniformBuffer;
            bufferViewDescriptor.Stride = (bufferDescriptor.size + 255) & ~255;
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
                indexBufferDescriptor.Size = indexs.Length * MemoryUtility.SizeOf<ushort>();
                indexBufferDescriptor.State = EBufferState.Common;
                indexBufferDescriptor.Usage = EBufferUsage.IndexBuffer;
                indexBufferDescriptor.StorageMode = EStorageMode.Dynamic;
            }
            m_IndexBuffer = renderContext.CreateBuffer(indexBufferDescriptor);

            IntPtr indexData = m_IndexBuffer.Map(indexBufferDescriptor.Size, 0);
            GCHandle indexsHandle = GCHandle.Alloc(indexs, GCHandleType.Pinned);
            IntPtr indexsPtr = indexsHandle.AddrOfPinnedObject();
            MemoryUtility.MemCpy(indexsPtr.ToPointer(), indexData.ToPointer(), indexBufferDescriptor.Size);
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
                vertexBufferDescriptor.Size = vertices.Length * MemoryUtility.SizeOf<Vertex>();
                vertexBufferDescriptor.State = EBufferState.Common;
                vertexBufferDescriptor.Usage = EBufferUsage.VertexBuffer;
                vertexBufferDescriptor.StorageMode = EStorageMode.Dynamic;
            }
            m_VertexBuffer = renderContext.CreateBuffer(vertexBufferDescriptor);

            IntPtr vertexData = m_VertexBuffer.Map(vertexBufferDescriptor.Size, 0);
            GCHandle verticesHandle = GCHandle.Alloc(vertices, GCHandleType.Pinned);
            IntPtr verticesPtr = verticesHandle.AddrOfPinnedObject();
            MemoryUtility.MemCpy(verticesPtr.ToPointer(), vertexData.ToPointer(), vertexBufferDescriptor.Size);
            verticesHandle.Free();

            RHIOutputAttachmentDescriptor[] outputColorAttachmentDescriptors = new RHIOutputAttachmentDescriptor[1];
            {
                outputColorAttachmentDescriptors[0].Format = EPixelFormat.RGBA8_UNorm;
                outputColorAttachmentDescriptors[0].ResolveMSAA = false;
            }

            RHIOutputAttachmentDescriptor outputDepthAttachmentDescriptor;
            {
                outputDepthAttachmentDescriptor.Format = EPixelFormat.D32_Float;
                outputDepthAttachmentDescriptor.ResolveMSAA = false;
            }

            RHIOutputStateDescriptor outputStateDescriptor;
            {
                outputStateDescriptor.SampleCount = ESampleCount.None;
                outputStateDescriptor.OutputDepthAttachmentDescriptor = outputDepthAttachmentDescriptor;
                outputStateDescriptor.OutputColorAttachmentDescriptors = new Memory<RHIOutputAttachmentDescriptor>(outputColorAttachmentDescriptors);
            }

            RHIVertexElementDescriptor[] vertexElementDescriptors = new RHIVertexElementDescriptor[2];
            {
                vertexElementDescriptors[0].Index = 1;
                vertexElementDescriptors[0].Offset = 0;
                vertexElementDescriptors[0].Type = ESemanticType.Color;
                vertexElementDescriptors[0].Format = ESemanticFormat.Float4;

                vertexElementDescriptors[1].Index = 0;
                vertexElementDescriptors[1].Offset = 16;
                vertexElementDescriptors[1].Type = ESemanticType.Position;
                vertexElementDescriptors[1].Format = ESemanticFormat.Float4;
            }

            RHIVertexLayoutDescriptor[] vertexLayoutDescriptors = new RHIVertexLayoutDescriptor[1];
            {
                vertexLayoutDescriptors[0].Stride = MemoryUtility.SizeOf<Vertex>();
                vertexLayoutDescriptors[0].StepMode = EVertexStepMode.PerVertex;
                vertexLayoutDescriptors[0].VertexElementDescriptors = new Memory<RHIVertexElementDescriptor>(vertexElementDescriptors);
            }

            RHIBlendStateDescriptor blendStateDescriptor;
            {
                blendStateDescriptor.AlphaToCoverage = false;
                blendStateDescriptor.IndependentBlend = false;
                blendStateDescriptor.BlendDescriptor0.BlendEnable = false;
                blendStateDescriptor.BlendDescriptor0.BlendOpColor = EBlendOp.Add;
                blendStateDescriptor.BlendDescriptor0.BlendOpAlpha = EBlendOp.Add;
                blendStateDescriptor.BlendDescriptor0.ColorWriteChannel = EColorWriteChannel.All;
                blendStateDescriptor.BlendDescriptor0.SrcBlendColor = EBlendMode.One;
                blendStateDescriptor.BlendDescriptor0.SrcBlendAlpha = EBlendMode.One;
                blendStateDescriptor.BlendDescriptor0.DstBlendColor = EBlendMode.Zero;
                blendStateDescriptor.BlendDescriptor0.DstBlendAlpha = EBlendMode.Zero;
                blendStateDescriptor.BlendDescriptor1 = blendStateDescriptor.BlendDescriptor0;
                blendStateDescriptor.BlendDescriptor2 = blendStateDescriptor.BlendDescriptor0;
                blendStateDescriptor.BlendDescriptor3 = blendStateDescriptor.BlendDescriptor0;
                blendStateDescriptor.BlendDescriptor4 = blendStateDescriptor.BlendDescriptor0;
                blendStateDescriptor.BlendDescriptor5 = blendStateDescriptor.BlendDescriptor0;
                blendStateDescriptor.BlendDescriptor6 = blendStateDescriptor.BlendDescriptor0;
                blendStateDescriptor.BlendDescriptor7 = blendStateDescriptor.BlendDescriptor0;
            }

            RHIRasterizerStateDescriptor rasterizerStateDescriptor;
            {
                rasterizerStateDescriptor.CullMode = ECullMode.Back;
                rasterizerStateDescriptor.FillMode = EFillMode.Solid;
                rasterizerStateDescriptor.DepthBias = 0;
                rasterizerStateDescriptor.DepthBiasClamp = 0;
                rasterizerStateDescriptor.SlopeScaledDepthBias = 0;
                rasterizerStateDescriptor.DepthClipEnable = true;
                rasterizerStateDescriptor.ConservativeRaster = false;
                rasterizerStateDescriptor.AntialiasedLineEnable = false;
                rasterizerStateDescriptor.FrontCounterClockwise = false;
            }

            RHIDepthStencilStateDescriptor depthStencilStateDescriptor;
            {
                depthStencilStateDescriptor.DepthEnable = true;
                depthStencilStateDescriptor.DepthWriteMask = true;
                depthStencilStateDescriptor.ComparisonMode = EComparisonMode.LessEqual;
                depthStencilStateDescriptor.StencilEnable = false;
                depthStencilStateDescriptor.StencilReference = 5;
                depthStencilStateDescriptor.StencilReadMask = 255;
                depthStencilStateDescriptor.StencilWriteMask = 255;
                depthStencilStateDescriptor.BackFaceDescriptor.ComparisonMode = EComparisonMode.Always;
                depthStencilStateDescriptor.BackFaceDescriptor.StencilPassOp = EStencilOp.Keep;
                depthStencilStateDescriptor.BackFaceDescriptor.StencilFailOp = EStencilOp.Keep;
                depthStencilStateDescriptor.BackFaceDescriptor.StencilDepthFailOp = EStencilOp.Keep;
                depthStencilStateDescriptor.FrontFaceDescriptor.ComparisonMode = EComparisonMode.Always;
                depthStencilStateDescriptor.FrontFaceDescriptor.StencilPassOp = EStencilOp.Keep;
                depthStencilStateDescriptor.FrontFaceDescriptor.StencilFailOp = EStencilOp.Keep;
                depthStencilStateDescriptor.FrontFaceDescriptor.StencilDepthFailOp = EStencilOp.Keep;
            }

            RHIRenderStateDescriptor renderStateDescriptor;
            {
                renderStateDescriptor.SampleMask = null;
                renderStateDescriptor.BlendStateDescriptor = blendStateDescriptor;
                renderStateDescriptor.RasterizerStateDescriptor = rasterizerStateDescriptor;
                renderStateDescriptor.DepthStencilStateDescriptor = depthStencilStateDescriptor;
            }

            // Create GraphicsBindGroupLayout
            RHIBindGroupLayoutElement[] graphicsBindGroupLayoutElements = new RHIBindGroupLayoutElement[2];
            {
                //graphicsBindGroupLayoutElements[0].Count = 1;
                graphicsBindGroupLayoutElements[0].BindSlot = 0;
                graphicsBindGroupLayoutElements[0].BindType = EBindType.Texture;
                graphicsBindGroupLayoutElements[0].FunctionStage = EFunctionStage.Fragment;

                //graphicsBindGroupLayoutElements[1].Count = 1;
                graphicsBindGroupLayoutElements[1].BindSlot = 1;
                graphicsBindGroupLayoutElements[1].BindType = EBindType.Sampler;
                graphicsBindGroupLayoutElements[1].FunctionStage = EFunctionStage.Fragment;
            }
            RHIBindGroupLayoutDescriptor graphicsBindGroupLayoutDescriptor;
            {
                graphicsBindGroupLayoutDescriptor.Index = 0;
                graphicsBindGroupLayoutDescriptor.Elements = new Memory<RHIBindGroupLayoutElement>(graphicsBindGroupLayoutElements);
            }
            m_GraphicsBindGroupLayout = renderContext.CreateBindGroupLayout(graphicsBindGroupLayoutDescriptor);

            // Create GraphicsBindGroup
            /*RHIBindGroupElement[] graphicsBindGroupElements = new RHIBindGroupElement[2];
            {
                graphicsBindGroupElements[0].Sampler = m_ComputeSampler;
                graphicsBindGroupElements[1].TextureView = m_ComputeTextureView;
            }
            RHIBindGroupDescriptor graphicsBindGroupDescriptor = new RHIBindGroupDescriptor();
            {
                graphicsBindGroupDescriptor.Layout = m_GraphicsBindGroupLayout;
                graphicsBindGroupDescriptor.Elements = new Memory<RHIBindGroupElement>(graphicsBindGroupElements);           
            }
            RHIBindGroup m_GraphicsBindGroup = renderContext.CreateBindGroup(graphicsBindGroupDescriptor);*/

            // Create GraphicsPipeline
            RHIFunctionDescriptor vertexFunctionDescriptor;
            {
                vertexFunctionDescriptor.Size = m_VertexBlob.Size;
                vertexFunctionDescriptor.ByteCode = m_VertexBlob.Data;
                vertexFunctionDescriptor.EntryName = "VSMain";
                vertexFunctionDescriptor.FunctionStage = EFunctionStage.Vertex;
            }
            m_VertexFunction = renderContext.CreateFunction(vertexFunctionDescriptor);

            RHIFunctionDescriptor fragmentFunctionDescriptor;
            {
                fragmentFunctionDescriptor.Size = m_FragmentBlob.Size;
                fragmentFunctionDescriptor.ByteCode = m_FragmentBlob.Data;
                fragmentFunctionDescriptor.EntryName = "PSMain";
                fragmentFunctionDescriptor.FunctionStage = EFunctionStage.Fragment;
            }
            m_FragmentFunction = renderContext.CreateFunction(fragmentFunctionDescriptor);

            RHIPipelineLayoutDescriptor graphicsPipelienLayoutDescriptor;
            {
                graphicsPipelienLayoutDescriptor.BindGroupLayouts = new RHIBindGroupLayout[] { m_GraphicsBindGroupLayout };
            }
            m_GraphicsPipelineLayout = renderContext.CreatePipelineLayout(graphicsPipelienLayoutDescriptor);

            RHIGraphicsPipelineDescriptor graphicsPipelineDescriptor;
            {
                graphicsPipelineDescriptor.VertexFunction = m_VertexFunction;
                graphicsPipelineDescriptor.FragmentFunction = m_FragmentFunction;
                graphicsPipelineDescriptor.PipelineLayout = m_GraphicsPipelineLayout;
                graphicsPipelineDescriptor.PrimitiveTopology = EPrimitiveTopology.TriangleList;
                graphicsPipelineDescriptor.OutputStateDescriptor = outputStateDescriptor;
                graphicsPipelineDescriptor.RenderStateDescriptor = renderStateDescriptor;
                graphicsPipelineDescriptor.VertexLayoutDescriptors = new Memory<RHIVertexLayoutDescriptor>(vertexLayoutDescriptors);
            }
            m_GraphicsPipelineState = renderContext.CreateGraphicsPipeline(graphicsPipelineDescriptor);

            m_ColorAttachmentDescriptors = new RHIColorAttachmentDescriptor[1];
            {
                m_ColorAttachmentDescriptors[0].ClearValue = new float4(0.5f, 0.5f, 1, 1);
                m_ColorAttachmentDescriptors[0].LoadOp = ELoadOp.Clear;
                m_ColorAttachmentDescriptors[0].StoreOp = EStoreOp.Store;
                m_ColorAttachmentDescriptors[0].ResolveTarget = null;
            }
        }

        public override void Render(RenderContext renderContext)
        {
            RHICommandBuffer cmdBuffer = renderContext.GetCommandBuffer(ECommandType.Graphics);

            using (cmdBuffer.BeginScoped("FrameRendering"))
            {
                RHIBlitEncoder blitEncoder = cmdBuffer.GetBlitEncoder();
                RHIComputeEncoder computeEncoder = cmdBuffer.GetComputeEncoder();
                RHIGraphicsEncoder graphicsEncoder = cmdBuffer.GetGraphicsEncoder();

                using (blitEncoder.BeginScopedPass("ResourceBarrier"))
                {
                    blitEncoder.ResourceBarrier(RHIBarrier.Transition(m_ComputeTexture, ETextureState.Common, ETextureState.UnorderedAccess));
                    blitEncoder.ResourceBarrier(RHIBarrier.Transition(renderContext.BackBuffer, ETextureState.Present, ETextureState.RenderTarget));
                }

                using (computeEncoder.BeginScopedPass("ComputePass"))
                {
                    computeEncoder.PushDebugGroup("GenereteUV");
                    computeEncoder.SetPipelineLayout(m_ComputePipelineLayout);
                    computeEncoder.SetPipelineState(m_ComputePipelineState);
                    computeEncoder.SetBindGroup(m_ComputeBindGroup);
                    computeEncoder.Dispatch((uint)math.ceil((float)renderContext.ScreenSize.x / 8), (uint)math.ceil((float)renderContext.ScreenSize.y / 8), 1);
                    computeEncoder.PopDebugGroup();
                }

                m_ColorAttachmentDescriptors[0].RenderTarget = renderContext.BackBufferView;
                RHIGraphicsPassDescriptor graphicsPassDescriptor = new RHIGraphicsPassDescriptor();
                graphicsPassDescriptor.Name = "GraphicsPass";
                graphicsPassDescriptor.ShadingRateDescriptor = new RHIShadingRateDescriptor(EShadingRate.Rate4x4);
                graphicsPassDescriptor.ColorAttachmentDescriptors = new Memory<RHIColorAttachmentDescriptor>(m_ColorAttachmentDescriptors);
                graphicsPassDescriptor.DepthStencilAttachmentDescriptor = null;
                using (graphicsEncoder.BeginScopedPass(graphicsPassDescriptor))
                {
                    graphicsEncoder.SetViewport(new Viewport(0, 0, (uint)renderContext.ScreenSize.x, (uint)renderContext.ScreenSize.y, 0, 1));
                    graphicsEncoder.SetScissorRect(new Rect(0, 0, (uint)renderContext.ScreenSize.x, (uint)renderContext.ScreenSize.y));
                    graphicsEncoder.PushDebugGroup("DrawTriange");
                    graphicsEncoder.SetPipelineLayout(m_GraphicsPipelineLayout);
                    graphicsEncoder.SetPipelineState(m_GraphicsPipelineState);
                    //graphicsEncoder.SetBindGroup(m_GraphicsBindGroup);
                    graphicsEncoder.SetVertexBuffer(m_VertexBuffer);
                    graphicsEncoder.SetIndexBuffer(m_IndexBuffer, EIndexFormat.UInt16);
                    graphicsEncoder.SetBlendFactor(1);
                    graphicsEncoder.DrawIndexed(3, 1, 0, 0, 0);
                    graphicsEncoder.PopDebugGroup();
                }

                using (blitEncoder.BeginScopedPass("ResourceBarrier"))
                {
                    blitEncoder.ResourceBarrier(RHIBarrier.Transition(m_ComputeTexture, ETextureState.UnorderedAccess, ETextureState.Common));
                    blitEncoder.ResourceBarrier(RHIBarrier.Transition(renderContext.BackBuffer, ETextureState.RenderTarget, ETextureState.Present));
                }
            }

            renderContext.ExecuteCommandBuffer(cmdBuffer);
        }

        protected override void Release()
        {
            m_ComputeBlob.Dispose();
            m_VertexBlob.Dispose();
            m_FragmentBlob.Dispose();
            m_IndexBuffer?.Dispose();
            m_VertexBuffer?.Dispose();
            //m_ComputeSampler?.Dispose();
            m_ComputeTexture?.Dispose();
            m_ComputeTextureView?.Dispose();
            m_ComputeFunction?.Dispose();
            m_VertexFunction?.Dispose();
            m_FragmentFunction?.Dispose();
            m_ComputeBindGroup?.Dispose();
            m_ComputeBindGroupLayout?.Dispose();
            m_ComputePipelineState?.Dispose();
            m_ComputePipelineLayout?.Dispose();
            //m_GraphicsBindGroup?.Dispose();
            m_GraphicsBindGroupLayout?.Dispose();
            m_GraphicsPipelineState?.Dispose();
            m_GraphicsPipelineLayout?.Dispose();
            Console.WriteLine("Release RenderPipeline");
        }
    }
}