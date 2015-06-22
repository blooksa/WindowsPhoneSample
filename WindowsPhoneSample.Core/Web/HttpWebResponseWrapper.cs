using System.Collections.Generic;
using System.IO;
using System.Net;

namespace WindowsPhoneSample.Core.Web
{
    internal sealed class HttpWebResponseWrapper : IHttpWebResponse
    {
        private readonly HttpWebResponse backingField;

        public HttpWebResponseWrapper(HttpWebResponse response)
        {
            backingField = response;
            StatusCode = response.StatusCode;
            StatusDescription = response.StatusDescription;
            Headers = new Dictionary<string, string>();
            foreach (string header in response.Headers.AllKeys)
            {
                Headers.Add(header, response.Headers[header]);
            }
        }

        public Dictionary<string, string> Headers { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
        public string StatusDescription { get; private set; }

        public Stream GetResponseStream()
        {
            return backingField.GetResponseStream();
        }
    }
}