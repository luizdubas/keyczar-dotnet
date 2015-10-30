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

using System.IO;
using System.Linq;
using Keyczar;
using NUnit.Framework;
using Keyczar.Portable.Exceptions;
using System.Threading.Tasks;
using Windows.Storage;

namespace KeyczarTest.Interop
{
    [TestFixture]
    public class AesNoPrimary : Interop
    {
        public AesNoPrimary(string implementation) : base(implementation)
        {
        }

        [Test]
        public async Task Decrypt()
        {
            var path = TestData("aes-noprimary");
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(Path.Combine(path, "1.out")))
            using (var reader = new StreamReader(stream))
            {
                var activeCiphertext = (WebBase64)reader.ReadLine();

                using (var crypter = new Crypter(path))
                {
                    var activeDecrypted = crypter.Decrypt(activeCiphertext);
                    Expect(activeDecrypted, Is.EqualTo(Input));
                }
            }
        }

        [Test]
        public void Encrypt()
        {
            var path = TestData("aes-noprimary");
            using (var encrypter = new Encrypter(path))
            {
                Expect(() => encrypter.Encrypt(Input), Throws.TypeOf<MissingPrimaryKeyException>());
            }
        }
    }
}