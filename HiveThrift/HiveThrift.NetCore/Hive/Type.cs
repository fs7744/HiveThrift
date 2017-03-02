/**
 * Autogenerated by Thrift Compiler (0.9.1)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Thrift;
using Thrift.Collections;

using Thrift.Protocol;
using Thrift.Transport;

namespace Hive
{
#if !SILVERLIGHT

#endif

    public partial class Type : TBase
    {
        private string _name;
        private string _type1;
        private string _type2;
        private List<FieldSchema> _fields;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                __isset.name = true;
                this._name = value;
            }
        }

        public string Type1
        {
            get
            {
                return _type1;
            }
            set
            {
                __isset.type1 = true;
                this._type1 = value;
            }
        }

        public string Type2
        {
            get
            {
                return _type2;
            }
            set
            {
                __isset.type2 = true;
                this._type2 = value;
            }
        }

        public List<FieldSchema> Fields
        {
            get
            {
                return _fields;
            }
            set
            {
                __isset.fields = true;
                this._fields = value;
            }
        }

        public Isset __isset;
#if !SILVERLIGHT

#endif

        public struct Isset
        {
            public bool name;
            public bool type1;
            public bool type2;
            public bool fields;
        }

        public Type()
        {
        }

        public void Read(TProtocol iprot)
        {
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
                        if (field.Type == TType.String)
                        {
                            Name = iprot.ReadString();
                        }
                        else
                        {
                            TProtocolUtil.Skip(iprot, field.Type);
                        }
                        break;

                    case 2:
                        if (field.Type == TType.String)
                        {
                            Type1 = iprot.ReadString();
                        }
                        else
                        {
                            TProtocolUtil.Skip(iprot, field.Type);
                        }
                        break;

                    case 3:
                        if (field.Type == TType.String)
                        {
                            Type2 = iprot.ReadString();
                        }
                        else
                        {
                            TProtocolUtil.Skip(iprot, field.Type);
                        }
                        break;

                    case 4:
                        if (field.Type == TType.List)
                        {
                            {
                                Fields = new List<FieldSchema>();
                                TList _list0 = iprot.ReadListBegin();
                                for (int _i1 = 0; _i1 < _list0.Count; ++_i1)
                                {
                                    FieldSchema _elem2 = new FieldSchema();
                                    _elem2 = new FieldSchema();
                                    _elem2.Read(iprot);
                                    Fields.Add(_elem2);
                                }
                                iprot.ReadListEnd();
                            }
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
        }

        public void Write(TProtocol oprot)
        {
            TStruct struc = new TStruct("Type");
            oprot.WriteStructBegin(struc);
            TField field = new TField();
            if (Name != null && __isset.name)
            {
                field.Name = "name";
                field.Type = TType.String;
                field.ID = 1;
                oprot.WriteFieldBegin(field);
                oprot.WriteString(Name);
                oprot.WriteFieldEnd();
            }
            if (Type1 != null && __isset.type1)
            {
                field.Name = "type1";
                field.Type = TType.String;
                field.ID = 2;
                oprot.WriteFieldBegin(field);
                oprot.WriteString(Type1);
                oprot.WriteFieldEnd();
            }
            if (Type2 != null && __isset.type2)
            {
                field.Name = "type2";
                field.Type = TType.String;
                field.ID = 3;
                oprot.WriteFieldBegin(field);
                oprot.WriteString(Type2);
                oprot.WriteFieldEnd();
            }
            if (Fields != null && __isset.fields)
            {
                field.Name = "fields";
                field.Type = TType.List;
                field.ID = 4;
                oprot.WriteFieldBegin(field);
                {
                    oprot.WriteListBegin(new TList(TType.Struct, Fields.Count));
                    foreach (FieldSchema _iter3 in Fields)
                    {
                        _iter3.Write(oprot);
                    }
                    oprot.WriteListEnd();
                }
                oprot.WriteFieldEnd();
            }
            oprot.WriteFieldStop();
            oprot.WriteStructEnd();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Type(");
            sb.Append("Name: ");
            sb.Append(Name);
            sb.Append(",Type1: ");
            sb.Append(Type1);
            sb.Append(",Type2: ");
            sb.Append(Type2);
            sb.Append(",Fields: ");
            sb.Append(Fields);
            sb.Append(")");
            return sb.ToString();
        }
    }
}