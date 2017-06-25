using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Dump.Core;
using Dump.ViewModels.Tests.Mocks;
using FluentAssertions;
using Xunit;

namespace Dump.ViewModels.Tests.ViewModels
{
    public class HomeViewModelTests
    {
        public HomeViewModel Subject { get; set; }
        public MockDumpImporter DumpImporter { get; set; }

        public HomeViewModelTests()
        {
            DumpImporter = new MockDumpImporter();
            Subject = new HomeViewModel(DumpImporter);
        }

        [Fact]
        public async Task Test_Uses_Given_Path()
        {
            Subject.Path = Path.Combine("path", "hello");

            await Subject.LoadData.Execute();

            DumpImporter.LoadedPath.Should().Be(Subject.Path);
        }

        [Fact]
        public async Task Test_Loads_Data_From_Given_DumpImporter()
        {
            DumpImporter.Result = new DumpResult();

            await Subject.LoadData.Execute();

            Subject.Data.Should().Be(DumpImporter.Result);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("test", true)]
        public async Task Test_Cannot_Laod_Data_From_Null_Or_Empty_Paths(string path, bool expected)
        {
            Subject.Path = path;

            (await Subject.LoadData.CanExecute.FirstAsync()).Should().Be(expected);
        }

        [Fact]
        public async Task Test_Triggers_LoadPathInteraction_When_LoadPath_Is_Executed()
        {
            using (Subject.LoadPathInteraction.RegisterHandler(e => e.SetOutput("testpath")))
            {
                await Subject.LoadPath.Execute().FirstAsync();

                Subject.Path.Should().Be("testpath");
                DumpImporter.LoadedPath.Should().Be("testpath");
            }
        }

        [Fact]
        public async Task Test_LoadPath_Handles_Null_Returned_Paths()
        {
            using (Subject.LoadPathInteraction.RegisterHandler(e => e.SetOutput(null)))
            {
                await Subject.LoadPath.Execute().FirstAsync();

                Subject.Path.Should().Be("");
                DumpImporter.LoadedPath.Should().BeNull();
            }
        }

        [Fact]
        public async Task Test_Selects_Row_With_Same_Key_As_Search()
        {
            DumpImporter.Result = new DumpResult()
            {
                Documents = new List<IDumpDocument>()
                {
                    new MockDumpDocument()
                    {
                        Name = "test",
                        Data = new []
                        {
                            new DumpData("key", "value"),
                            new DumpData("other", "data"),
                        },
                        Text = "test"
                    }
                }
            };

            await Subject.LoadData.Execute().FirstAsync();

            Subject.Search = "key";

            Subject.SelectedRow.Should().Be(DumpImporter.Result.Documents[0].Data[0]);
        }

        [Theory]
        [InlineData("01234\n67\n9", 1, 0, 5)]
        [InlineData("01234\n67\n9", 2, 6, 8)]
        public void Test_GetSelectionForData_Returns_Correct_Start_And_End_Indexes(string text, int line, int start, int end)
        {
            var data = new DumpData("a", "b", line);

            var result = Subject.GetSelectionForData(data, text);

            result.start.Should().Be(start);
            result.end.Should().Be(end);
        }

        [Fact]
        public void Test_Path_Is_Not_Null_By_Default()
        {
            Subject.Path.Should().NotBeNull();
        }

        [Fact]
        public void Test_Search_Is_Not_Null_By_Default()
        {
            Subject.Search.Should().NotBeNull();
        }
    }
}
