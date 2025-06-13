using iTextSharp.text.pdf;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Utilities.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlliedAdapter.Helpers
{
    public class PGPDecryptor
    {
        public static void DecryptFile(string inputFilePath, string outputFilePath, string privateKeyPath, string passphrase)
        {
            try
            {
                using (Stream privateKeyStream = File.OpenRead(privateKeyPath))
                using (Stream inputStream = File.OpenRead(inputFilePath))
                using (Stream outputStream = File.Create(outputFilePath))
                {
                    Decrypt(inputStream, outputStream, privateKeyStream, passphrase.ToCharArray());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Decryption failed: " + ex.Message);
            }
        }

        private static void Decrypt(Stream inputStream, Stream outputStream, Stream privateKeyStream, char[] passphrase)
        {
            inputStream = PgpUtilities.GetDecoderStream(inputStream);
            PgpObjectFactory pgpFactory = new PgpObjectFactory(inputStream);
            PgpEncryptedDataList encryptedDataList = GetEncryptedDataList(pgpFactory);

            PgpPrivateKey privateKey = FindPrivateKey(privateKeyStream, encryptedDataList, passphrase);

            using (Stream clearStream = GetClearStream(encryptedDataList, privateKey))
            {
                PgpObjectFactory clearFactory = new PgpObjectFactory(clearStream);
                PgpCompressedData compressedData = (PgpCompressedData)clearFactory.NextPgpObject();
                PgpLiteralData literalData = GetLiteralData(compressedData);

                using (Stream literalStream = literalData.GetInputStream())
                {
                    Streams.PipeAll(literalStream, outputStream);
                }
            }
        }

        private static PgpEncryptedDataList GetEncryptedDataList(PgpObjectFactory pgpFactory)
        {
            PgpObject pgpObject = pgpFactory.NextPgpObject();
            return pgpObject is PgpEncryptedDataList encryptedDataList
                ? encryptedDataList
                : (PgpEncryptedDataList)pgpFactory.NextPgpObject();
        }

        private static PgpPrivateKey FindPrivateKey(Stream privateKeyStream, PgpEncryptedDataList encryptedDataList, char[] passphrase)
        {
            foreach (PgpPublicKeyEncryptedData data in encryptedDataList.GetEncryptedDataObjects())
            {
                PgpSecretKeyRingBundle secretKeyRingBundle = new PgpSecretKeyRingBundle(
                    PgpUtilities.GetDecoderStream(privateKeyStream)
                );

                foreach (PgpSecretKeyRing keyRing in secretKeyRingBundle.GetKeyRings())
                {
                    foreach (PgpSecretKey secretKey in keyRing.GetSecretKeys())
                    {
                        if (secretKey.KeyId == data.KeyId)
                        {
                            return secretKey.ExtractPrivateKey(passphrase);
                        }
                    }
                }
            }
            throw new ArgumentException("Private key not found for decryption.");
        }

        private static Stream GetClearStream(PgpEncryptedDataList encryptedDataList, PgpPrivateKey privateKey)
        {
            PgpPublicKeyEncryptedData publicKeyData = (PgpPublicKeyEncryptedData)encryptedDataList[0];
            return publicKeyData.GetDataStream(privateKey);
        }

        private static PgpLiteralData GetLiteralData(PgpCompressedData compressedData)
        {
            PgpObjectFactory pgpFactory = new PgpObjectFactory(compressedData.GetDataStream());
            PgpObject pgpObject = pgpFactory.NextPgpObject();
            return pgpObject is PgpLiteralData literalData
                ? literalData
                : (PgpLiteralData)pgpFactory.NextPgpObject();
        }
    }
}
