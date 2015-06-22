using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace WindowsPhoneSample.Core
{
    internal sealed class StorageProvider : IStorageProvider
    {
        public bool DirectoryExists(string directoryPath)
        {
            return DirectoryExistsAsync(directoryPath).Result;
        }

        public async Task<bool> DirectoryExistsAsync(string directoryPath)
        {
            try
            {
                await ApplicationData.Current.LocalFolder.GetFolderAsync(directoryPath);
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        public async Task CreateDirectoryAsync(string directoryPath)
        {
            try
            {
                await ApplicationData.Current.LocalFolder.CreateFolderAsync(directoryPath);
            }
            catch (Exception)
            {
            }
        }

        public bool FileExists(string filePath)
        {
            return FileExistsAsync(filePath).Result;
        }

        public async Task<bool> FileExistsAsync(string filePath)
        {
            try
            {
                await ApplicationData.Current.LocalFolder.GetFileAsync(filePath);
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        public void CreateDirectory(string directoryPath)
        {
            CreateDirectoryAsync(directoryPath).Wait();
        }

        public Stream CreateFile(string directoryPath)
        {
            return CreateFileAsync(directoryPath).Result;
        }

        public async Task<Stream> CreateFileAsync(string filePath)
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(filePath);
            return await file.OpenStreamForWriteAsync();
        }

        public async Task<string[]> GetFileNamesAsync(string searchPattern)
        {
            IReadOnlyList<StorageFile> files = await ApplicationData.Current.LocalFolder.GetFilesAsync(CommonFileQuery.DefaultQuery);
            string prefix = searchPattern;
            if (searchPattern.Contains("*"))
            {
                prefix = prefix.Substring(0, prefix.IndexOf("*", StringComparison.OrdinalIgnoreCase));
            }
            return files.Select(x => x.Name).Where(x => x.StartsWith(prefix)).ToArray();
        }

        public string[] GetFileNames(string searchPattern)
        {
            return GetFileNamesAsync(searchPattern).Result;
        }

        public async Task DeleteFileAsync(string filePath)
        {
            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(filePath);
                await file.DeleteAsync();
            }
            catch (Exception)
            {
            }
        }

        public void DeleteFile(string filePath)
        {
            DeleteFileAsync(filePath).Wait();
        }

        public async Task<Stream> OpenFileAsync(string path, StorageCreateMode createMode, StorageMode storageMode, StorageShareMode shareMode)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;

            if (createMode == StorageCreateMode.Open)
            {
                switch (storageMode)
                {
                    case StorageMode.Read:
                        return await folder.OpenStreamForReadAsync(path);
                    default:
                        return await folder.OpenStreamForWriteAsync(path, CreationCollisionOption.OpenIfExists);
                }
            }

            switch (storageMode)
            {
                case StorageMode.Read:
                    return await folder.OpenStreamForReadAsync(path);
                default:
                    return await folder.OpenStreamForWriteAsync(path, CreationCollisionOption.OpenIfExists);
            }
        }
    }
}