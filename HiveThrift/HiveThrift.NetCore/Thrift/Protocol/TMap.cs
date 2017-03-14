using System;

namespace Thrift.Protocol
{
	public struct TMap
	{
		private TType keyType;

		private TType valueType;

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

		public TType KeyType
		{
			get
			{
				return this.keyType;
			}
			set
			{
				this.keyType = value;
			}
		}

		public TType ValueType
		{
			get
			{
				return this.valueType;
			}
			set
			{
				this.valueType = value;
			}
		}

		public TMap(TType keyType, TType valueType, int count)
		{
			this = new TMap()
			{
				keyType = keyType,
				valueType = valueType,
				count = count
			};
		}
	}
}