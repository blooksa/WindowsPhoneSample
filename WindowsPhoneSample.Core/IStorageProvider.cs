// The MIT License (MIT)
// 
// Copyright (c) 2015 Capsor Consulting AB
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
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