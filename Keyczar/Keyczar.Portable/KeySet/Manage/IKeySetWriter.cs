﻿/*  Copyright 2012 James Tuley (jay+code@tuley.name)
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
using System.Linq;
using System.Text;
using Keyczar.Util;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Keyczar
{
    /// <summary>
    /// Extension methods for IKeySetWriter
    /// </summary>
    public static class StandardKeySetWriterOperations
    {
        /// <summary>
        /// Writes the specified key.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="key">The key.</param>
        /// <param name="version">The version.</param>
        public static async Task Write(this IKeySetWriter writer, Key key, int version)
        {
            await writer.Write(Keyczar.RawStringEncoding.GetBytes(key.ToJson()), version);
        }
    }


    /// <summary>
    /// KeyWriter that always needs to rewrite out it's key data even there is only a meta data change
    /// </summary>
    public interface INonSeparatedMetadataAndKey
    {
    }

    /// <summary>
    /// Interface for mechanisms to store keysets
    /// </summary>
    public interface IKeySetWriter

    {
        /// <summary>
        /// Writes the specified key data.
        /// </summary>
        /// <param name="keyData">The key data.</param>
        /// <param name="version">The version.</param>
        Task Write(byte[] keyData, int version);

        /// <summary>
        /// Writes the specified metadata.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        Task Write(KeyMetadata metadata);

        /// <summary>
        /// Finishes this writing of the key.
        /// </summary>
        /// <returns></returns>
        Task<bool> Finish();
    }
}