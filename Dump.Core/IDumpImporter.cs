using System.Threading.Tasks;

namespace Dump.Core
{
    public interface IDumpImporter
    {
        Task<DumpResult> LoadFromFileAsync(string path);
    }
}