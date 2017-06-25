using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dump.Core;

namespace Dump.ViewModels
{
    public class DumpViewModel
    {
        public DumpResult Dump { get; }

        public DumpViewModel(DumpResult dump)
        {
            Dump = dump;
        }
    }
}
