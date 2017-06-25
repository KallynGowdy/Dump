using System;
using System.Threading.Tasks;
using Dump.Core;

namespace Dump.ViewModels.Tests.Mocks
{
    public class MockDumpImporter : IDumpImporter
    {
        public string LoadedPath { get; private set; }
        public DumpResult Result { get; set; }

        public Task<DumpResult> LoadFromFileAsync(string path)
        {
            LoadedPath = path;
            return Task.FromResult(Result);
        }
    }
}