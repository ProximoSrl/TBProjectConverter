using System;
using System.IO;
using System.Xml.Linq;
using TBProjectConverter;
using Xunit;

namespace PrxmTBUtils.Tests
{
    public class ConversionTests
    {
        private readonly string _rootFolder;

        public ConversionTests()
        {
            _rootFolder = Path.GetDirectoryName(typeof(ProjectUpdater).Assembly.CodeBase).Replace("file:\\", "");
        }

        private string GetPathToSampleFile(string name)
        {
            return Path.Combine(_rootFolder, "samples", name);
        }

        [Fact]
        public void should_convert_with_ma_options()
        {
            var expected = File.ReadAllText(GetPathToSampleFile("updated.vcxproj"));
            var source = GetPathToSampleFile("original.vcxproj");

            var updater = new ProjectUpdater();

            var result = updater.ConvertFromFile(source);

            Assert.Equal(
                XDocument.Parse(expected).ToString() , 
                XDocument.Parse(result).ToString()
            );
        }

        [Fact]
        public void conversion_should_be_idempotent()
        {
            var source = GetPathToSampleFile("original.vcxproj");
            var updater = new ProjectUpdater();

            var result = updater.ConvertFromFile(source);
            var result2 = updater.ConvertFromSource(result);

            Assert.Equal(
                XDocument.Parse(result2).ToString(),
                XDocument.Parse(result).ToString()
            );
        }
    }
}
