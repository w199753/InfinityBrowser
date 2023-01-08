using System;
using System.Diagnostics;
using Infinity.Collections;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.Windows.Windows;

namespace Infinity.Graphics
{
#pragma warning disable CS8600, CS8602, CA1416
    internal unsafe class Dx12Function : RHIFunction
    {
        public D3D12_SHADER_BYTECODE NativeShaderBytecode
        {
            get
            {
                return m_NativeShaderBytecode;
            }
        }

        private D3D12_SHADER_BYTECODE m_NativeShaderBytecode;

        public Dx12Function(in RHIFunctionDescriptor descriptor)
        {
            m_Descriptor = descriptor;
            m_NativeShaderBytecode = new D3D12_SHADER_BYTECODE(descriptor.ByteCode.ToPointer(), new UIntPtr(descriptor.ByteSize));
        }

        protected override void Release()
        {

        }
    }

    internal struct Dx12FunctionTableEntry
    {
        public string ShaderIdentifier;
        public RHIBindGroup[]? BindGroups;
    };

    internal unsafe class Dx12FunctionTable : RHIFunctionTable
    {
        private Dx12Device m_Dx12Device;
        private ID3D12Resource* m_NativeResource;
        private Dx12FunctionTableEntry m_RayGenerationProgram;
        private TArray<Dx12FunctionTableEntry> m_MissPrograms;
        private TArray<Dx12FunctionTableEntry> m_HitGroupPrograms;

        public Dx12FunctionTable(Dx12Device device)
        {
            m_Dx12Device = device;
            m_MissPrograms = new TArray<Dx12FunctionTableEntry>(2);
            m_HitGroupPrograms = new TArray<Dx12FunctionTableEntry>(8);
        }

        public override void SetRayGenerationProgram(string exportName, RHIBindGroup[]? bindGroups = null)
        {
            m_RayGenerationProgram.BindGroups = bindGroups;
            m_RayGenerationProgram.ShaderIdentifier = exportName;
        }

        public override int AddMissProgram(string exportName, RHIBindGroup[]? bindGroups = null)
        {
            Dx12FunctionTableEntry missEntry;
            missEntry.BindGroups = bindGroups;
            missEntry.ShaderIdentifier = exportName;
            return m_MissPrograms.Add(missEntry);
        }

        public override int AddHitGroupProgram(string exportName, RHIBindGroup[]? bindGroups = null)
        {
            Dx12FunctionTableEntry hitGroupEntry;
            hitGroupEntry.BindGroups = bindGroups;
            hitGroupEntry.ShaderIdentifier = exportName;
            return m_HitGroupPrograms.Add(hitGroupEntry);
        }

        public override void SetMissProgram(in int index, RHIBindGroup[]? bindGroups = null)
        {
            throw new NotImplementedException();
        }

        public override void SetHitGroupProgram(in int index, RHIBindGroup[]? bindGroups = null)
        {
            throw new NotImplementedException();
        }

        public override void ClearMissPrograms()
        {
            m_MissPrograms.Clear();
        }

        public override void ClearHitGroupPrograms()
        {
            m_HitGroupPrograms.Clear();
        }

        public override void Generate(RHIRaytracingPipeline rayTracingPipeline)
        {
            Dx12RaytracingPipeline dx12RaytracingPipeline = rayTracingPipeline as Dx12RaytracingPipeline;

            int entryCount = D3D12.D3D12_SHADER_IDENTIFIER_SIZE_IN_BYTES + sizeof(ulong) + (int)dx12RaytracingPipeline.MaxLocalRootParameters;
            int programCount = 1 + m_MissPrograms.length + m_HitGroupPrograms.length;

            D3D12_RESOURCE_DESC resourceDesc = D3D12_RESOURCE_DESC.Buffer((ulong)(entryCount * programCount), D3D12_RESOURCE_FLAGS.D3D12_RESOURCE_FLAG_NONE);
            D3D12_HEAP_PROPERTIES heapProperties = new D3D12_HEAP_PROPERTIES(D3D12_HEAP_TYPE.D3D12_HEAP_TYPE_UPLOAD, 0, 0);

            ID3D12Resource* dx12Resource;
            bool success = SUCCEEDED(m_Dx12Device.NativeDevice->CreateCommittedResource(&heapProperties, D3D12_HEAP_FLAGS.D3D12_HEAP_FLAG_NONE, &resourceDesc, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_GENERIC_READ, null, __uuidof<ID3D12Resource>(), (void**)&dx12Resource));
            Debug.Assert(success);
            m_NativeResource = dx12Resource;
        }

        public override void Update(RHIRaytracingPipeline rayTracingPipeline)
        {
            Dx12RaytracingPipeline? dx12RaytracingPipeline = rayTracingPipeline as Dx12RaytracingPipeline;
        }

        protected override void Release()
        {
            m_NativeResource->Release();
        }
    }
#pragma warning restore CS8600, CS8602, CA1416
}
