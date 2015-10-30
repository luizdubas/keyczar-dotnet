using System;
using System.IO;
using System.Linq;
using Keyczar;
using Keyczar.Compat;
using NUnit.Framework;
using System.Threading.Tasks;

namespace KeyczarTest.Interop
{
    public abstract class VerifierFullInterop : Interop
    {
        protected string Location;

        public VerifierFullInterop(string imp)
            : base(imp)
        {
        }

        [Test]
        public async Task VerifyAttached()
        {
            var path = TestData(Location);
            using (var verifier = new AttachedVerifier(path))
            {
                var primarySignature = (WebBase64) await Util.ReadFirstLine(Path.Combine(path, "2.attached"));
                Expect(verifier.Verify(primarySignature), Is.True);
            }
        }

        [Test]
        public async Task VerifyAttachedSecret()
        {
            var path = TestData(Location);
            using (var verifier = new AttachedVerifier(path))
            {
                var primarySignature = (WebBase64) await Util.ReadFirstLine(Path.Combine(path, "2.secret.attached"));
                Expect(verifier.Verify(primarySignature, Keyczar.Keyczar.RawStringEncoding.GetBytes("secret")), Is.True);
            }
        }

        [Test]
        public async Task VerifyTimeoutSucess()
        {
            Func<DateTime> earlyCurrentTimeProvider =
                () => new DateTime(2012, 12, 21, 11, 11, 0, DateTimeKind.Utc).AddMinutes(-5);

            var path = TestData(Location);
            using (var verifier = new TimeoutVerifier(path, earlyCurrentTimeProvider))
            {
                var primarySignature = (WebBase64) await Util.ReadFirstLine(Path.Combine(path, "2.timeout"));
                Expect(verifier.Verify(Input, primarySignature), Is.True);
            }
        }

        [Test]
        public async Task VerifyTimeoutExpired()
        {
            Func<DateTime> lateCurrentTimeProvider =
                () => new DateTime(2012, 12, 21, 11, 11, 0, DateTimeKind.Utc).AddMinutes(5);
            var path = TestData(Location);
            using (var verifier = new TimeoutVerifier(path, lateCurrentTimeProvider))
            {
                var primarySignature = (WebBase64) await Util.ReadFirstLine(Path.Combine(path, "2.timeout"));
                Expect(verifier.Verify(Input, primarySignature), Is.False);
            }
        }


        [Test]
        public async Task VerifyUnversioned()
        {
            var path = TestData(Location);
            using (var verifier = new VanillaVerifier(path))
            {
                var primarySignature = (WebBase64) await Util.ReadFirstLine(Path.Combine(path, "2.unversioned"));
                Expect(verifier.Verify(Input, primarySignature), Is.True);
            }
        }
    }
}