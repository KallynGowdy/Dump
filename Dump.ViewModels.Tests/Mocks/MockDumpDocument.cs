using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dump.Core;

namespace Dump.ViewModels.Tests.Mocks
{
    public class MockDumpDocument : IDumpDocument
    {
        public string Name { get; set; }
        public DumpData[] Data { get; set; }
        public string Text { get; set; }
    }
}
