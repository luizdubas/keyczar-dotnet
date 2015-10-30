using System;
using System.IO;
using System.Linq;
using Keyczar;
using NUnit.Framework;
using System.Threading.Tasks;

namespace KeyczarTest
{
    [TestFixture("rem|dotnet")]
    [TestFixture("gen|cstestdata")]
    public class SessionDecryptTest : AssertionHelper
    {
        private readonly String TEST_DATA;

        private String input = "This is some test data";


        private Crypter privateKeyDecrypter;
        private AttachedVerifier publicKeyVerifier;

        [SetUp]
        public void Setup()
        {
            privateKeyDecrypter = new Crypter(Util.TestDataPath(TEST_DATA, "rsa"));
            publicKeyVerifier = new AttachedVerifier(Util.TestDataPath(TEST_DATA, "dsa.public"));
        }

        public SessionDecryptTest(string testPath)
        {
            testPath = Util.ReplaceDirPrefix(testPath);

            TEST_DATA = testPath;
        }

        [Test]
        public async Task TestSignedDecrypt()
        {
            var subPath = Util.TestDataPath(TEST_DATA, "signedsession");
            var sessionMaterialInput =
                (WebBase64) await Util.ReadFirstLine(Path.Combine(subPath, "signed.session.out"));

            var sessionCiphertextInput =
                (WebBase64) await Util.ReadFirstLine(Path.Combine(subPath, "signed.ciphertext.out"));

            using (var sessionCrypter = new SessionCrypter(privateKeyDecrypter, sessionMaterialInput, publicKeyVerifier)
                )
            {
                var plaintext = sessionCrypter.Decrypt(sessionCiphertextInput);
                Expect(plaintext, Is.EqualTo(input));
            }
        }

        [Test]
        public async Task TestDecrypt()
        {
            var subPath = Util.TestDataPath(TEST_DATA, "rsa");
            var sessionMaterialInput =
                (WebBase64) await Util.ReadFirstLine(Path.Combine(subPath, "session.material.out"));

            var sessionCiphertextInput =
                (WebBase64) await Util.ReadFirstLine(Path.Combine(subPath, "session.ciphertext.out"));

            using (var sessionCrypter = new SessionCrypter(privateKeyDecrypter, sessionMaterialInput))
            {
                var plaintext = sessionCrypter.Decrypt(sessionCiphertextInput);
                Expect(plaintext, Is.EqualTo(input));
            }
        }
    }
}