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
    public partial class HiveObjectPrivilege : TBase
    {
        private HiveObjectRef _hiveObject;
        private string _principalName;
        private PrincipalType _principalType;
        private PrivilegeGrantInfo _grantInfo;

        public HiveObjectRef HiveObject
        {
            get
            {
                return _hiveObject;
            }
            set
            {
                __isset.hiveObject = true;
                this._hiveObject = value;
            }
        }

        public string PrincipalName
        {
            get
            {
                return _principalName;
            }
            set
            {
                __isset.principalName = true;
                this._principalName = value;
            }
        }

        /// <summary>
        ///
        /// <seealso cref="PrincipalType"/>
        /// </summary>
        public PrincipalType PrincipalType
        {
            get
            {
                return _principalType;
            }
            set
            {
                __isset.principalType = true;
                this._principalType = value;
            }
        }

        public PrivilegeGrantInfo GrantInfo
        {
            get
            {
                return _grantInfo;
            }
            set
            {
                __isset.grantInfo = true;
                this._grantInfo = value;
            }
        }

        public Isset __isset;
#if !SILVERLIGHT

        [Serializable]
#endif
        public struct Isset
        {
            public bool hiveObject;
            public bool principalName;
            public bool principalType;
            public bool grantInfo;
        }

        public HiveObjectPrivilege()
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
                        if (field.Type == TType.Struct)
                        {
                            HiveObject = new HiveObjectRef();
                            HiveObject.Read(iprot);
                        }
                        else
                        {
                            TProtocolUtil.Skip(iprot, field.Type);
                        }
                        break;

                    case 2:
                        if (field.Type == TType.String)
                        {
                            PrincipalName = iprot.ReadString();
                        }
                        else
                        {
                            TProtocolUtil.Skip(iprot, field.Type);
                        }
                        break;

                    case 3:
                        if (field.Type == TType.I32)
                        {
                            PrincipalType = (PrincipalType)iprot.ReadI32();
                        }
                        else
                        {
                            TProtocolUtil.Skip(iprot, field.Type);
                        }
                        break;

                    case 4:
                        if (field.Type == TType.Struct)
                        {
                            GrantInfo = new PrivilegeGrantInfo();
                            GrantInfo.Read(iprot);
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
            TStruct struc = new TStruct("HiveObjectPrivilege");
            oprot.WriteStructBegin(struc);
            TField field = new TField();
            if (HiveObject != null && __isset.hiveObject)
            {
                field.Name = "hiveObject";
                field.Type = TType.Struct;
                field.ID = 1;
                oprot.WriteFieldBegin(field);
                HiveObject.Write(oprot);
                oprot.WriteFieldEnd();
            }
            if (PrincipalName != null && __isset.principalName)
            {
                field.Name = "principalName";
                field.Type = TType.String;
                field.ID = 2;
                oprot.WriteFieldBegin(field);
                oprot.WriteString(PrincipalName);
                oprot.WriteFieldEnd();
            }
            if (__isset.principalType)
            {
                field.Name = "principalType";
                field.Type = TType.I32;
                field.ID = 3;
                oprot.WriteFieldBegin(field);
                oprot.WriteI32((int)PrincipalType);
                oprot.WriteFieldEnd();
            }
            if (GrantInfo != null && __isset.grantInfo)
            {
                field.Name = "grantInfo";
                field.Type = TType.Struct;
                field.ID = 4;
                oprot.WriteFieldBegin(field);
                GrantInfo.Write(oprot);
                oprot.WriteFieldEnd();
            }
            oprot.WriteFieldStop();
            oprot.WriteStructEnd();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("HiveObjectPrivilege(");
            sb.Append("HiveObject: ");
            sb.Append(HiveObject == null ? "<null>" : HiveObject.ToString());
            sb.Append(",PrincipalName: ");
            sb.Append(PrincipalName);
            sb.Append(",PrincipalType: ");
            sb.Append(PrincipalType);
            sb.Append(",GrantInfo: ");
            sb.Append(GrantInfo == null ? "<null>" : GrantInfo.ToString());
            sb.Append(")");
            return sb.ToString();
        }
    }
}