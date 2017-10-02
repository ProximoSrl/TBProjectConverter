using System;
using System.IO;
using System.Xml.Linq;
using TBProjectConverter;
using Xunit;

namespace PrxmTBUtils.Tests
{
    public class ConversionTests
    {
        [Fact]
        public void should_convert_with_ma_options()
        {
            var rootFolder = Path.GetDirectoryName(typeof(ProjectUpdater).Assembly.CodeBase).Replace("file:\\","");

            var expected = File.ReadAllText(Path.Combine(rootFolder, "samples", "updated.vcxproj"));

            var updater = new ProjectUpdater();

            var source = Path.Combine(rootFolder, "samples", "original.vcxproj");

            var result = updater.ConvertFromFile(source);

            Assert.Equal(
                XDocument.Parse(expected).ToString() , 
                XDocument.Parse(result).ToString()
            );
        }
    }
}
