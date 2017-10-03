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
            return ConvertFromSource(File.ReadAllText(pathToFile));
        }

        private void EnsureNodeWithAttributeAndValue(IEnumerable<XElement> parents, string node, string attrname, string value)
        {
            foreach (var parent in parents)
            {
                var child = parent.Descendants().SingleOrDefault(x => 
                    x.Name.LocalName == node &&
                    x.Attributes().Any(a=>a.Name.LocalName == attrname && a.Value == value) 
                );

                if (child == null)
                {
                    parent.Add(new XElement(node,
                        new XAttribute(attrname, value))
                    );
                }
            }
        }

        private void EnsureNodeWithValue(IEnumerable<XContainer> parents, string node, string value)
        {
            foreach (var parent in parents)
            {
                EnsureNodeWithValue(parent, node, value);
            }
        }

        private void EnsureNodeWithValue(XContainer parent, string node, string value)
        {
            var child = parent.Descendants().SingleOrDefault(x => x.Name.LocalName == node);

            if (child != null)
            {
                child.Value = value;
            }
            else
            {
                parent.Add(new XElement(node, value));
            }
        }

        public string ConvertFromSource(string source)
        {
            var doc = XDocument.Parse(source);
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

            EnsureNodeWithValue(globals, "WindowsTargetPlatformVersion", "8.1");
            EnsureNodeWithValue(configs, "PlatformToolset", "v140");

            EnsureNodeWithAttributeAndValue(propertySheets, "Import", "Project", "..\\..\\..\\ERP\\ERP.props");


            foreach (var include in includeDirectories)
            {
                if (!include.Value.ToLowerInvariant().Contains("$(tbocdev)"))
                {
                    include.Value = "$(TBOCDev);" + include.Value;
                }
            }

            foreach (var fnlink in functionLevelLinking)
            {
                fnlink.Value = "true";
            }

            foreach (var setting in wcharsettings)
            {
                setting.Value = "true";
            }

            EnsureNodeWithValue(clcompileoptions, "AdditionalOptions", "/Zm180 %(AdditionalOptions)");

            var exclusions = new[] { "tboledb.lib", "tbgenlib.lib" };

            foreach (var include in additionalDependencies)
            {
                var tokens = include.Value.Split(";")
                    .Where(x => !exclusions.Contains(x.ToLowerInvariant()));

                include.Value = string.Join(";", tokens);
            }

            foreach (var lib in additionalLibraryDirectories)
            {
                if (!lib.Value.ToLowerInvariant().Contains("$(tboclib)"))
                {
                    lib.Value = lib.Value + ";$(TBOCLib)";
                }
            }

            return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" + doc.ToString()
                    .Replace(" xmlns=\"\"", "")
                    //                    .Replace("></ImportGroup>", ">\r\n  </ImportGroup>")
                    ;
        }
    }
}
