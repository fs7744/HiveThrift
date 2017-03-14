using System;

namespace Thrift.Protocol
{
	public struct TField
	{
		private string name;

		private TType type;

		private short id;

		public short ID
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public TType Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		public TField(string name, TType type, short id)
		{
			this = new TField()
			{
				name = name,
				type = type,
				id = id
			};
		}
	}
}