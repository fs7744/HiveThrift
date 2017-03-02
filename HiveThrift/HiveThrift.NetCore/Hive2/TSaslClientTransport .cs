using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Thrift.Transport
{
    public class TSaslClientTransport : TTransport, IDisposable
    {
        protected SASLClient _sasl;
        protected TSocket _socket;
        protected static int STATUS_BYTES = 1;
        protected static int PAYLOAD_LENGTH_BYTES = 4;
        protected List<byte> statusBytes = new List<byte>() { ((byte)0x01), ((byte)0x02), ((byte)0x03), ((byte)0x04), ((byte)0x05) };
        private bool _IsOpen = false;
        private byte[] _MessageHeader = new byte[STATUS_BYTES + PAYLOAD_LENGTH_BYTES];
        private ByteArrayOutputStream _WriteBuffer = new ByteArrayOutputStream();

        public TSaslClientTransport(TSocket socket, string userName, string password)
        {
            _sasl = new SASLClient(socket.Host, new PlainMechanism(userName, password));
            _socket = socket;
        }

        public void Dispose()
        {
            _socket.Close();
            _socket = null;
            _sasl.Dispose();
        }

        public override void Close()
        {
            _socket.Close();
            _sasl.Dispose();
        }

        public override bool IsOpen
        {
            get { return _IsOpen; }
        }

        public override void Open()
        {
            if (!IsOpen)
            {
                this._socket.Open();
                Send_Sasl_Msg(SaslStatus.START, _sasl.Mechanism);
                Send_Sasl_Msg(SaslStatus.OK, _sasl.process(null));

                while (true)
                {
                    var result = Recv_Sasl_Msg();
                    if (result.Status == SaslStatus.COMPLETE)
                    {
                        _IsOpen = true;
                        break;
                    }
                    else if (result.Status == SaslStatus.OK)
                    {
                        Send_Sasl_Msg(SaslStatus.OK, _sasl.process(Encoding.UTF8.GetBytes(result.Body)));
                    }
                    else
                    {
                        this._socket.Close();
                        throw new Exception(string.Format("Bad SASL negotiation status: {0} ({1})", result.Status, result.Body));
                    }
                }
            }
        }

        public void Send_Sasl_Msg(SaslStatus status, string body)
        {
            Send_Sasl_Msg(status, Encoding.UTF8.GetBytes(body));
        }

        public void Send_Sasl_Msg(SaslStatus status, byte[] body)
        {
            _MessageHeader[0] = statusBytes[(int)status - 1];
            EncodingUtils.encodeBigEndian(body.Length, _MessageHeader, STATUS_BYTES);
            _socket.Write(_MessageHeader);
            _socket.Write(body);
            _socket.Flush();
        }

        public class Sasl_Msg
        {
            public SaslStatus Status { get; set; }
            public string Body { get; set; }
            public Sasl_Msg(SaslStatus status, string body)
            {
                Status = status;
                Body = body;
            }

            public Sasl_Msg()
            {
            }
        }

        public Sasl_Msg Recv_Sasl_Msg()
        {
            Sasl_Msg result = new Sasl_Msg();
            _socket.ReadAll(_MessageHeader, 0, _MessageHeader.Length);
            result.Status = (SaslStatus)(statusBytes.IndexOf(_MessageHeader[0]) + 1);
            byte[] body = new byte[EncodingUtils.decodeBigEndian(_MessageHeader, STATUS_BYTES)];
            _socket.ReadAll(body, 0, body.Length);

            result.Body = Encoding.UTF8.GetString(body);
            return result;
        }

        public int ReadLength()
        {
            byte[] lenBuf = new byte[4];
            _socket.ReadAll(lenBuf, 0, lenBuf.Length);
            return EncodingUtils.decodeBigEndian(lenBuf);
        }

        public void WriteLength(int length)
        {
            byte[] lenBuf = new byte[4];
            EncodingUtils.encodeFrameSize(length, lenBuf);
            _socket.Write(lenBuf);
        }

        TMemoryInputTransport readBuffer = new TMemoryInputTransport();
        public override int Read(byte[] buf, int off, int len)
        {
            int length = readBuffer.Read(buf, off, len);
            if (length > 0)
                return length;

            ReadFrame();
            int i = readBuffer.Read(buf, off, len);
            return i;
        }

        private void ReadFrame()
        {
            int dataLength = ReadLength();
            if (dataLength < 0)
                throw new TTransportException("Read a negative frame size (" + dataLength + ")!");

            byte[] buff = new byte[dataLength];
            _socket.ReadAll(buff, 0, dataLength);
            //string s = Encoding.UTF8.GetString(buff);
            readBuffer.Reset(buff);
        }


        public override void Write(byte[] buf, int off, int len)
        {
            _WriteBuffer.Write(buf, off, len);
        }

        public override void Flush()
        {
            byte[] buff = _WriteBuffer.GetBytes();
            _WriteBuffer.Reset();
            WriteLength(buff.Length);
            _socket.Write(buff, 0, buff.Length);
            _socket.Flush();
        }

        protected override void Dispose(bool disposing)
        {
            Dispose();
        }
    }
}