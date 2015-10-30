/* 
 * Copyright 2013 James Tuley (jay+code@tuley.name)
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Keyczar;
using NUnit.Framework;
using System.Threading.Tasks;

namespace KeyczarTest
{
    public class CollisionTest : AssertionHelper
    {
        private static String input = "This is some test data";
        private readonly String TEST_DATA = Path.Combine("remote-testdata", "special-case", "key-collision");

        [TestCase("aes")]
        [TestCase("rsa")]
        public async Task TwoKeysWithSameHashDecrypt(string dir)
        {
            var subPath = Util.TestDataPath(TEST_DATA, dir);

            using (var crypter = new Crypter(subPath))
            {
                var activeCiphertext = (WebBase64) await Util.ReadFirstLine(Path.Combine(subPath, "1.out"));
                var primaryCiphertext = (WebBase64) await Util.ReadFirstLine(Path.Combine(subPath, "2.out"));

                var activeDecrypted = crypter.Decrypt(activeCiphertext);
                Expect(activeDecrypted, Is.EqualTo(input));
                var primaryDecrypted = crypter.Decrypt(primaryCiphertext);
                Expect(primaryDecrypted, Is.EqualTo(input));
            }
        }


        [TestCase("hmac")]
        [TestCase("dsa")]
        [TestCase("rsa-sign")]
        public async Task TwoKeysWithSameHashVerify(string dir)
        {
            var subPath = Util.TestDataPath(TEST_DATA, dir);

            using (var verifier = new Verifier(subPath))
            {
                var activeSignature = (WebBase64) await Util.ReadFirstLine(Path.Combine(subPath, "1.out"));
                var primarySignature = (WebBase64) await Util.ReadFirstLine(Path.Combine(subPath, "2.out"));

                var activeVerify = verifier.Verify(input, activeSignature);
                Expect(activeVerify, Is.True);
                var primaryVerify = verifier.Verify(input, primarySignature);
                Expect(primaryVerify, Is.True);
            }
        }

        [TestCase("hmac")]
        [TestCase("dsa")]
        [TestCase("rsa-sign")]
        public async Task TwoKeysWithSameHashTimeoutVerifySucess(string dir)
        {
            Func<DateTime> earlyCurrentTimeProvider =
                () => new DateTime(2012, 12, 21, 11, 11, 0, DateTimeKind.Utc).AddMinutes(-5);

            var subPath = Util.TestDataPath(TEST_DATA, dir);

            using (var verifier = new TimeoutVerifier(subPath, earlyCurrentTimeProvider))
            {
                var activeSignature = (WebBase64) await Util.ReadFirstLine(Path.Combine(subPath, "1.timeout"));
                var primarySignature = (WebBase64) await Util.ReadFirstLine(Path.Combine(subPath, "2.timeout"));

                var activeVerify = verifier.Verify(input, activeSignature);
                Expect(activeVerify, Is.True);
                var primaryVerify = verifier.Verify(input, primarySignature);
                Expect(primaryVerify, Is.True);
            }
        }

        [TestCase("hmac")]
        [TestCase("dsa")]
        [TestCase("rsa-sign")]
        public async Task TwoKeysWithSameHashTimeoutVerifyExpired(string dir)
        {
            Func<DateTime> lateCurrentTimeProvider =
                () => new DateTime(2012, 12, 21, 11, 11, 0, DateTimeKind.Utc).AddMinutes(5);

            var subPath = Util.TestDataPath(TEST_DATA, dir);

            using (var verifier = new TimeoutVerifier(subPath, lateCurrentTimeProvider))
            {
                var activeSignature = (WebBase64) await Util.ReadFirstLine(Path.Combine(subPath, "1.timeout"));
                var primarySignature = (WebBase64) await Util.ReadFirstLine(Path.Combine(subPath, "2.timeout"));

                var activeVerify = verifier.Verify(input, activeSignature);
                Expect(activeVerify, Is.False);
                var primaryVerify = verifier.Verify(input, primarySignature);
                Expect(primaryVerify, Is.False);
            }
        }

        [TestCase("hmac")]
        [TestCase("dsa")]
        [TestCase("rsa-sign")]
        public async Task TwoKeysWithSameHashTimeoutAttachedVerify(string dir)
        {
            var subPath = Util.TestDataPath(TEST_DATA, dir);

            using (var verifier = new AttachedVerifier(subPath))
            {
                var activeSignature = (WebBase64) await Util.ReadFirstLine(Path.Combine(subPath, "1.attached"));
                var primarySignature = (WebBase64) await Util.ReadFirstLine(Path.Combine(subPath, "2.attached"));

                string activeVerifiedMessage;
                var activeVerify = verifier.TryGetVerifiedMessage(activeSignature, out activeVerifiedMessage);
                Expect(activeVerify, Is.True);
                Expect(activeVerifiedMessage, Is.EqualTo(input));
                string primaryVerifiedMessage;
                var primaryVerify = verifier.TryGetVerifiedMessage(primarySignature, out primaryVerifiedMessage);
                Expect(primaryVerify, Is.True);
                Expect(primaryVerifiedMessage, Is.EqualTo(input));
            }
        }

        [TestCase("hmac")]
        [TestCase("dsa")]
        [TestCase("rsa-sign")]
        public async Task TwoKeysWithSameHashTimeoutAttachedSecretVerify(string dir)
        {
            var subPath = Util.TestDataPath(TEST_DATA, dir);

            using (var verifier = new AttachedVerifier(subPath))
            {
                var activeSignature = (WebBase64) await Util.ReadFirstLine(Path.Combine(subPath, "1.secret.attached"));
                var primarySignature = (WebBase64) await Util.ReadFirstLine(Path.Combine(subPath, "2.secret.attached"));

                string activeVerifiedMessage;
                var activeVerify = verifier.TryGetVerifiedMessage(activeSignature, out activeVerifiedMessage,
                                                                  Encoding.UTF8.GetBytes("secret"));
                Expect(activeVerify, Is.True);
                Expect(activeVerifiedMessage, Is.EqualTo(input));
                string primaryVerifiedMessage;
                var primaryVerify = verifier.TryGetVerifiedMessage(primarySignature, out primaryVerifiedMessage,
                                                                   Encoding.UTF8.GetBytes("secret"));
                Expect(primaryVerify, Is.True);
                Expect(primaryVerifiedMessage, Is.EqualTo(input));
            }
        }
    }
}