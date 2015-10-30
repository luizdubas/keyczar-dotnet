﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Keyczar.Crypto;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Keyczar.Util;
using Windows.Storage;
using System.Threading.Tasks;

namespace Keyczar.Compat
{
    /// <summary>
    /// Methods for exporting a keyset to a third party format
    /// </summary>
    public static class Export
    {
        /// <summary>
        /// Exports the primary key as PKCS.
        /// </summary>
        /// <param name="keySet">The keyset.</param>
        /// <param name="location">The location.</param>
        /// <param name="passwordPrompt">The password prompt.</param>
        /// <returns></returns>
        /// <exception cref="InvalidKeyTypeException">Needs to be a private key.</exception>
        /// <exception cref="InvalidKeyTypeException">Non exportable key type.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage",
            "CA2202:Do not dispose objects multiple times")]
        public static async Task<bool> ExportPrimaryAsPkcs(this IKeySet keySet, string location, Func<string> passwordPrompt)
        {
            var i = keySet.Metadata.Versions.First(it => it.Status == KeyStatus.Primary).VersionNumber;
            using (var key = keySet.GetKey(i))
            {
                using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(location, CreationCollisionOption.ReplaceExisting).ConfigureAwait(false))
                using (var writer = new StreamWriter(stream))
                {
                    var pemWriter = new Org.BouncyCastle.Utilities.IO.Pem.PemWriter(writer);

                    AsymmetricKeyParameter writeKey;
                    if (!(key is IPrivateKey))
                    {
                        if (key.KeyType == KeyType.DsaPub)
                        {
                            var dsaKey = (DsaPublicKey) key;
                            writeKey = new DsaPublicKeyParameters(dsaKey.Y.ToBouncyBigInteger(),
                                                                  new DsaParameters(
                                                                      dsaKey.P.ToBouncyBigInteger(),
                                                                      dsaKey.Q.ToBouncyBigInteger(),
                                                                      dsaKey.G.ToBouncyBigInteger()));
                        }
                        else if (key is IRsaPublicKey)
                        {
                            var rsaKey = (IRsaPublicKey) key;
                            writeKey = new RsaKeyParameters(false,
                                                            rsaKey.Modulus.ToBouncyBigInteger(),
                                                            rsaKey.PublicExponent.ToBouncyBigInteger());
                        }
                        else
                        {
                            throw new InvalidKeyTypeException("Non exportable key type.");
                        }

                        pemWriter.WriteObject(new MiscPemGenerator(writeKey));
                    }
                    else
                    {
                        if (key.KeyType == KeyType.DsaPriv)
                        {
                            var dsaKey = (DsaPrivateKey) key;
                            writeKey = new DsaPrivateKeyParameters(dsaKey.X.ToBouncyBigInteger(),
                                                                   new DsaParameters(
                                                                       dsaKey.PublicKey.P.ToBouncyBigInteger(),
                                                                       dsaKey.PublicKey.Q.ToBouncyBigInteger(),
                                                                       dsaKey.PublicKey.G.ToBouncyBigInteger()));
                        }
                        else if (key is IRsaPrivateKey)
                        {
                            var rsaKey = (IRsaPrivateKey) key;
                            writeKey = new RsaPrivateCrtKeyParameters(
                                rsaKey.PublicKey.Modulus.ToBouncyBigInteger(),
                                rsaKey.PublicKey.PublicExponent.ToBouncyBigInteger(),
                                rsaKey.PrivateExponent.ToBouncyBigInteger(),
                                rsaKey.PrimeP.ToBouncyBigInteger(),
                                rsaKey.PrimeQ.ToBouncyBigInteger(),
                                rsaKey.PrimeExponentP.ToBouncyBigInteger(),
                                rsaKey.PrimeExponentQ.ToBouncyBigInteger(),
                                rsaKey.CrtCoefficient.ToBouncyBigInteger());
                        }
                        else
                        {
                            throw new InvalidKeyTypeException("Non exportable key type.");
                        }

                        pemWriter.WriteObject(new Pkcs8Generator(writeKey, Pkcs8Generator.PbeSha1_RC2_128)
                                                  {
                                                      Password = (passwordPrompt() ?? String.Empty).ToCharArray(),
                                                      SecureRandom = Secure.Random,
                                                      IterationCount = 4096
                                                  });
                    }
                }
            }

            return true;
        }
    }
}