using System;

namespace WindowsPhoneSample.Core
{
    public static class Contract
    {
        public static void AssertNotNull<T>(T obj, string parameterName) where T : class
        {
            if (obj == null)
            {
                throw new ArgumentNullException(parameterName, "value cannot be null");
            }
        }

        public static void AssertNotNullOrWhitespace(string str, string parameterName)
        {
            if (str == null)
            {
                throw new ArgumentNullException(parameterName, "value cannot be null");
            }
        }
    }
}