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
using Keyczar.Util;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Windows.Storage;
using Keyczar.Portable.Extensions;

namespace Keyczar
{
    /// <summary>
    /// Writes a keyset using the standard storage format
    /// </summary>
    public class KeySetWriter : IKeySetWriter
    {
        private readonly string _location;
        private readonly bool _overwrite;
        private List<string> _filePaths = new List<string>();
        private List<Exception> _exceptions = new List<Exception>();
        private bool success = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeySetWriter"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        public KeySetWriter(string location, bool overwrite = false)
        {
            _location = location;
            _overwrite = overwrite;
        }

        /// <summary>
        /// Writes the specified key data.
        /// </summary>
        /// <param name="keyData">The key data.</param>
        /// <param name="version">The version.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Catching to throw later"),
         System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage",
             "CA2202:Do not dispose objects multiple times")]
        public async Task Write(byte[] keyData, int version)
        {
            var versionFile = Path.Combine(_location, version.ToString(CultureInfo.InvariantCulture));
            var file = versionFile + ".temp";
            var fileExists = await ApplicationData.Current.LocalFolder.Exists(file);
            if (!_overwrite && fileExists)
            {
                success = false;
                return;
            }
            _filePaths.Add(file);
            try
            {
                using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(file, CreationCollisionOption.ReplaceExisting))
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(keyData);
                }
            }
            catch (Exception ex)
            {
                _exceptions.Add(ex);
                success = false;
            }
        }


        /// <summary>
        /// Writes the specified metadata.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Catching to throw later"),
         System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage",
             "CA2202:Do not dispose objects multiple times")]
        public async Task Write(KeyMetadata metadata)
        {
            var meta_file = Path.Combine(_location, "meta");
            var file = meta_file + ".temp";
            var fileExists = await ApplicationData.Current.LocalFolder.Exists(meta_file);
            if (!_overwrite && fileExists)
            {
                success = false;
                return;
            }
            try
            {
                _filePaths.Add(file);
                using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(file, CreationCollisionOption.ReplaceExisting))
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(metadata.ToJson());
                }
            }
            catch (Exception ex)
            {
                _exceptions.Add(ex);
                success = false;
            }
        }

        /// <summary>
        /// Finishes this writing of the key.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Finish()
        {
            if (success)
            {
                foreach (var path in _filePaths)
                {
                    var newPath = Path.GetFileNameWithoutExtension(path);
                    try
                    {
                        var fileToDelete = await ApplicationData.Current.LocalFolder.GetFileAsync(newPath);
                        await fileToDelete.DeleteAsync();
                    }
                    catch
                    {
                        //File doesn't exist
                    }

                    var fileToRename = await ApplicationData.Current.LocalFolder.GetFileAsync(path);
                    await fileToRename.RenameAsync(newPath);
                }
            }

            if (!success)
            {
                foreach (var path in _filePaths)
                {
                    var fileToDelete = await ApplicationData.Current.LocalFolder.GetFileAsync(path);
                    await fileToDelete.DeleteAsync();
                }
            }

            Exception newEx = null;
            if (_exceptions.Any())
                newEx = new AggregateException(_exceptions);

            _filePaths.Clear();
            _exceptions.Clear();

            if (newEx != null)
                throw newEx;

            return success;
        }
    }
}