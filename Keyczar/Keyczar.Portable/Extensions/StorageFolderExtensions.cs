using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Keyczar.Portable.Extensions
{
    public static class StorageFolderExtensions
    {
        public static async Task<bool> Exists(this StorageFolder folder, string file)
        {
            try
            {
                await folder.GetItemAsync(file);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
