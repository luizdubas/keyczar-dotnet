/*  Copyright 2012 James Tuley (jay+code@tuley.name)
 * 
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Keyczar
{
    /// <summary>
    /// standard key set
    /// </summary>
    public class KeySet : IKeySet
    {
        private readonly string _location;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeySet"/> class.
        /// </summary>
        /// <param name="keySetLocation">The key set location.</param>
        public KeySet(string keySetLocation)
        {
            _location = keySetLocation;
        }

        /// <summary>
        /// Gets the binary data that the key is stored in.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        public byte[] GetKeyData(int version)
            =>
                Task.Run(() => GetKeyDataAsync(version)).Result;

        private async Task<byte[]> GetKeyDataAsync(int version)
        {
            var path = Path.Combine(_location, version.ToString(CultureInfo.InvariantCulture));
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(path));
            using (var stream = await file.OpenStreamForReadAsync().ConfigureAwait(false))
            {
                byte[] buffer = new byte[stream.Length];
                await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                return buffer;
            }
        }

        /// <summary>
        /// Gets the metadata.
        /// </summary>
        /// <value>The metadata.</value>
        public KeyMetadata Metadata
            =>
                Task.Run(() => GetMetadataAsync()).Result;

        private async Task<KeyMetadata> GetMetadataAsync()
        {
            var path = Path.Combine(_location, "meta");
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(path));
            using (var stream = await file.OpenStreamForReadAsync().ConfigureAwait(false))
            using (var reader = new StreamReader(stream, Keyczar.RawStringEncoding))
            {
                byte[] buffer = new byte[stream.Length];
                var text = await reader.ReadToEndAsync().ConfigureAwait(false);
                return KeyMetadata.Read(text);
            }
        }
    }
}