using System;
using System.IO;
using Andromeda.Core.Validation;

namespace Andromeda.Data.Parsing.Files
{
    public class FileAccessor : IFileAccessor
    {
        public string Name { get; } 
        public string FullPath { get; }
        public byte[] Data { get; }

        public FileAccessor(string filePath)
        {
            var pathValidation = new PathValidation();
            if (!pathValidation.IsValid(filePath)) throw new DirectoryNotFoundException(filePath);
            if (!File.Exists(filePath)) throw new FileNotFoundException(filePath);

            Name = Path.GetFileNameWithoutExtension(filePath);
            FullPath = filePath;
            Data = Load(filePath);
        }

        private byte[] Load(string filePath)
        {
            var fileData = File.ReadAllBytes(filePath);
            if(fileData.Length < 1) throw new InvalidOperationException($"File is empty : {filePath}");
            return fileData;
        }
    }
}
