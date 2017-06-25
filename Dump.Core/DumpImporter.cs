using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Dump.Core
{
    public class DumpImporter : IDumpImporter
    {
        public async Task<DumpResult> LoadFromFileAsync(string path)
        {
            using (var stream = new StreamReader(File.OpenRead(path)))
            {
                var text = await stream.ReadToEndAsync();
                var document = XDocument.Parse(text, LoadOptions.SetLineInfo);
                return new DumpResult()
                {
                    Documents = new List<IDumpDocument>()
                    {
                        new XmlDumpDocument(path, text, document)
                    }
                };
            }
        }
    }
}
