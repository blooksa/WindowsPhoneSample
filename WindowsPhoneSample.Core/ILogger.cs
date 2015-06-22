using System;

namespace WindowsPhoneSample.Core
{
    public interface ILogger
    {
        void Trace(string format, params object[] args);

        void Debug(string format, params object[] args);

        void Message(string format, params object[] args);

        void Warning(string format, params object[] args);

        void Error(string format, params object[] args);

        void Exception(Exception e, string format, params object[] args);
    }
}