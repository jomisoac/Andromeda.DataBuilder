using System;
using System.Collections.Generic;
using System.Text;
using Andromeda.Data.Parsing.D2O.Models;
using Andromeda.Data.Parsing.Files;

namespace Andromeda.Data.Parsing.D2O
{
    public class D2OFile : ID2OFile
    {
        private IFileUnpacker<D2OClass> _classValues;
        private bool _areValuesInitialized;

        public IFileAccessor File { get; }
        
        /// <remarks>
        /// Class values must be set after class definitions parsing and reordering (by packageName)
        /// </remarks>
        public IFileUnpacker<D2OClass> ClassValues
        {
            get => _classValues ?? throw new NullReferenceException("Class values haven't been parsed yet");
            set
            {
                if (_areValuesInitialized) return;
                _classValues = value;
                _areValuesInitialized = true;
            }
        }

        public IFileUnpacker<D2OClassDefinition> LocalClassDefinitions { get; }

        public D2OFile(IFileAccessor file, IFileUnpacker<D2OClassDefinition> classDefUnpacker)
        {
            File = file;
            LocalClassDefinitions = classDefUnpacker;
        }
    }
}
