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
using System.Runtime.Serialization;
using Thrift.Protocol;
using Thrift.Transport;

namespace Hive2
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class TExecuteStatementReq : TBase
  {
    private Dictionary<string, string> _confOverlay;
    private bool _runAsync;

    public TSessionHandle SessionHandle { get; set; }

    public string Statement { get; set; }

    public Dictionary<string, string> ConfOverlay
    {
      get
      {
        return _confOverlay;
      }
      set
      {
        __isset.confOverlay = true;
        this._confOverlay = value;
      }
    }

    public bool RunAsync
    {
      get
      {
        return _runAsync;
      }
      set
      {
        __isset.runAsync = true;
        this._runAsync = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool confOverlay;
      public bool runAsync;
    }

    public TExecuteStatementReq() {
      this._runAsync = false;
      this.__isset.runAsync = true;
    }

    public TExecuteStatementReq(TSessionHandle sessionHandle, string statement) : this() {
      this.SessionHandle = sessionHandle;
      this.Statement = statement;
    }

    public void Read (TProtocol iprot)
    {
      bool isset_sessionHandle = false;
      bool isset_statement = false;
      TField field;
      iprot.ReadStructBegin();
      while (true)
      {
        field = iprot.ReadFieldBegin();
        if (field.Type == TType.Stop) { 
          break;
        }
        switch (field.ID)
        {
          case 1:
            if (field.Type == TType.Struct) {
              SessionHandle = new TSessionHandle();
              SessionHandle.Read(iprot);
              isset_sessionHandle = true;
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.String) {
              Statement = iprot.ReadString();
              isset_statement = true;
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.Map) {
              {
                ConfOverlay = new Dictionary<string, string>();
                TMap _map81 = iprot.ReadMapBegin();
                for( int _i82 = 0; _i82 < _map81.Count; ++_i82)
                {
                  string _key83;
                  string _val84;
                  _key83 = iprot.ReadString();
                  _val84 = iprot.ReadString();
                  ConfOverlay[_key83] = _val84;
                }
                iprot.ReadMapEnd();
              }
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.Bool) {
              RunAsync = iprot.ReadBool();
            } else { 
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
      if (!isset_sessionHandle)
        throw new TProtocolException(TProtocolException.INVALID_DATA);
      if (!isset_statement)
        throw new TProtocolException(TProtocolException.INVALID_DATA);
    }

    public void Write(TProtocol oprot) {
      TStruct struc = new TStruct("TExecuteStatementReq");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      field.Name = "sessionHandle";
      field.Type = TType.Struct;
      field.ID = 1;
      oprot.WriteFieldBegin(field);
      SessionHandle.Write(oprot);
      oprot.WriteFieldEnd();
      field.Name = "statement";
      field.Type = TType.String;
      field.ID = 2;
      oprot.WriteFieldBegin(field);
      oprot.WriteString(Statement);
      oprot.WriteFieldEnd();
      if (ConfOverlay != null && __isset.confOverlay) {
        field.Name = "confOverlay";
        field.Type = TType.Map;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteMapBegin(new TMap(TType.String, TType.String, ConfOverlay.Count));
          foreach (string _iter85 in ConfOverlay.Keys)
          {
            oprot.WriteString(_iter85);
            oprot.WriteString(ConfOverlay[_iter85]);
          }
          oprot.WriteMapEnd();
        }
        oprot.WriteFieldEnd();
      }
      if (__isset.runAsync) {
        field.Name = "runAsync";
        field.Type = TType.Bool;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteBool(RunAsync);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("TExecuteStatementReq(");
      sb.Append("SessionHandle: ");
      sb.Append(SessionHandle== null ? "<null>" : SessionHandle.ToString());
      sb.Append(",Statement: ");
      sb.Append(Statement);
      sb.Append(",ConfOverlay: ");
      sb.Append(ConfOverlay);
      sb.Append(",RunAsync: ");
      sb.Append(RunAsync);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
