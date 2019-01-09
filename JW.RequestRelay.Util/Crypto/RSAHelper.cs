using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace JW.RequestRelay.Util.Crypto
{
    public partial class RSAHelper
    {
        private RSACryptoServiceProvider _privateKeyRsaProvider;
        private RSACryptoServiceProvider _publicKeyRsaProvider;

        public RSAHelper(string privateKey, string publicKey = null)
        {
            if (!string.IsNullOrEmpty(privateKey))
            {
                _privateKeyRsaProvider = new RSACryptoServiceProvider();
                _privateKeyRsaProvider.FromXmlString(privateKey);
                //  _privateKeyRsaProvider = CreateRsaProviderFromPrivateKey(privateKey);
            }

            if (!string.IsNullOrEmpty(publicKey))
            {
                _publicKeyRsaProvider = new RSACryptoServiceProvider();
                _publicKeyRsaProvider.FromXmlString(publicKey);
                //  _publicKeyRsaProvider = CreateRsaProviderFromPublicKey(publicKey);
            }
        }


        private RSACryptoServiceProvider CreateRsaProviderFromPrivateKey(string privateKey)
        {
            var privateKeyBits = System.Convert.FromBase64String(privateKey);

            var RSA = new RSACryptoServiceProvider();
            var RSAparams = new RSAParameters();

            using (BinaryReader binr = new BinaryReader(new MemoryStream(privateKeyBits)))
            {
                byte bt = 0;
                ushort twobytes = 0;
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();
                else
                    throw new Exception("Unexpected value read binr.ReadUInt16()");

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)
                    throw new Exception("Unexpected version");

                bt = binr.ReadByte();
                if (bt != 0x00)
                    throw new Exception("Unexpected value read binr.ReadByte()");

                RSAparams.Modulus = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.Exponent = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.D = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.P = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.Q = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.DP = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.DQ = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.InverseQ = binr.ReadBytes(GetIntegerSize(binr));
            }

            RSA.ImportParameters(RSAparams);
            return RSA;
        }

        private int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();
            else
                if (bt == 0x82)
            {
                highbyte = binr.ReadByte();
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;
            }

            while (binr.ReadByte() == 0x00)
            {
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;
        }

        private RSACryptoServiceProvider CreateRsaProviderFromPublicKey(string publicKeyString)
        {
            // encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
            byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] x509key;
            byte[] seq = new byte[15];
            int x509size;

            x509key = Convert.FromBase64String(publicKeyString);
            x509size = x509key.Length;

            // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
            using (MemoryStream mem = new MemoryStream(x509key))
            {
                using (BinaryReader binr = new BinaryReader(mem))  //wrap Memory Stream with BinaryReader for easy reading
                {
                    byte bt = 0;
                    ushort twobytes = 0;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                        binr.ReadByte();    //advance 1 byte
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();   //advance 2 bytes
                    else
                        return null;

                    seq = binr.ReadBytes(15);       //read the Sequence OID
                    if (!CompareBytearrays(seq, SeqOID))    //make sure Sequence for OID is correct
                        return null;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8103) //data read as little endian order (actual data order for Bit String is 03 81)
                        binr.ReadByte();    //advance 1 byte
                    else if (twobytes == 0x8203)
                        binr.ReadInt16();   //advance 2 bytes
                    else
                        return null;

                    bt = binr.ReadByte();
                    if (bt != 0x00)     //expect null byte next
                        return null;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                        binr.ReadByte();    //advance 1 byte
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();   //advance 2 bytes
                    else
                        return null;

                    twobytes = binr.ReadUInt16();
                    byte lowbyte = 0x00;
                    byte highbyte = 0x00;

                    if (twobytes == 0x8102) //data read as little endian order (actual data order for Integer is 02 81)
                        lowbyte = binr.ReadByte();  // read next bytes which is bytes in modulus
                    else if (twobytes == 0x8202)
                    {
                        highbyte = binr.ReadByte(); //advance 2 bytes
                        lowbyte = binr.ReadByte();
                    }
                    else
                        return null;
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian order
                    int modsize = BitConverter.ToInt32(modint, 0);

                    int firstbyte = binr.PeekChar();
                    if (firstbyte == 0x00)
                    {   //if first byte (highest order) of modulus is zero, don't include it
                        binr.ReadByte();    //skip this null byte
                        modsize -= 1;   //reduce modulus buffer size by 1
                    }

                    byte[] modulus = binr.ReadBytes(modsize);   //read the modulus bytes

                    if (binr.ReadByte() != 0x02)            //expect an Integer for the exponent data
                        return null;
                    int expbytes = (int)binr.ReadByte();        // should only need one byte for actual exponent data (for all useful values)
                    byte[] exponent = binr.ReadBytes(expbytes);

                    // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                    RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                    RSAParameters RSAKeyInfo = new RSAParameters();
                    RSAKeyInfo.Modulus = modulus;
                    RSAKeyInfo.Exponent = exponent;
                    RSA.ImportParameters(RSAKeyInfo);

                    return RSA;
                }

            }
        }

        private bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }

        /// <summary>
        /// 生成密钥
        /// </summary>
        /// <param name="privateKeyPath">私钥完整路径</param>
        /// <param name="publicKeyPath">公钥完整路径</param>
        public static void GenderKey(string privateKeyPath, string publicKeyPath)
        {
            privateKeyPath = privateKeyPath.Replace("//", "/").Replace("\\", "/").Replace(@"\", "/");
            publicKeyPath = publicKeyPath.Replace("//", "/").Replace("\\", "/").Replace(@"\", "/");
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            var privatePath = privateKeyPath.Substring(0, privateKeyPath.LastIndexOf("/"));
            if (!Directory.Exists(privatePath))
            {
                Directory.CreateDirectory(privatePath);
            }
            using (StreamWriter writer = new StreamWriter(privateKeyPath))
            {
                var xml = rsa.ToXmlString(true);
                xml = XMLPrivatekeyToPEM(xml);
                writer.WriteLine(xml);
            }
            var publicPath = publicKeyPath.Substring(0, publicKeyPath.LastIndexOf("/"));
            if (!Directory.Exists(publicPath))
            {
                Directory.CreateDirectory(publicPath);
            }
            using (StreamWriter writer = new StreamWriter(publicKeyPath))
            {
                var xml = rsa.ToXmlString(false);
                xml = XMLPublickeyToPEM(xml);
                writer.WriteLine(xml);
            }
        }

        /// <summary>
        /// XML格式私钥转换PEM格式
        /// </summary>
        public static string XMLPrivatekeyToPEM(string xml)
        {
            var rsa2 = new RSACryptoServiceProvider();
            rsa2.FromXmlString(xml);
            var p = rsa2.ExportParameters(true);
            var key = new RsaPrivateCrtKeyParameters(
                 new BigInteger(1, p.Modulus), new BigInteger(1, p.Exponent), new BigInteger(1, p.D),
                 new BigInteger(1, p.P), new BigInteger(1, p.Q), new BigInteger(1, p.DP), new BigInteger(1, p.DQ),
                 new BigInteger(1, p.InverseQ));
            using (MemoryStream stream = new MemoryStream())
            {
                using (var sw = new StreamWriter(stream, Encoding.UTF8))
                {
                    var pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(sw);
                    pemWriter.WriteObject(key);
                    sw.Flush();
                    var ret = System.Text.Encoding.UTF8.GetString(stream.ToArray());
                    return ret;
                }
            }
        }

        /// <summary>
        /// XML格式私钥转换PEM格式
        /// </summary>
        public static string XMLPublickeyToPEM(string xml)
        {
            var rsa2 = new RSACryptoServiceProvider();
            rsa2.FromXmlString(xml);
            var p = rsa2.ExportParameters(false);
            var key = new RsaKeyParameters(false, new BigInteger(1, p.Modulus), new BigInteger(1, p.Exponent));
            using (MemoryStream stream = new MemoryStream())
            {
                using (var sw = new StreamWriter(stream, Encoding.UTF8))
                {
                    var pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(sw);
                    pemWriter.WriteObject(key);
                    sw.Flush();
                    var ret = System.Text.Encoding.UTF8.GetString(stream.ToArray());
                    return ret;
                }
            }
        }

        /// <summary>
        /// PEM 私钥转换为XML
        /// </summary>
        public static string PEMPrivatekeyToXML(string privateKey)
        {
            AsymmetricCipherKeyPair keyPair;
            var buffer = System.Text.Encoding.UTF8.GetBytes(privateKey);
            using (var stream = new MemoryStream(buffer))
            {
                using (var sr = new StreamReader(stream))
                {
                    var pemReader = new Org.BouncyCastle.OpenSsl.PemReader(sr);
                    keyPair = (AsymmetricCipherKeyPair)pemReader.ReadObject();
                }
            }
            var key = (RsaPrivateCrtKeyParameters)keyPair.Private;
            var p = new RSAParameters
            {
                Modulus = key.Modulus.ToByteArrayUnsigned(),
                Exponent = key.PublicExponent.ToByteArrayUnsigned(),
                D = key.Exponent.ToByteArrayUnsigned(),
                P = key.P.ToByteArrayUnsigned(),
                Q = key.Q.ToByteArrayUnsigned(),
                DP = key.DP.ToByteArrayUnsigned(),
                DQ = key.DQ.ToByteArrayUnsigned(),
                InverseQ = key.QInv.ToByteArrayUnsigned(),
            };
            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(p);
            return rsa.ToXmlString(true);
        }

        /// <summary>
        /// PEM公钥转换为XML
        /// </summary>
        public static string PEMPublickeyToXML(string publicKeyPEM)
        {
            RsaKeyParameters keyPair;
            var buffer = System.Text.Encoding.UTF8.GetBytes(publicKeyPEM);
            using (var stream = new MemoryStream(buffer))
            {
                using (var sr = new StreamReader(stream))
                {
                    var pemReader = new Org.BouncyCastle.OpenSsl.PemReader(sr);
                    keyPair = (RsaKeyParameters)pemReader.ReadObject();
                }
            }
            var key = keyPair;
            var p = new RSAParameters
            {
                Modulus = key.Modulus.ToByteArrayUnsigned(),
                Exponent = key.Exponent.ToByteArrayUnsigned(),
            };
            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(p);
            return rsa.ToXmlString(false);
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="rawInput"></param>
        /// <param name="publicKeyPEM"></param>
        /// <param name="publicKeyXml"></param>
        /// <returns></returns>
        public static string Encrypt(string rawInput, string publicKeyPEM = null, string publicKeyXml = null)
        {
            if (string.IsNullOrEmpty(rawInput))
            {
                return string.Empty;
            }
            if (publicKeyXml == null && publicKeyPEM != null)
            {
                publicKeyXml = PEMPublickeyToXML(publicKeyPEM);
            }
            if (string.IsNullOrWhiteSpace(publicKeyXml))
            {
                throw new ArgumentException("无效公钥");
            }
            using (var rsaProvider = new RSACryptoServiceProvider())
            {
                var inputBytes = Encoding.UTF8.GetBytes(rawInput);//有含义的字符串转化为字节流
                rsaProvider.FromXmlString(publicKeyXml);//载入公钥
                int bufferSize = (rsaProvider.KeySize / 8) - 11;//单块最大长度
                var buffer = new byte[bufferSize];
                using (MemoryStream inputStream = new MemoryStream(inputBytes), outputStream = new MemoryStream())
                {
                    while (true)
                    { //分段加密
                        int readSize = inputStream.Read(buffer, 0, bufferSize);
                        if (readSize <= 0)
                        {
                            break;
                        }
                        var temp = new byte[readSize];
                        Array.Copy(buffer, 0, temp, 0, readSize);
                        var encryptedBytes = rsaProvider.Encrypt(temp, false);
                        outputStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                    }
                    return Convert.ToBase64String(outputStream.ToArray());//转化为字节流方便传输
                }
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="privateKeyPEM"></param>
        /// <param name="privateKeyXml"></param>
        /// <returns></returns>
        public static string Decrypt(string cipherText, string privateKeyPEM = null, string privateKeyXml = null)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                return string.Empty;
            }
            if (privateKeyXml == null && privateKeyPEM != null)
            {
                privateKeyXml = PEMPrivatekeyToXML(privateKeyPEM);
            }
            if (string.IsNullOrWhiteSpace(privateKeyXml))
            {
                throw new ArgumentException("无效私钥");
            }
            using (var rsaProvider = new RSACryptoServiceProvider())
            {
                var inputBytes = Convert.FromBase64String(cipherText);
                rsaProvider.FromXmlString(privateKeyXml);
                int bufferSize = rsaProvider.KeySize / 8;
                var buffer = new byte[bufferSize];
                using (MemoryStream inputStream = new MemoryStream(inputBytes), outputStream = new MemoryStream())
                {
                    while (true)
                    {
                        int readSize = inputStream.Read(buffer, 0, bufferSize);
                        if (readSize <= 0)
                        {
                            break;
                        }
                        var temp = new byte[readSize];
                        Array.Copy(buffer, 0, temp, 0, readSize);
                        var rawBytes = rsaProvider.Decrypt(temp, false);
                        outputStream.Write(rawBytes, 0, rawBytes.Length);
                    }
                    return Encoding.UTF8.GetString(outputStream.ToArray());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="split"></param>
        /// <param name="privateKeyPEM"></param>
        /// <param name="privateKeyXml"></param>
        /// <returns></returns>
        public static string Decrypt(string cipherText, string split, string privateKeyPEM = null, string privateKeyXml = null)
        {
            var list = cipherText.ToListData("~");
            StringBuilder ret = new StringBuilder();
            list.ForEach(item =>
            {
                if (!string.IsNullOrEmpty(item))
                {
                    ret.Append(Decrypt(item, privateKeyPEM: privateKeyPEM, privateKeyXml: privateKeyXml));
                }
            });
            return ret.ToString();
        }
    }
}
