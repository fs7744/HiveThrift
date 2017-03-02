using System;

namespace Thrift.Protocol
{
	public struct TStruct
	{
		private string name;

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

		public TStruct(string name)
		{
			this = new TStruct()
			{
				name = name
			};
		}
	}
}