using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Andromeda.Binary.Reader;
using Andromeda.Core.Helpers;
using Andromeda.Core.Validation;
using Andromeda.Data.Parsing.D2O;
using Andromeda.Data.Parsing.D2O.Models;
using Andromeda.Data.Parsing.Files;
using Andromeda.IoC;
using Andromeda.Logging;

namespace Andromeda.DataBuilder.D2O
{
    public class D2OManager
    {
        private Logger _logger = ServiceLocator.Container.GetInstance<Logger>();
        private AppConstants _constants = ServiceLocator.Container.GetInstance<AppConstants>();
        private List<D2OClassDefinition> _definitions;
        private List<ID2OFile> _files;

        public void Load(string[] d2OPaths)
        {
            if(_constants.Debug)
                _logger.Log(LogType.Debug, $"Attempting to load {d2OPaths.Length} d2o files...");
            
            var fileValidator = new FileNameValidation(".d2o");
            if (d2OPaths.Any(path => !fileValidator.IsValid(Path.GetFileName(path))))
                throw new InvalidOperationException("file aren't .d2o");

            _files = new List<ID2OFile>(d2OPaths.Length);
            _definitions = new List<D2OClassDefinition>();

            var elapsed = ActionTimer.Bench(() => _parseAllClassDefinitions(d2OPaths));
            _logger.Log(LogType.Info, $"{d2OPaths.Length} .d2o files with {_definitions.Count} class definitions parsed in {elapsed.Milliseconds}ms");
        }

        public void CompileAllModels()
        {
            if (_constants.Debug)
                _logger.Log(LogType.Debug, $"Attempting to compile {_definitions.Count} d2o models to C# class...");

            var elapsedCompiling = ActionTimer.Bench(() =>
            {
                foreach (var file in _files)
                {
                    foreach (var classDef in file.LocalClassDefinitions.Unpacked)
                    {
                        var elapsed = ActionTimer.Bench(() =>
                            new D2OModelCompiler(file.LocalClassDefinitions.Unpacked, classDef).Compile());
                        if (_constants.Debug)
                            _logger.Log(LogType.Debug,
                                $"{classDef.Name}.cs successfully compiled in {elapsed.Milliseconds}ms");
                    }
                }
            });
            _logger.Log(LogType.Info, $"{_definitions.Count} d2o models succcessfully compiled to C# class in {elapsedCompiling.Milliseconds}ms");
        }

        private void _parseAllClassDefinitions(string[] paths)
        {
            foreach (var path in paths)
            {
                var fileAccessor = new FileAccessor(path);
                var reader = ServiceLocator.Container.GetInstance<IReader>(fileAccessor.Data);
                var d2ODefUnpacker = new D2OClassDefinitionUnpacker();
                d2ODefUnpacker.Unpack(reader);

                if (_constants.Debug)
                    _logger.Log(LogType.Debug, Path.GetFileName(path) + 
                        $" unpacked successfully with {d2ODefUnpacker.Unpacked.Count} class definitions" );

                _definitions.AddRange(d2ODefUnpacker.Unpacked);
                _files.Add(new D2OFile(fileAccessor, d2ODefUnpacker));
            }
        }
    }
}
