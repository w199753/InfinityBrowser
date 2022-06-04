namespace Infinity.Graphics
{
#pragma warning disable CS8602 
    public struct RHIPipelineLayoutCreateInfo
    {
        public int bindGroupCount => bindGroupLayouts.Length;
        public RHIBindGroupLayout[] bindGroupLayouts;
        // TODO pipeline constant
        // uint32 pipelineConstantNum;
        // PipelineConstantLayout pipelineConstants;
    };

    public abstract class RHIPipelineLayout : Disposal
    {

    }
#pragma warning restore CS8602 
}
