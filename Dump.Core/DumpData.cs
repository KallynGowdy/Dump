using System;
using System.Collections.Generic;
using System.Text;

namespace Dump.Core
{
    /// <summary>
    /// Defines a class that represents a single piece of data gleaned from a document.
    /// </summary>
    public class DumpData
    {
        public string Key { get; }
        public string Value { get; }
        public int LineNumber { get; }

        public string KeyAndValue => $"{Key} ({Value})";

        public DumpData(string key, string value, int lineNum = Int32.MinValue)
        {
            Key = key;
            Value = value;
            LineNumber = lineNum;
        }
    }
}
