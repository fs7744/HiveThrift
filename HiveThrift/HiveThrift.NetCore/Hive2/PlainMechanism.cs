using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thrift.Transport
{
    public class PlainMechanism
    {
        public string Name { get { return "PLAIN"; } }
        protected string _userName;
        protected string _password;
        private byte _sign = (byte)0x00;
        public PlainMechanism(string userName, string password)
        {
            _userName = userName;
            _password = password;
        }

        public byte[] process(byte[] challenge)
        {
            List<byte> result = new List<byte>();
            result.Add(_sign);
            result.AddRange(Encoding.UTF8.GetBytes(_userName));
            result.Add(_sign);
            result.AddRange(Encoding.UTF8.GetBytes(_password));

            return result.ToArray();
        }
    }
}