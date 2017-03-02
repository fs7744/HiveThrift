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

    
#endif
    public partial class OpenTxnRequest : TBase
    {
        public int Num_txns { get; set; }

        public string User { get; set; }

        public string Hostname { get; set; }

        public OpenTxnRequest()
        {
        }

        public OpenTxnRequest(int num_txns, string user, string hostname)
            : this()
        {
            this.Num_txns = num_txns;
            this.User = user;
            this.Hostname = hostname;
        }

        public void Read(TProtocol iprot)
        {
            bool isset_num_txns = false;
            bool isset_user = false;
            bool isset_hostname = false;
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
                        if (field.Type == TType.I32)
                        {
                            Num_txns = iprot.ReadI32();
                            isset_num_txns = true;
                        }
                        else
                        {
                            TProtocolUtil.Skip(iprot, field.Type);
                        }
                        break;

                    case 2:
                        if (field.Type == TType.String)
                        {
                            User = iprot.ReadString();
                            isset_user = true;
                        }
                        else
                        {
                            TProtocolUtil.Skip(iprot, field.Type);
                        }
                        break;

                    case 3:
                        if (field.Type == TType.String)
                        {
                            Hostname = iprot.ReadString();
                            isset_hostname = true;
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
            if (!isset_num_txns)
                throw new TProtocolException(TProtocolException.INVALID_DATA);
            if (!isset_user)
                throw new TProtocolException(TProtocolException.INVALID_DATA);
            if (!isset_hostname)
                throw new TProtocolException(TProtocolException.INVALID_DATA);
        }

        public void Write(TProtocol oprot)
        {
            TStruct struc = new TStruct("OpenTxnRequest");
            oprot.WriteStructBegin(struc);
            TField field = new TField();
            field.Name = "num_txns";
            field.Type = TType.I32;
            field.ID = 1;
            oprot.WriteFieldBegin(field);
            oprot.WriteI32(Num_txns);
            oprot.WriteFieldEnd();
            field.Name = "user";
            field.Type = TType.String;
            field.ID = 2;
            oprot.WriteFieldBegin(field);
            oprot.WriteString(User);
            oprot.WriteFieldEnd();
            field.Name = "hostname";
            field.Type = TType.String;
            field.ID = 3;
            oprot.WriteFieldBegin(field);
            oprot.WriteString(Hostname);
            oprot.WriteFieldEnd();
            oprot.WriteFieldStop();
            oprot.WriteStructEnd();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("OpenTxnRequest(");
            sb.Append("Num_txns: ");
            sb.Append(Num_txns);
            sb.Append(",User: ");
            sb.Append(User);
            sb.Append(",Hostname: ");
            sb.Append(Hostname);
            sb.Append(")");
            return sb.ToString();
        }
    }
}