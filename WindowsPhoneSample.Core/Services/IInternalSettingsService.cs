using System.Threading.Tasks;

namespace WindowsPhoneSample.Core.Services
{
    internal interface IInternalSettingsService : ISettingsService
    {
        Task SaveAsync();
        Task LoadAsync();

        void Unload(bool isClosing);
        bool IsLoaded { get; }
    }
}