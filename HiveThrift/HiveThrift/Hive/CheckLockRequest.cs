/**
 * Autogenerated by Thrift Compiler (0.9.1)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */

using System;
using System.Text;
using Thrift.Protocol;

namespace Hive
{
#if !SILVERLIGHT

    [Serializable]
#endif
    public partial class CheckLockRequest : TBase
    {
        public long Lockid { get; set; }

        public CheckLockRequest()
        {
        }

        public CheckLockRequest(long lockid)
            : this()
        {
            this.Lockid = lockid;
        }

        public void Read(TProtocol iprot)
        {
            bool isset_lockid = false;
            TField field;
            iprot.ReadStructBegin();
            while (true)
            {
                field = iprot.ReadFieldBegin();
                if (field.Type == TType.Stop)
                {
                    break;
                }
                switch (field.ID)
                {
                    case 1:
                        if (field.Type == TType.I64)
                        {
                            Lockid = iprot.ReadI64();
                            isset_lockid = true;
                        }
                        else
                        {
                            TProtocolUtil.Skip(iprot, field.Type);
                        }
                        break;

                    default:
                        TProtocolUtil.Skip(iprot, field.Type);
                        break;
                }
                iprot.ReadFieldEnd();
            }
            iprot.ReadStructEnd();
            if (!isset_lockid)
                throw new TProtocolException(TProtocolException.INVALID_DATA);
        }

        public void Write(TProtocol oprot)
        {
            TStruct struc = new TStruct("CheckLockRequest");
            oprot.WriteStructBegin(struc);
            TField field = new TField();
            field.Name = "lockid";
            field.Type = TType.I64;
            field.ID = 1;
            oprot.WriteFieldBegin(field);
            oprot.WriteI64(Lockid);
            oprot.WriteFieldEnd();
            oprot.WriteFieldStop();
            oprot.WriteStructEnd();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("CheckLockRequest(");
            sb.Append("Lockid: ");
            sb.Append(Lockid);
            sb.Append(")");
            return sb.ToString();
        }
    }
}