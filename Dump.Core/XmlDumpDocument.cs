using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Dump.Core
{
    public class XmlDumpDocument : IDumpDocument
    {
        public string Name { get; }
        public DumpData[] Data { get; }
        public string Text { get; }

        private readonly XDocument doc;

        public XmlDumpDocument(string path, string documentText, XDocument doc)
        {
            Name = Path.GetFileName(path);
            Text = documentText;
            var elements = from node in doc.DescendantNodes()
                           where node is XElement
                           let element = (XElement)node
                           let attrs = element.Attributes()
                           from attr in attrs
                           let key = BuildKey(attr)
                           let value = attr.Value
                           let info = (IXmlLineInfo)element
                           let line = info.LineNumber
                           orderby key
                           select new DumpData(key, value, line);
            var text = from node in doc.DescendantNodes()
                       where node is XText
                       let t = (XText)node
                       let key = BuildKey(t)
                       let value = t.Value
                       let info = (IXmlLineInfo)t.Parent
                       let line = info.LineNumber
                       orderby key
                       select new DumpData(key, value, line);

            Data = elements.Concat(text)
                .OrderBy(kv => kv.Key.Length)
                .ThenBy(kv => kv.Key)
                .ToArray();
        }

        private string BuildKey(XAttribute attr)
        {
            return $"{JoinSegments(attr.Parent)}@{attr.Name.LocalName}";
        }

        private string BuildKey(XText node)
        {
            return $"{JoinSegments(node.Parent)}/";
        }

        private static List<string> BuildSegments(XElement element)
        {
            List<string> segments = new List<string>();
            XElement current = element;
            while (current != null)
            {
                var count = current.ElementsBeforeSelf().Count();
                if (count > 0)
                {
                    segments.Add($"{current.Name.LocalName}[{count}]");
                }
                else
                {
                    segments.Add(current.Name.LocalName);
                }

                current = current.Parent;
            }
            segments.Reverse();
            return segments;
        }

        private static string JoinSegments(XElement element)
        {
            var s = BuildSegments(element);
            return String.Join("/", s);
        }
    }
}