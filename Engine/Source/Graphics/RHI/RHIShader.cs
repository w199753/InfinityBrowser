namespace InfinityEngine.Graphics
{
    public class RHIShader : Disposal
    {
        public string name;

        public RHIShader() : base()
        {

        }
    }

    public class RHIComputeShader : RHIShader
    {
        public RHIComputeShader() : base()
        {

        }
    }

    public enum EGraphicsShaderType
    {
        Vertex = 0,
        Hull = 1,
        Domain = 2,
        Geometry = 3,
        Pixel = 4
    }

    public class RHIGraphicsShader : RHIShader
    {
        public RHIGraphicsShader() : base()
        {

        }
    }

    public enum ERayTraceShaderType
    {
        RayGen = 0,
        RayMiss = 1,
        RayHitGroup = 2
    }

    public class RHIRayTraceShader : RHIShader
    {
        //Intersection, AnyHit, ClosestHit, Miss, RayGeneration
        public RHIRayTraceShader() : base()
        {

        }
    }
}
