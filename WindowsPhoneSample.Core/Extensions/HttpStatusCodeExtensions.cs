using System.Net;

namespace WindowsPhoneSample.Core.Extensions
{
    public static class HttpStatusCodeExtensions
    {
        public static bool ResponseWasOk(this HttpStatusCode code)
        {
            int status = (int)code;
            return status >= 200 && status < 300;
        }

        public static bool ResponseWasRedirected(this HttpStatusCode code)
        {
            int status = (int)code;
            return status >= 300 && status < 400;
        }

        public static bool ResponseWasRejected(this HttpStatusCode code)
        {
            int status = (int)code;
            return status >= 400 && status < 500;
        }

        public static bool RequestFailedDueToServerError(this HttpStatusCode code)
        {
            int status = (int)code;
            return status >= 500 && status < 600;
        }
    }
}