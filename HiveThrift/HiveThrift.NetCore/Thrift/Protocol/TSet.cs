using System;

namespace Thrift.Protocol
{
	public struct TSet
	{
		private TType elementType;

		private int count;

		public int Count
		{
			get
			{
				return this.count;
			}
			set
			{
				this.count = value;
			}
		}

		public TType ElementType
		{
			get
			{
				return this.elementType;
			}
			set
			{
				this.elementType = value;
			}
		}

		public TSet(TType elementType, int count)
		{
			this = new TSet()
			{
				elementType = elementType,
				count = count
			};
		}

		public TSet(TList list) : this(list.ElementType, list.Count)
		{
		}
	}
}