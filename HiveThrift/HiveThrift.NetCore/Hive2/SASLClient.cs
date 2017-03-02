using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thrift.Transport
{
    public class SASLClient : IDisposable
    {
        PlainMechanism _chose_mechanism;

        public SASLClient(string host, PlainMechanism mechanism)
        {
            this.Mechanism = mechanism.Name;
            _chose_mechanism = mechanism;

        }

        public string Mechanism
        {
            get;
            private set;
        }

        public byte[] process(byte[] challenge)
        {
            return _chose_mechanism.process(challenge);
        }

        public void Dispose()
        {
            _chose_mechanism = null;
        }
    }

}