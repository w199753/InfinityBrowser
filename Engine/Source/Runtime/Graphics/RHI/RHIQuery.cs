using InfinityEngine.Core.Object;
using System.Runtime.CompilerServices;

namespace InfinityEngine.Graphics.RHI
{
	public enum EQueryType
    {
		Occlusion = 0,
		Statistic = 2,
		CopyTimestamp = 5,
		GenericTimestamp = 1
	}

    public abstract class FRHIQuery : FDisposal
	{
		internal int indexHead;
		internal int indexLast;

		internal FRHIQuery(FRHIQueryContext queryContext) { }
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public abstract int GetResult();
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public abstract float GetResult(in ulong frequency);
	}

	internal abstract class FRHIQueryContext : FDisposal
	{
		public EQueryType queryType;
		public ulong[]  queryData => m_QueryData;

		protected int m_QueryCount;
		protected ulong[] m_QueryData;
		public virtual int countActive => -1;
		public virtual int countInactive => -1;
		public virtual bool IsReady => true;
		public virtual bool IsTimeQuery => false;

		public FRHIQueryContext(FRHIDevice device, in EQueryType queryType, in int queryCount, string name) { }
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public abstract void Submit(FRHICommandContext commandContext);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public abstract void ResolveData();
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal abstract int Allocate();
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal abstract void Free(in int index);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public abstract FRHIQuery GetTemporary(string name = null);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public abstract void ReleaseTemporary(FRHIQuery query);
	}
}
