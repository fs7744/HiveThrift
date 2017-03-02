using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Thrift.Transport
{
    public class THttpClient : TTransport, IDisposable
    {
        private readonly Uri uri;

        private Stream inputStream;

        private MemoryStream outputStream = new MemoryStream();

        private int connectTimeout = 30000;

        private int readTimeout = 30000;

        private IDictionary<string, string> customHeaders = new Dictionary<string, string>();

        private HttpClient connection = new HttpClient();

        private IWebProxy proxy = new HttpClientHandler().Proxy;

        private bool _IsDisposed;

        public int ConnectTimeout
        {
            set
            {
                this.connectTimeout = value;
            }
        }

        public IDictionary<string, string> CustomHeaders
        {
            get
            {
                return this.customHeaders;
            }
        }

        public override bool IsOpen
        {
            get
            {
                return true;
            }
        }

        public IWebProxy Proxy
        {
            set
            {
                this.proxy = value;
            }
        }

        public int ReadTimeout
        {
            set
            {
                this.readTimeout = value;
            }
        }

        public THttpClient(Uri u)
        {
            this.uri = u;
            this.connection = this.CreateRequest();
        }

        public override void Close()
        {
            if (this.inputStream != null)
            {
                this.inputStream.Dispose();
                this.inputStream = null;
            }
            if (this.outputStream != null)
            {
                this.outputStream.Dispose();
                this.outputStream = null;
            }
        }

        private HttpClient CreateRequest()
        {
            HttpClientHandler version10 = new HttpClientHandler();
            version10.Proxy = this.proxy;
            using (var hc = new HttpClient(version10))
            {
                //HttpMethod.Post, this.uri
                if (this.connectTimeout > 0)
                {
                    hc.Timeout = new TimeSpan(this.connectTimeout);
                }
                //if (this.readTimeout > 0)
                //{
                //    version10.ReadWriteTimeout = this.readTimeout;
                //}
                hc.DefaultRequestHeaders.Add("ContentType", "application/x-thrift");
                hc.DefaultRequestHeaders.Add("Accept", "application/x-thrift");
                hc.DefaultRequestHeaders.Add("UserAgent", "C#/THttpClient");
                foreach (KeyValuePair<string, string> customHeader in this.customHeaders)
                {
                    hc.DefaultRequestHeaders.Add(customHeader.Key, customHeader.Value);
                }
                //version10.Method = "POST";
                //version10.ProtocolVersion = HttpVersion.Version10;
                return hc;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!this._IsDisposed && disposing)
            {
                if (this.inputStream != null)
                {
                    this.inputStream.Dispose();
                }
                if (this.outputStream != null)
                {
                    this.outputStream.Dispose();
                }
            }
            this._IsDisposed = true;
        }

        public override void Flush()
        {
            try
            {
                this.SendRequest();
            }
            finally
            {
                this.outputStream = new MemoryStream();
            }
        }

        public override void Open()
        {
        }

        public override int Read(byte[] buf, int off, int len)
        {
            int num;
            if (this.inputStream == null)
            {
                throw new TTransportException(TTransportException.ExceptionType.NotOpen, "No request has been sent");
            }
            try
            {
                int num1 = this.inputStream.Read(buf, off, len);
                if (num1 == -1)
                {
                    throw new TTransportException(TTransportException.ExceptionType.EndOfFile, "No more data available");
                }
                num = num1;
            }
            catch (IOException oException)
            {
                throw new TTransportException(TTransportException.ExceptionType.Unknown, oException.ToString());
            }
            return num;
        }

        private void SendRequest()
        {
            try
            {
                var length = this.CreateRequest();
                byte[] array = this.outputStream.ToArray();
                using (Stream requestStream = new MemoryStream(array.Length))
                {
                    requestStream.Write(array, 0, (int)array.Length);
                    var sc = new StreamContent(requestStream);
                    var t = length.PostAsync(this.uri, sc);
                    t.Wait();
                    var o = t.Result.Content.ReadAsStreamAsync();
                    o.Wait();
                    this.inputStream = o.Result;
                }
            }
            catch (IOException oException)
            {
                throw new TTransportException(TTransportException.ExceptionType.Unknown, oException.ToString());
            }
            catch (Exception webException)
            {
                throw new TTransportException(TTransportException.ExceptionType.Unknown, string.Concat("Couldn't connect to server: ", webException));
            }
        }

        public override void Write(byte[] buf, int off, int len)
        {
            this.outputStream.Write(buf, off, len);
        }
    }
}