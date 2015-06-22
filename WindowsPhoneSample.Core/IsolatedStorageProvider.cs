using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;

namespace Com.Powa.PowaTag.Web.Cache
{
    internal sealed class IsolatedStorageProvider : IStorageProvider
    {
        public Task<bool> DirectoryExistsAsync(string directoryPath)
        {
            return Task.FromResult(DirectoryExists(directoryPath));
        }

        public bool DirectoryExists(string directoryPath)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                return isoStore.DirectoryExists(directoryPath);
            }
        }

        public Task<bool> FileExistsAsync(string filePath)
        {
            return Task.FromResult(FileExists(filePath));
        }

        public bool FileExists(string filePath)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                return isoStore.FileExists(filePath);
            }
        }

        public Task CreateDirectoryAsync(string directoryPath)
        {
            return Task.Run(() =>
            {
                CreateDirectory(directoryPath);
            });
        }

        public void CreateDirectory(string directoryPath)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                isoStore.CreateDirectory(directoryPath);
            }
        }

        public Task<Stream> CreateFileAsync(string filePath)
        {
            return Task.FromResult(CreateFile(filePath));
        }

        public Stream CreateFile(string filePath)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                return isoStore.CreateFile(filePath);
            }
        }

        public Task<string[]> GetFileNamesAsync(string searchPattern)
        {
            return Task.FromResult(GetFileNames(searchPattern));
        }

        public string[] GetFileNames(string searchPattern)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                return isoStore.GetFileNames(searchPattern);
            }
        }

        public Task DeleteFileAsync(string filePath)
        {
            return Task.Run(() =>
            {
                DeleteFile(filePath);
            });
        }

        public void DeleteFile(string filePath)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                isoStore.DeleteFile(filePath);
            }
        }

        public Stream OpenFile(string path,StorageCreateMode createMode, StorageMode storageMode, StorageShareMode shareMode)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                FileMode mode;
                switch (createMode)
                {
                    case StorageCreateMode.Open:
                        mode = FileMode.Open;
                        break;
                    case StorageCreateMode.Create:
                        mode = FileMode.Create;
                        break;
                    default:
                        mode = FileMode.OpenOrCreate;
                        break;
                }

                FileAccess access;
                switch (storageMode)
                {
                    case StorageMode.Read:
                        access = FileAccess.Read;
                        break;
                    case StorageMode.Write:
                        access = FileAccess.Write;
                        break;
                    default:
                        access = FileAccess.ReadWrite;
                        break;
                }

                FileShare share;
                switch (shareMode)
                {
                    case StorageShareMode.Read:
                        share = FileShare.Read;
                        break;
                    case StorageShareMode.Write:
                        share = FileShare.Write;
                        break;
                    default:
                        share = FileShare.ReadWrite;
                        break;
                }

                return isoStore.OpenFile(path, mode, access, share);
            }
        }
    }
}