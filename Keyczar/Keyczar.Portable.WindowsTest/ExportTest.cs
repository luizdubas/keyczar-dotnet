using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Keyczar;
using Keyczar.Compat;
using Keyczar.Unofficial;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Diagnostics;

namespace KeyczarTest
{
    [TestFixture]
    public class ExportTest : AssertionHelper
    {
        private static readonly String TEST_DATA = Path.Combine("remote-testdata", "existing-data", "dotnet");

        [Test]
        public void TestSymetricKeyExport()
        {
            var ks = new KeySet(Util.TestDataPath(TEST_DATA, "aes"));
            Expect(() => ks.ExportPrimaryAsPkcs("dummy.pem", () => "dummy"),
                   Throws.InstanceOf<InvalidKeyTypeException>());
        }

        [Test]
        public async Task TestPublicKeyExport()
        {
            var ks = new KeySet(Util.TestDataPath(TEST_DATA, "rsa.public"));
            var path = "dummy.pem";
            Debug.WriteLine(path);
            await ks.ExportPrimaryAsPkcs(path, () => "dummy");
        }
    }
}