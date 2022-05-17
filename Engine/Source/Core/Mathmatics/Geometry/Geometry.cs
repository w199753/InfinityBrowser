using System;

namespace InfinityEngine.Mathmatics.Geometry
{
    /*[Serializable]
    public struct FPlane : IEquatable<FPlane>
    {
        private float m_Distance;
        private float3 m_Normal;

        public float distance { get { return m_Distance; } set { m_Distance = value; } }
        public float3 normal { get { return m_Normal; } set { m_Normal = value; } }


        public FPlane(float3 inNormal, float3 inPoint)
        {
            m_Normal = math.normalize(inNormal);
            m_Distance = -math.dot(m_Normal, inPoint);
        }

        public FPlane(float3 inNormal, float d)
        {
            m_Normal = math.normalize(inNormal);
            m_Distance = d;
        }

        public FPlane(float3 a, float3 b, float3 c)
        {
            m_Normal = math.normalize(math.cross(b - a, c - a));
            m_Distance = -math.dot(m_Normal, a);
        }

        public override bool Equals(object other)
        {
            if (!(other is FPlane)) return false;

            return Equals((FPlane)other);
        }

        public bool Equals(FPlane other)
        {
            return distance.Equals(other.distance) && m_Normal.Equals(other.m_Normal);
        }

        public override int GetHashCode()
        {
            return distance.GetHashCode() ^ (m_Normal.GetHashCode() << 2);
        }
    }*/

    [Serializable]
    public struct AABB : IEquatable<AABB>
    {
        private float3 m_Center;
        private float3 m_Extents;

        public float3 center { get { return m_Center; } set { m_Center = value; } }
        public float3 size { get { return m_Extents * 2.0F; } set { m_Extents = value * 0.5F; } }
        public float3 extents { get { return m_Extents; } set { m_Extents = value; } }
        public float3 min { get { return center - extents; } set { SetMinMax(value, max); } }
        public float3 max { get { return center + extents; } set { SetMinMax(min, value); } }


        public AABB(float3 center, float3 size)
        {
            m_Center = center;
            m_Extents = size * 0.5F;
        }

        public override bool Equals(object other)
        {
            if (!(other is AABB)) return false;

            return Equals((AABB)other);
        }

        public bool Equals(AABB other)
        {
            return center.Equals(other.center) && extents.Equals(other.extents);
        }

        public override int GetHashCode()
        {
            return center.GetHashCode() ^ (extents.GetHashCode() << 2);
        }

        public void SetMinMax(float3 min, float3 max)
        {
            extents = (max - min) * 0.5F;
            center = min + extents;
        }
    }

    [Serializable]
    public struct Bound : IEquatable<Bound>
    {
        public float3 center;
        public float3 extents;

        public Bound(float3 Center, float3 Extents)
        {
            center = Center;
            extents = Extents;
        }

        public override bool Equals(object other)
        {
            if (!(other is Bound)) return false;

            return Equals((Bound)other);
        }

        public bool Equals(Bound other)
        {
            return center.Equals(other.center) && extents.Equals(other.extents);
        }

        public override int GetHashCode()
        {
            return center.GetHashCode() ^ (extents.GetHashCode() << 2);
        }

        public static implicit operator Bound(AABB Bound) { return new Bound(Bound.center, Bound.extents); }
        public static implicit operator AABB(Bound Bound) { return new AABB(Bound.center, Bound.extents * 2); }
    }

    [Serializable]
    public struct Sphere : IEquatable<Sphere>
    {
        private float m_Radius;
        private float3 m_Center;

        public float radius { get { return m_Radius; } set { m_Radius = value; } }
        public float3 center { get { return m_Center; } set { m_Center = value; } }


        public Sphere(float radius, float3 center)
        {
            m_Radius = radius;
            m_Center = center;
        }

        public override bool Equals(object other)
        {
            if (!(other is Sphere)) return false;

            return Equals((Sphere)other);
        }

        public bool Equals(Sphere other)
        {
            return radius.Equals(other.radius) && center.Equals(other.center);
        }

        public override int GetHashCode()
        {
            return radius.GetHashCode() ^ (center.GetHashCode() << 2);
        }
    }

    public static class Geometry
    {
        public static float CaculateBoundRadius(in AABB BoundBox)
        {
            float3 Extents = BoundBox.extents;
            return math.max(math.max(math.abs(Extents.x), math.abs(Extents.y)), math.abs(Extents.z));
        }

        public static AABB CaculateWorldBound(in AABB LocalBound, in float4x4 Matrix)
        {
            float4 Center = math.mul(Matrix, new float4(LocalBound.center.x, LocalBound.center.y, LocalBound.center.z, 1));
            float4 Extents = math.abs(Matrix.c0 * LocalBound.extents.x) + math.abs(Matrix.c1 * LocalBound.extents.y) + math.abs(Matrix.c2 * LocalBound.extents.z);

            AABB WorldBound = LocalBound;
            WorldBound.center = Center.xyz;
            WorldBound.extents = Extents.xyz;

            return WorldBound;
        }

        public static bool IntersectAABBFrustum(in AABB bound, Plane[] plane)
        {
            for (int i = 0; i < 6; ++i)
            {
                float3 normal = plane[i].Normal;
                float distance = plane[i].Distance;

                float dist = math.dot(normal, bound.center) + distance;
                float radius = math.dot(bound.extents, math.abs(normal));

                if (dist + radius < 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
