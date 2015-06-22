using System;
using System.Net;

namespace WindowsPhoneSample.Core.Extensions
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Determines if the exception itself or any of its inner exceptions are of the specified type.
        /// </summary>
        public static bool IsOfTypeOrWraps<T>(this Exception e, out T te) where T : Exception
        {
            T tException = e as T;
            if (tException != null)
            {
                te = tException;
                return true;
            }

            Exception innerException = e.InnerException;
            while (innerException != null)
            {
                T innerTException = innerException as T;
                if (innerTException != null)
                {
                    te = innerTException;
                    return true;
                }
                innerException = innerException.InnerException;
            }

            te = null;
            return false;
        }

        public static HttpStatusCode GetStatus(this WebException we)
        {
            if (we != null)
            {
                HttpWebResponse response = we.Response as HttpWebResponse;
                if (response != null)
                {
                    return response.StatusCode;
                }
            }

            return HttpStatusCode.NotFound;
        }

    }
}