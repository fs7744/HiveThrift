using System;

namespace Thrift.Protocol
{
	public struct TMessage
	{
		private string name;

		private TMessageType type;

		private int seqID;

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

		public int SeqID
		{
			get
			{
				return this.seqID;
			}
			set
			{
				this.seqID = value;
			}
		}

		public TMessageType Type
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

		public TMessage(string name, TMessageType type, int seqid)
		{
			this = new TMessage()
			{
				name = name,
				type = type,
				seqID = seqid
			};
		}
	}
}