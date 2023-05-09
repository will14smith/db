using System;
using System.IO.Abstractions;

namespace SimpleDatabase.Storage.Paging
{
    public class FolderPageSourceFactory : IPageStorageFactory
    {
        private readonly IFileSystem _fileSystem;
        private readonly string _path;

        public FolderPageSourceFactory(IFileSystem fileSystem, string path)
        {
            _fileSystem = fileSystem;
            _path = path;
        }

        public IPageStorage Create(PageSource source)
        {
            var filePath = FilePathToSource(source);

            return new FilePageStorage(_fileSystem, source, filePath);
        }

        public string FilePathToSource(PageSource source)
        {
            switch (source)
            {
                case PageSource.Database: return _fileSystem.Path.Combine(_path, "database");
                case PageSource.Table table: return _fileSystem.Path.Combine(_path, $"{table.TableName}.tbl");
                
                default: throw new ArgumentOutOfRangeException(nameof(source));
            }
        }
    }
}
