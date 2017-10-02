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
            var expected = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "samples", "updated.vcxproj"));

            var updater = new ProjectUpdater();
            var source = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "samples", "original.vcxproj");

            var result = updater.ConvertFromFile(source);

            Assert.Equal(
                XDocument.Parse(expected).ToString() , 
                XDocument.Parse(result).ToString()
            );
        }
    }
}
