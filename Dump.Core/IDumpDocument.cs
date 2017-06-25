using System.Collections.Generic;

namespace Dump.Core
{
    public interface IDumpDocument
    {
        string Name { get; }
        DumpData[] Data { get; }
        string Text { get; }
    }
}