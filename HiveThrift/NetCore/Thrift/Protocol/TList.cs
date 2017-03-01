using System;

namespace Thrift.Protocol
{
	public struct TList
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

		public TList(TType elementType, int count)
		{
			this = new TList()
			{
				elementType = elementType,
				count = count
			};
		}
	}
}