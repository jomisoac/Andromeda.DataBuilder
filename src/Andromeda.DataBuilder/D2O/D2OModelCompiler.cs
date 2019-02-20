using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Andromeda.Core.Extensions;
using Andromeda.Data.Parsing.D2O.Enums;
using Andromeda.Data.Parsing.D2O.Models;
using Andromeda.Data.Parsing.Helpers;
using Andromeda.DataBuilder.CodeDom;
using Andromeda.IoC;

namespace Andromeda.DataBuilder.D2O
{
    public class D2OModelCompiler
    {
        private IReadOnlyCollection<D2OClassDefinition> _localClassDefinitions;
        private D2OClassDefinition _currentDefinition;
        private AppConstants _constants = ServiceLocator.Container.GetInstance<AppConstants>();

        public D2OModelCompiler(IReadOnlyCollection<D2OClassDefinition> localClassDefinitions, D2OClassDefinition currentDefinition)
        {
            _localClassDefinitions = localClassDefinitions;
            _currentDefinition = currentDefinition;
        }

        public void Compile()
        {
            var classNamespace = IsNotHandledNamespace(_currentDefinition.Name)
                ? GetNotHandledNamespaces(_currentDefinition.Name)
                : GetCleanNamespace(_currentDefinition.PackageName);

            var targetUnit = new CodeCompileUnit();
            var targetClass = new CodeTypeDeclaration(_currentDefinition.Name) { IsClass = true, TypeAttributes = TypeAttributes.Public };
            var targetNamespace = new CodeNamespace(classNamespace);

            foreach (var field in _currentDefinition.Fields)
            {
                var fieldType = (D2OFieldValueType) field.TypeValue;
                CodeMemberField property = default;
                if (fieldType.IsPrimitiveField())
                {
                    property = CodeDomHelper.CreateProperty(field.Name.Capitalize());
                    property.Type = fieldType.GetPrimitivePropertyType();
                    targetClass.Members.Add(property); continue;
                }
                if (field.TypeValue > 0) // field is another object
                {
                    var relative = _localClassDefinitions.First(def => def.ClassIdentifier == field.TypeValue);
                    var propertyName = relative.Name != field.Name.Capitalize() ? field.Name.Capitalize() : field.Name;
                    property = CodeDomHelper.CreateProperty(propertyName, relative.Name);
                    targetClass.Members.Add(property); 
                    if (relative.PackageName != _currentDefinition.PackageName)
                    {
                        var relativeNamespace = IsNotHandledNamespace(relative.Name)
                            ? GetNotHandledNamespaces(relative.Name)
                            : GetCleanNamespace(relative.PackageName);
                        targetNamespace.Imports.Add(new CodeNamespaceImport(relativeNamespace));
                    }
                    continue;
                }

                var innerFieldType = (D2OFieldValueType) field.InnerTypeValue;
                targetNamespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
                switch (fieldType) // field is either a vector or a vector 2d
                {
                    case D2OFieldValueType.Vector when innerFieldType.IsPrimitiveField():
                    {
                        property = CodeDomHelper.CreateProperty(field.Name.Capitalize());
                        property.Type = innerFieldType.GetPrimitiveVectorPropertyType();
                        break;
                    }
                    case D2OFieldValueType.VectorOfVector when innerFieldType.IsPrimitiveField():
                    {
                        property = CodeDomHelper.CreateProperty(field.Name.Capitalize());
                        property.Type = innerFieldType.GetPrimitiveVectorOfVectorPropertyType();
                        break;
                    }
                    case D2OFieldValueType.Vector when !innerFieldType.IsPrimitiveField():
                    {
                        var relative = _localClassDefinitions.First(def => def.ClassIdentifier == field.InnerTypeValue);
                        var propertyName = relative.Name != field.Name.Capitalize() ? field.Name.Capitalize() : field.Name;
                        property = CodeDomHelper.CreateProperty(propertyName);
                        property.Type = relative.Name.GetPrimitiveVectorPropertyType();
                        if (relative.PackageName != _currentDefinition.PackageName)
                        {
                            var relativeNamespace = IsNotHandledNamespace(relative.Name)
                                ? GetNotHandledNamespaces(relative.Name)
                                : GetCleanNamespace(relative.PackageName);
                            targetNamespace.Imports.Add(new CodeNamespaceImport(relativeNamespace));
                        }
                            break;
                    }
                    case D2OFieldValueType.VectorOfVector when !innerFieldType.IsPrimitiveField():
                    {
                        var relative = _localClassDefinitions.First(def => def.ClassIdentifier == field.InnerTypeValue);
                        var propertyName = relative.Name != field.Name.Capitalize()
                            ? field.Name.Capitalize()
                            : field.Name;
                        property = CodeDomHelper.CreateProperty(propertyName);
                        property.Type = relative.Name.GetPrimitiveVectorOfVectorPropertyType();
                        if (relative.PackageName != _currentDefinition.PackageName)
                        {
                            var relativeNamespace = IsNotHandledNamespace(relative.Name)
                                ? GetNotHandledNamespaces(relative.Name)
                                : GetCleanNamespace(relative.PackageName);
                            targetNamespace.Imports.Add(new CodeNamespaceImport(relativeNamespace));
                        }
                            break;
                    }
                }
                targetClass.Members.Add(property);
            }

            targetNamespace.Types.Add(targetClass);
            targetUnit.Namespaces.Add(targetNamespace);
            var filePath = IsNotHandledNamespace(_currentDefinition.Name)
                ? $@".\D2OModels\{GetNotHandledNamespaces(_currentDefinition.Name).Replace(_constants.D2ONamepace + ".", string.Empty).Replace(".", @"\")}\"
                : $@".\D2OModels\{CleanPackageName(_currentDefinition.PackageName).Replace(".", @"\")}\";
            var fileName = $"{_currentDefinition.Name}.cs";

            if(!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            var provider = CodeDomProvider.CreateProvider("CSharp");
            var options = new CodeGeneratorOptions { BracingStyle = "C", BlankLinesBetweenMembers = false };
            using (var sourceWriter = new StreamWriter(filePath + fileName))
                provider.GenerateCodeFromCompileUnit(targetUnit, sourceWriter, options);

            var postProcessed = File.ReadAllText(filePath + fileName).Replace("//;", "");
            File.WriteAllText(filePath + fileName, postProcessed);
        }

        private string GetCleanNamespace(string packageName) => _constants.D2ONamepace + "." + CleanPackageName(packageName);
        private string CleanPackageName(string packageName) => string.Join('.', packageName.Split('.').Skip(4));
        private bool IsNotHandledNamespace(string className) => GetNotHandledNamespaces(className) != string.Empty;
        private string GetNotHandledNamespaces(string className)
        {
            switch (className)
            {
                case "Point":
                case "Rectangle":
                case "TransformData":
                    return $"{_constants.D2ONamepace}.utils";
                case "AmbientSound":
                case "PlaylistSound":
                    return $"{_constants.D2ONamepace}.ambientSounds";
                case "EffectInstance": return $"{_constants.D2ONamepace}.effects";
                case "EffectInstanceDice": return $"{_constants.D2ONamepace}.effects.instances";
                case "QuestObjectiveParameters": return $"{_constants.D2ONamepace}.quest.objectives";
                default:
                    return string.Empty;
            }
        }
        
    }
}
