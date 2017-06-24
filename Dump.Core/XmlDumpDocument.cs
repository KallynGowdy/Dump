﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Dump.Core
{
    public class XmlDumpDocument : IDumpDocument
    {
        public string Name { get; }
        public KeyValuePair<string, string>[] Data { get; }

        private readonly XDocument doc;

        public XmlDumpDocument(string path, XDocument doc)
        {
            Name = Path.GetFileName(path);
            var elements = from node in doc.DescendantNodes()
                           where node is XElement
                           let element = (XElement)node
                           let attrs = element.Attributes()
                           from attr in attrs
                           let key = BuildKey(attr)
                           let value = attr.Value
                           orderby key
                           select new KeyValuePair<string, string>(key, value);
            var text = from node in doc.DescendantNodes()
                       where node is XText
                       let t = (XText)node
                       let key = BuildKey(t)
                       let value = t.Value
                       orderby key
                       select new KeyValuePair<string, string>(key, value);

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
                segments.Add(current.Name.LocalName);
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