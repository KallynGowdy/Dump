using System.Collections.Generic;

namespace Dump.Core
{
    public interface IDumpDocument
    {
        string Name { get; }
        KeyValuePair<string, string>[] Data { get; }
    }
}