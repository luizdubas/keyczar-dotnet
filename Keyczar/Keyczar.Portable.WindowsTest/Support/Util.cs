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
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace KeyczarTest
{
    public static class Util
    {
        public static string ReplaceDirPrefix(string prefixedDir)
        {
            prefixedDir = prefixedDir.Replace("gen|", Path.Combine("gen-testdata"));
            prefixedDir = prefixedDir.Replace("rem|",
                                              Path.Combine("remote-testdata", "existing-data"));
            return prefixedDir;
        }

        private static string TestDataBaseDir(string baseDir)
        {
            return Path.Combine("..", "..", "..", "TestData", baseDir);
        }

        public static string TestDataPath(string baseDir, string topDir, string subDir = null)
        {
            baseDir = TestDataBaseDir(baseDir);


            if (String.IsNullOrWhiteSpace(topDir))
            {
                return baseDir;
            }

            return String.IsNullOrWhiteSpace(subDir)
                       ? Path.Combine(baseDir, topDir)
                       : Path.Combine(baseDir, subDir, topDir);
        }

        public static async Task<string> ReadFirstLine(string path)
        {
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(path))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadLine();
            }
        }

        public static async Task WriteAllText(string path, string text)
        {
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(path, CreationCollisionOption.ReplaceExisting))
            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine(text);
            }
        }
    }
}