using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Keyczar.Unofficial;
using NUnit.Framework;
using Keyczar;
using System.Threading.Tasks;
using Windows.Storage;

namespace KeyczarTest.Unofficial
{
    [Category("Unofficial")]
    public class BlobKeySetTest : AssertionHelper
    {
        private static string input = "Some test text";

        private static string TEST_DATA = Path.Combine("remote-testdata", "existing-data", "dotnet", "unofficial",
                                                       "blob");


        [Test]
        public async Task TestDecrypt()
        {
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(Util.TestDataPath(TEST_DATA, "cryptkey.zip")))
            using (var keySet = new BlobKeySet(stream))
            using (var crypter = new Crypter(keySet))
            using (var cryptout = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(Util.TestDataPath(TEST_DATA, "crypt.out")))
            {
                var cipherText = (WebBase64)new StreamReader(cryptout).ReadToEnd();
                Expect(crypter.Decrypt(cipherText), Is.EqualTo(input));
            }
        }

        [Test]
        public async Task TestSign()
        {
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(Util.TestDataPath(TEST_DATA, "cryptkey.zip")))
            using (var keySet = new BlobKeySet(stream))
            using (var crypter = new Crypter(keySet))
            using (var signstream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(Util.TestDataPath(TEST_DATA, "signkey.zip")))
            using (var signkeySet = new BlobKeySet(signstream))
            using (var verifier = new Verifier(new EncryptedKeySet(signkeySet, crypter)))
            using (var signoutstream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(Util.TestDataPath(TEST_DATA, "sign.out")))
            {
                var sig = (WebBase64) new StreamReader(signoutstream).ReadToEnd();
                Expect(verifier.Verify(input, sig), Is.True);
            }
        }
    }
}