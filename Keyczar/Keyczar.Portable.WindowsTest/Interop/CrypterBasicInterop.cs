//
//  Copyright 2013  
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using System;
using NUnit.Framework;
using Keyczar;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KeyczarTest.Interop
{
    [TestFixture]
    public abstract class CrypterBasicInterop : BasicInterop
    {
        protected string Location;

        public CrypterBasicInterop(string imp) : base(imp)
        {
        }


        protected async Task HelperDecryptVariousSizes(string size)
        {
            var path = TestData(Location) + "-size";

            var activeCiphertext = (WebBase64) await Util.ReadFirstLine(Path.Combine(path, size + ".out"));

            using (var crypter = new Crypter(path))
            {
                var activeDecrypted = crypter.Decrypt(activeCiphertext);
                Expect(activeDecrypted, Is.EqualTo(Input));
            }
        }

        [Test]
        public async Task Decrypt()
        {
            var path = TestData(Location);

            var activeCiphertext = (WebBase64) await Util.ReadFirstLine(Path.Combine(path, "1.out"));
            var primaryCiphertext = (WebBase64) await Util.ReadFirstLine(Path.Combine(path, "2.out"));

            using (var crypter = new Crypter(path))
            {
                var activeDecrypted = crypter.Decrypt(activeCiphertext);
                Expect(activeDecrypted, Is.EqualTo(Input));
                var primaryDecrypted = crypter.Decrypt(primaryCiphertext);
                Expect(primaryDecrypted, Is.EqualTo(Input));
            }
        }
    }
}