using System;

namespace WindowsPhoneSample.Core.Services
{
    internal interface IService
    {
        void Load();
        void Unload(bool isClosing);
        IObservable<bool> Loaded { get; }
    }
}