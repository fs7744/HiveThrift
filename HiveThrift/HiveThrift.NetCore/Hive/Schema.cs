/**
 * Autogenerated by Thrift Compiler (0.9.1)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */

using System;
using System.Collections.Generic;
using System.Text;
using Thrift.Protocol;

namespace Hive
{
#if !SILVERLIGHT

    
#endif
    public partial class Schema : TBase
    {
        private List<FieldSchema> _fieldSchemas;
        private Dictionary<string, string> _properties;

        public List<FieldSchema> FieldSchemas
        {
            get
            {
                return _fieldSchemas;
            }
            set
            {
                __isset.fieldSchemas = true;
                this._fieldSchemas = value;
            }
        }

        public Dictionary<string, string> Properties
        {
            get
            {
                return _properties;
            }
            set
            {
                __isset.properties = true;
                this._properties = value;
            }
        }

        public Isset __isset;
#if !SILVERLIGHT

        
#endif
        public struct Isset
        {
            public bool fieldSchemas;
            public bool properties;
        }

        public Schema()
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
                        if (field.Type == TType.List)
                        {
                            {
                                FieldSchemas = new List<FieldSchema>();
                                TList _list130 = iprot.ReadListBegin();
                                for (int _i131 = 0; _i131 < _list130.Count; ++_i131)
                                {
                                    FieldSchema _elem132 = new FieldSchema();
                                    _elem132 = new FieldSchema();
                                    _elem132.Read(iprot);
                                    FieldSchemas.Add(_elem132);
                                }
                                iprot.ReadListEnd();
                            }
                        }
                        else
                        {
                            TProtocolUtil.Skip(iprot, field.Type);
                        }
                        break;

                    case 2:
                        if (field.Type == TType.Map)
                        {
                            {
                                Properties = new Dictionary<string, string>();
                                TMap _map133 = iprot.ReadMapBegin();
                                for (int _i134 = 0; _i134 < _map133.Count; ++_i134)
                                {
                                    string _key135;
                                    string _val136;
                                    _key135 = iprot.ReadString();
                                    _val136 = iprot.ReadString();
                                    Properties[_key135] = _val136;
                                }
                                iprot.ReadMapEnd();
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
            TStruct struc = new TStruct("Schema");
            oprot.WriteStructBegin(struc);
            TField field = new TField();
            if (FieldSchemas != null && __isset.fieldSchemas)
            {
                field.Name = "fieldSchemas";
                field.Type = TType.List;
                field.ID = 1;
                oprot.WriteFieldBegin(field);
                {
                    oprot.WriteListBegin(new TList(TType.Struct, FieldSchemas.Count));
                    foreach (FieldSchema _iter137 in FieldSchemas)
                    {
                        _iter137.Write(oprot);
                    }
                    oprot.WriteListEnd();
                }
                oprot.WriteFieldEnd();
            }
            if (Properties != null && __isset.properties)
            {
                field.Name = "properties";
                field.Type = TType.Map;
                field.ID = 2;
                oprot.WriteFieldBegin(field);
                {
                    oprot.WriteMapBegin(new TMap(TType.String, TType.String, Properties.Count));
                    foreach (string _iter138 in Properties.Keys)
                    {
                        oprot.WriteString(_iter138);
                        oprot.WriteString(Properties[_iter138]);
                    }
                    oprot.WriteMapEnd();
                }
                oprot.WriteFieldEnd();
            }
            oprot.WriteFieldStop();
            oprot.WriteStructEnd();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Schema(");
            sb.Append("FieldSchemas: ");
            sb.Append(FieldSchemas);
            sb.Append(",Properties: ");
            sb.Append(Properties);
            sb.Append(")");
            return sb.ToString();
        }
    }
}