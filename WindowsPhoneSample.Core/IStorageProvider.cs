using System.IO;
using System.Threading.Tasks;
#if WINDOWS_PHONE_APP
using Windows.Storage.Streams;
#endif

namespace WindowsPhoneSample.Core
{
    internal interface IStorageProvider
    {
        bool DirectoryExists(string directoryPath);
        Task<bool> DirectoryExistsAsync(string directoryPath);
        bool FileExists(string filePath);
        Task<bool> FileExistsAsync(string filePath);

        void CreateDirectory(string directoryPath);
        Task CreateDirectoryAsync(string directoryPath);
        Stream CreateFile(string directoryPath);
        Task<Stream> CreateFileAsync(string directoryPath);
        void DeleteFile(string filePath);
        Task DeleteFileAsync(string filePath);
        string[] GetFileNames(string searchPattern);
        Task<string[]> GetFileNamesAsync(string searchPattern);
#if WINDOWS_PHONE_APP
        Task<Stream> OpenFileAsync(string path,StorageCreateMode createMode, StorageMode mode, StorageShareMode shareMode);
#else
        Stream OpenFile(string path,StorageCreateMode createMode, StorageMode mode, StorageShareMode shareMode);
#endif
    }

    internal enum StorageCreateMode
    {
        Open,
        Create,
        OpenOrCreate
    }

    internal enum StorageMode
    {
        Read,
        Write,
        ReadWrite
    }

    internal enum StorageShareMode
    {
        Read,
        Write,
        ReadWrite
    }
}