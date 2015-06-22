using System.Collections.Generic;
using System.IO;
using System.Net;

namespace WindowsPhoneSample.Core.Web
{
    public interface IHttpWebResponse
    {
        HttpStatusCode StatusCode { get; }
        Dictionary<string, string> Headers { get; } 
        string StatusDescription { get; }
        Stream GetResponseStream();
    }
}