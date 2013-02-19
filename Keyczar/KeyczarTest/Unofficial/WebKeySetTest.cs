﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Keyczar;
using Keyczar.Unofficial;
using NUnit.Framework;

namespace KeyczarTest.Unofficial
{
    [Category("Unofficial")]
    public class WebKeySetTest : AssertionHelper
    {

        private static string input = "Some test text";

		private static string TEST_DATA = Path.Combine("remote-testdata","existing-data", "dotnet");

        private static string TEST_WEBDATA = "http://jbtule.github.com/keyczar-dotnet/keys/";

        [Test]
        public void TestCryptedKey()
        {
            var basePath = Util.TestDataPath(TEST_DATA, "");
            var keyPath = Path.Combine(basePath, "aes");
            var webKeyPath = TEST_WEBDATA + "aes-crypted/";

            WebBase64 ciphertext;
            using (var keyDecrypter = new Crypter(keyPath))
            {
                using (var dataEncrypter = new Encrypter(new EncryptedKeySet(new WebKeySet(webKeyPath), keyDecrypter)))
                {
                    ciphertext = dataEncrypter.Encrypt(input);



                }

                using (var dataDecrypter = new Crypter(new EncryptedKeySet(new WebKeySet(webKeyPath), keyDecrypter)))
                {
                    var plaintext = dataDecrypter.Decrypt(ciphertext);
                    Expect(plaintext, Is.EqualTo(input));
                }
            }
        }

        [Test]
        public void TestPublicKeyCrypt()
        {
            var basePath = Util.TestDataPath(TEST_DATA, "");
            var keyPath = Path.Combine(basePath, "rsa");
            var webKeyPath = TEST_WEBDATA + "rsa.public/";
            
            WebBase64 ciphertext; 
            using (var dataEncrypter = new Encrypter(new WebKeySet(webKeyPath)))
            {
                ciphertext =dataEncrypter.Encrypt(input);
            }

            using (var dataDecrypter = new Crypter(keyPath))
            {
                var plaintext = dataDecrypter.Decrypt(ciphertext);
                Expect(plaintext, Is.EqualTo(input));
            }

        }


        [Test]
        public void TestPublicKeyVerify()
        {
            var basePath = Util.TestDataPath(TEST_DATA, "");
            var keyPath = Path.Combine(basePath, "rsa-sign");
            var webKeyPath = TEST_WEBDATA + "rsa-sign.public/";

            WebBase64 signature;
            using (var dataSigner = new Signer(keyPath))
            {
                signature = dataSigner.Sign(input);
            }

            using (var dataVerifier = new Verifier(new WebKeySet(webKeyPath)))
            {
                var verified = dataVerifier.Verify(input, signature);
                Expect(verified, Is.True);
            }
        }
    }
}
