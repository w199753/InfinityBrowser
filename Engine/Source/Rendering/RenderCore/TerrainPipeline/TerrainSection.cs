using System;
using InfinityEngine.Core.Mathmatics;
using InfinityEngine.Core.Mathmatics.Geometry;

namespace InfinityEngine.Rendering
{
    [Serializable]
    public struct SectionLODData
    {
        public int LastLODIndex;
        public float LOD0ScreenSizeSquared;
        public float LOD1ScreenSizeSquared;
        public float LODOnePlusDistributionScalarSquared;
        public float LastLODScreenSizeSquared;
    };

    [Serializable]
    public struct TerrainSection : IComparable<TerrainSection>, IEquatable<TerrainSection>
    {
        public int NumQuad;
        public int LODIndex;
        public float FractionLOD;

        public Bound BoundingBox;
        public float3 PivotPosition;
        public float3 CenterPosition;
        public SectionLODData LODSetting;

        public bool Equals(TerrainSection obj)
        {
            return NumQuad.Equals(obj.NumQuad) && LODIndex.Equals(obj.LODIndex) && FractionLOD.Equals(obj.FractionLOD) && BoundingBox.Equals(obj.BoundingBox) && PivotPosition.Equals(obj.PivotPosition) && CenterPosition.Equals(obj.CenterPosition);
        }

        public override bool Equals(object obj)
        {
            return Equals((TerrainSection)obj);
        }

        public int CompareTo(TerrainSection obj)
        {
            return LODIndex.CompareTo(obj.LODIndex);
        }

        public override int GetHashCode()
        {
            int hashCode = NumQuad;
            hashCode += LODIndex.GetHashCode();
            hashCode += BoundingBox.GetHashCode();
            hashCode += FractionLOD.GetHashCode();
            hashCode += PivotPosition.GetHashCode();
            hashCode += CenterPosition.GetHashCode();
            return hashCode;
        }
    }
}
