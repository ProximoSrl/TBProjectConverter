using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace TBProjectConverter
{
    public class ProjectUpdater
    {
        public string ConvertFromFile(string pathToFile)
        {
            var doc = XDocument.Load(pathToFile);
            var namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace("prj", "http://schemas.microsoft.com/developer/msbuild/2003");

            // find
            var globals = doc.XPathSelectElement("/prj:Project/prj:PropertyGroup[@Label=\'Globals\']", namespaceManager);
            var configs = doc.XPathSelectElements("/prj:Project/prj:PropertyGroup[@Label=\'Configuration\']", namespaceManager);
            var propertySheets = doc.XPathSelectElements("/prj:Project/prj:ImportGroup[@Label=\'PropertySheets\']", namespaceManager);
            var includeDirectories = doc.XPathSelectElements("/prj:Project/prj:ItemDefinitionGroup/prj:ClCompile/prj:AdditionalIncludeDirectories", namespaceManager);
            var functionLevelLinking = doc.XPathSelectElements("/prj:Project/prj:ItemDefinitionGroup/prj:ClCompile/prj:FunctionLevelLinking", namespaceManager);
            var wcharsettings = doc.XPathSelectElements("/prj:Project/prj:ItemDefinitionGroup/prj:ClCompile/prj:TreatWChar_tAsBuiltInType", namespaceManager);
            var clcompileoptions = doc.XPathSelectElements("/prj:Project/prj:ItemDefinitionGroup/prj:ClCompile", namespaceManager);
            var additionalDependencies = doc.XPathSelectElements("/prj:Project/prj:ItemDefinitionGroup/prj:Link/prj:AdditionalDependencies", namespaceManager);
            var additionalLibraryDirectories = doc.XPathSelectElements("/prj:Project/prj:ItemDefinitionGroup/prj:Link/prj:AdditionalLibraryDirectories", namespaceManager);

            // update
            globals.Add(new XElement("WindowsTargetPlatformVersion", "8.1"));

            foreach (var configSection in configs)
            {
                configSection.Add(new XElement("PlatformToolset", "v140"));
            }

            foreach (var propertySheet in propertySheets)
            {
                propertySheet.Add(new XElement("Import",
                    new XAttribute("Project", "..\\..\\..\\ERP\\ERP.props"))
                );
            }

            foreach (var include in includeDirectories)
            {
                include.Value = "$(TBOCDev);" + include.Value;
            }

            foreach (var fnlink in functionLevelLinking)
            {
                fnlink.Value = "true";
            }

            foreach (var setting in wcharsettings)
            {
                setting.Value = "true";
            }

            foreach (var clcompile in clcompileoptions)
            {
                clcompile.Add(new XElement("AdditionalOptions", "/Zm180 %(AdditionalOptions)"));
            }

            var exclusions = new[] { "tboledb.lib", "tbgenlib.lib" };

            foreach (var include in additionalDependencies)
            {
                var tokens = include.Value.Split(";")
                    .Where(x => !exclusions.Contains(x.ToLowerInvariant()));

                include.Value = string.Join(";", tokens);
            }

            foreach (var lib in additionalLibraryDirectories)
            {
                lib.Value = lib.Value + ";$(TBOCLib)";
            }

            return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" + doc.ToString()
                    .Replace(" xmlns=\"\"", "")
                    //                    .Replace("></ImportGroup>", ">\r\n  </ImportGroup>")
                    ;
        }
    }
}
