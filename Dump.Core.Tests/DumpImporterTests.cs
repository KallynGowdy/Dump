using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Dump.Core.Tests
{
    public class DumpImporterTests
    {
        public DumpImporter Subject { get; set; }

        public DumpImporterTests()
        {
            Subject = new DumpImporter();
        }

        [Fact]
        public async Task Test_Loads_Arbitrary_Data_From_XML_Files()
        {
            var result = await Subject.LoadFromFileAsync(PathToTestCase("TestLoad.xml"));

            result.Documents.Count.Should().Be(1);

            var document = result.Documents.First();

            document.Name.Should().Be("TestLoad.xml");

            Assert.Collection(document.Data,
                d => d.Key.Should().Be("RootNode@attribute"),
                d => d.Key.Should().Be("RootNode/ChildNode@id"),
                d => d.Key.Should().Be("RootNode/ChildNode/Value/"),
                d => d.Key.Should().Be("RootNode/ChildNode/Value[1]/"));

            Assert.Collection(document.Data,
                d => d.Value.Should().Be("abc"),
                d => d.Value.Should().Be("1"),
                d => d.Value.Should().Be("abcdefg"),
                d => d.Value.Should().Be("xyz"));
        }

        [Fact]
        public async Task Test_Loads_Full_Text_From_File()
        {
            var path = PathToTestCase("TestLoad.xml");
            var result = await Subject.LoadFromFileAsync(path);
            var text = File.ReadAllText(path);

            var document = result.Documents.First();

            document.Text.Should().Be(text);
        }

        [Fact]
        public async Task Test_Loads_Element_Positions()
        {
            var result = await Subject.LoadFromFileAsync(PathToTestCase("TestLoad.xml"));

            result.Documents.Count.Should().Be(1);

            var document = result.Documents.First();

            document.Name.Should().Be("TestLoad.xml");

            Assert.Collection(document.Data,
                d => d.LineNumber.Should().Be(2),
                d => d.LineNumber.Should().Be(3),
                d => d.LineNumber.Should().Be(4),
                d => d.LineNumber.Should().Be(5));
        }

        public string PathToTestCase(string caseName) =>
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "TestCases", "DumpImporter", caseName);
    }
}
