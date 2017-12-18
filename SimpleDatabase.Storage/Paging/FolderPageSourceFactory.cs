using System;
using System.IO;

namespace SimpleDatabase.Storage.Paging
{
    public class FolderPageSourceFactory : IPageStorageFactory
    {
        private readonly string _path;

        public FolderPageSourceFactory(string path)
        {
            _path = path;
        }

        public IPageStorage Create(PageSource source)
        {
            var filePath = FilePathToSource(source);

            return new FilePageStorage(source, filePath);
        }

        public string FilePathToSource(PageSource source)
        {
            switch (source)
            {
                case PageSource.Heap heap: return Path.Combine(_path, heap.TableName + ".tbh");
                case PageSource.Index index: return Path.Combine(_path, index.TableName + "_" + index.IndexName + ".idx");

                default: throw new ArgumentOutOfRangeException(nameof(source));
            }
        }
    }
}
