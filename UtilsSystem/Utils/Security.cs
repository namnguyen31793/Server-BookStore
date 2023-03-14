using ShareData.LogSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UtilsSystem.Utils
{
    public class Security
    {
        private static readonly RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();

        private Security()
        {
        }

        public static string GetVerifyToken(ref string verifyToken)
        {
            string empty = string.Empty;
            return Security.GetVerifyToken(ref verifyToken, ref empty, 6);
        }

        public static string GetVerifyToken(ref string verifyToken, int length)
        {
            string empty = string.Empty;
            return Security.GetVerifyToken(ref verifyToken, ref empty, length);
        }

        public static string GetVerifyToken(ref string verifyToken, ref string captcha)
        {
            return Security.GetVerifyToken(ref verifyToken, ref captcha, 6);
        }

        public static string GetVerifyToken(ref string verifyToken, ref string captcha, int length)
        {
            byte[] array = new byte[length];
            Security.rngCsp.GetBytes(array);
            int length2 = "123456789ABCDEFGHIJKLMNPQRSTUVXYZ".Length;
            string text = "";
            int num;
            for (int i = 0; i < length; i = num + 1)
            {
                text += "123456789ABCDEFGHIJKLMNPQRSTUVXYZ"[(int)array[i] % length2].ToString();
                num = i;
            }
            long ticks = DateTime.Now.Ticks;
            captcha = text;
            verifyToken = string.Format("{0}-{1}", ticks, Security.MD5Encrypt(text + ticks.ToString()));
            return "";
        }

        public static string GetTokenPlainText(string token)
        {
            bool flag = string.IsNullOrEmpty(token);
            string result;
            if (flag)
            {
                result = string.Empty;
            }
            else
            {
                result = "";
            }
            return result;
        }

        public static DateTime GetTokenTime(string verify)
        {
            long ticks = Convert.ToInt64(verify.Split(new char[]
            {
                '-'
            })[0]);
            return new DateTime(ticks);
        }

        public static string MD5Encrypt(string plainText)
        {
            UTF8Encoding uTF8Encoding = new UTF8Encoding();
            MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
            byte[] bytes = uTF8Encoding.GetBytes(plainText);
            byte[] value = mD5CryptoServiceProvider.ComputeHash(bytes);
            return BitConverter.ToString(value).Replace("-", "").ToLower();
        }

        public static string EncryptPlainTextToCipherText(string PlainText, string SecurityKey)
        {
            // Getting the bytes of Input String.
            byte[] toEncryptedArray = UTF8Encoding.UTF8.GetBytes(PlainText);

            MD5CryptoServiceProvider objMD5CryptoService = new MD5CryptoServiceProvider();
            //Gettting the bytes from the Security Key and Passing it to compute the Corresponding Hash Value.
            byte[] securityKeyArray = objMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(SecurityKey));
            //De-allocatinng the memory after doing the Job.
            objMD5CryptoService.Clear();

            var objTripleDESCryptoService = new TripleDESCryptoServiceProvider();
            //Assigning the Security key to the TripleDES Service Provider.
            objTripleDESCryptoService.Key = securityKeyArray;
            //Mode of the Crypto service is Electronic Code Book.
            objTripleDESCryptoService.Mode = CipherMode.ECB;
            //Padding Mode is PKCS7 if there is any extra byte is added.
            objTripleDESCryptoService.Padding = PaddingMode.PKCS7;


            var objCrytpoTransform = objTripleDESCryptoService.CreateEncryptor();
            //Transform the bytes array to resultArray
            byte[] resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptedArray, 0, toEncryptedArray.Length);
            objTripleDESCryptoService.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        //This method is used to convert the Encrypted/Un-Readable Text back to readable  format.
        public static string DecryptCipherTextToPlainText(string CipherText, string SecurityKey)
        {
            byte[] toEncryptArray = Convert.FromBase64String(CipherText);
            MD5CryptoServiceProvider objMD5CryptoService = new MD5CryptoServiceProvider();

            //Gettting the bytes from the Security Key and Passing it to compute the Corresponding Hash Value.
            byte[] securityKeyArray = objMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(SecurityKey));
            objMD5CryptoService.Clear();

            var objTripleDESCryptoService = new TripleDESCryptoServiceProvider();
            //Assigning the Security key to the TripleDES Service Provider.
            objTripleDESCryptoService.Key = securityKeyArray;
            //Mode of the Crypto service is Electronic Code Book.
            objTripleDESCryptoService.Mode = CipherMode.ECB;
            //Padding Mode is PKCS7 if there is any extra byte is added.
            objTripleDESCryptoService.Padding = PaddingMode.PKCS7;

            var objCrytpoTransform = objTripleDESCryptoService.CreateDecryptor();
            //Transform the bytes array to resultArray
            byte[] resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            objTripleDESCryptoService.Clear();

            //Convert and return the decrypted data/byte into string format.
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    
        public static string EncryptPassword(string userName, string Md5password, string salt)
        {
            return Md5password;
        }

        public static string RandomPassword()
        {
            string text = string.Empty;
            Random random = new Random(DateTime.Now.Millisecond);
            int num;
            for (int i = 1; i < 10; i = num + 1)
            {
                text = string.Format("{0}{1}", text, random.Next(0, 9));
                num = i;
            }
            return text;
        }

        public static string RandomString(int length)
        {
            string text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int length2 = text.Length;
            Random random = new Random();
            string text2 = string.Empty;
            int num;
            for (int i = 0; i < length; i = num + 1)
            {
                text2 = string.Format("{0}{1}", text2, text[random.Next(length2)]);
                num = i;
            }
            return text2;
        }

        public static string TripleDESEncrypt(string key, string data)
        {
            data = data.Trim();
            byte[] bytes = Encoding.ASCII.GetBytes(key);
            string text = BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(bytes)).Replace("-", "").ToLower();
            byte[] bytes2 = Encoding.ASCII.GetBytes(text.Substring(0, 24));
            TripleDES tripleDES = TripleDES.Create();
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Key = bytes2;
            tripleDES.GenerateIV();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, tripleDES.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(Encoding.ASCII.GetBytes(data), 0, Encoding.ASCII.GetByteCount(data));
            cryptoStream.FlushFinalBlock();
            byte[] array = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(array, 0, array.GetLength(0)).Trim();
        }

        public static string TripleDESDecrypt(string key, string data)
        {
            string result;
            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(key);
                string text = BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(bytes)).Replace("-", "").ToLower();
                byte[] bytes2 = Encoding.ASCII.GetBytes(text.Substring(0, 24));
                TripleDES tripleDES = TripleDES.Create();
                tripleDES.Mode = CipherMode.ECB;
                tripleDES.Key = bytes2;
                byte[] array = Convert.FromBase64String(data);
                MemoryStream stream = new MemoryStream(array, 0, array.Length);
                ICryptoTransform transform = tripleDES.CreateDecryptor();
                CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read);
                StreamReader streamReader = new StreamReader(stream2);
                result = streamReader.ReadToEnd();
            }
            catch
            {
                result = string.Empty;
            }
            return result;
        }

        public static string Encrypt(string key, string data)
        {
            data = data.Trim();
            bool flag = string.IsNullOrEmpty(data);
            string result;
            if (flag)
            {
                result = "Input string is empty!";
            }
            else
            {
                byte[] bytes = Encoding.ASCII.GetBytes(key);
                string text = BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(bytes)).Replace("-", "").ToLower();
                byte[] bytes2 = Encoding.ASCII.GetBytes(text.Substring(0, 24));
                TripleDES tripleDES = TripleDES.Create();
                tripleDES.Mode = CipherMode.ECB;
                tripleDES.Key = bytes2;
                tripleDES.GenerateIV();
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, tripleDES.CreateEncryptor(), CryptoStreamMode.Write);
                cryptoStream.Write(Encoding.ASCII.GetBytes(data), 0, Encoding.ASCII.GetByteCount(data));
                cryptoStream.FlushFinalBlock();
                byte[] array = memoryStream.ToArray();
                memoryStream.Close();
                cryptoStream.Close();
                result = Convert.ToBase64String(array, 0, array.GetLength(0)).Trim();
            }
            return result;
        }

        public static string Decrypt(string key, string data)
        {
            data = data.Replace("-", "").Replace(" ", "+");
            byte[] bytes = Encoding.ASCII.GetBytes(key);
            string text = BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(bytes)).Replace("-", "").Replace(" ", "+").ToLower();
            // NLogManager.LogMessage(data+"_xxx_"+text+"_");
            byte[] bytes2 = Encoding.ASCII.GetBytes(text.Substring(0, 24));
            TripleDES tripleDES = TripleDES.Create();
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Key = bytes2;
            byte[] array = Convert.FromBase64String(data);
            MemoryStream stream = new MemoryStream(array, 0, array.Length);
            ICryptoTransform transform = tripleDES.CreateDecryptor();
            CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read);
            StreamReader streamReader = new StreamReader(stream2);
            return streamReader.ReadToEnd();
        }

        public static string Base64Encode(string data)
        {
            string result;
            try
            {
                byte[] array = new byte[data.Length];
                result = Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
            return result;
        }

        public static string Base64Decode(string data)
        {
            string result;
            try
            {
                Decoder decoder = new UTF8Encoding().GetDecoder();
                byte[] array = Convert.FromBase64String(data);
                char[] array2 = new char[decoder.GetCharCount(array, 0, array.Length)];
                decoder.GetChars(array, 0, array.Length, array2, 0);
                result = new string(array2);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Decode" + ex.Message);
            }
            return result;
        }
        #region ClientEnCode
        const int Iterations = 555;
        static string strPassword = "VtvLive@2018!123";
        static string strSalt = "VtvLive123";
        public static string EncryptBase64(string strPlain)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();

                Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(strPassword, GetIV(), Iterations);

                byte[] key = rfc2898DeriveBytes.GetBytes(8);

                using (var memoryStream = new MemoryStream())
                using (var cryptoStream = new CryptoStream(memoryStream, des.CreateEncryptor(key, GetIV()), CryptoStreamMode.Write))
                {
                    memoryStream.Write(GetIV(), 0, GetIV().Length);

                    byte[] plainTextBytes = Encoding.UTF8.GetBytes(strPlain);

                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();

                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
            catch (Exception)
            {
                return strPlain;
            }
        }

        public static string DecryptBase64(string strEncript)
        {
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(strEncript);

                using (var memoryStream = new MemoryStream(cipherBytes))
                {
                    DESCryptoServiceProvider des = new DESCryptoServiceProvider();

                    byte[] iv = GetIV();
                    memoryStream.Read(iv, 0, iv.Length);

                    // use derive bytes to generate key from password and IV
                    var rfc2898DeriveBytes = new Rfc2898DeriveBytes(strPassword, iv, Iterations);

                    byte[] key = rfc2898DeriveBytes.GetBytes(8);

                    using (var cryptoStream = new CryptoStream(memoryStream, des.CreateDecryptor(key, iv), CryptoStreamMode.Read))
                    using (var streamReader = new StreamReader(cryptoStream))
                    {
                        string strPlain = streamReader.ReadToEnd();
                        return strPlain;
                    }
                }
            }
            catch (Exception)
            {
                return strEncript;
            }

        }

        static byte[] GetIV()
        {
            byte[] IV = Encoding.UTF8.GetBytes(strSalt);
            return IV;
        }
        #endregion

        public static string HMACSHA256(string message, string secret)
        {
            secret = (secret ?? "");
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            string result;
            using (HMACSHA256 hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                result = Convert.ToBase64String(hashmessage);
            }
            return result;
        }

        public static string HmacSha256Digest(string message, string secretKey)
        {
            byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(secretKey);
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
            System.Security.Cryptography.HMACSHA256 cryptographer = new System.Security.Cryptography.HMACSHA256(keyBytes);
            byte[] bytes = cryptographer.ComputeHash(messageBytes);
            string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
            return base64String;
        }

        public static string Compress(string s)
        {
            //byte[] bytes = CLZF2.Compress(Encoding.Unicode.GetBytes(s));
            //byte[] bytes = SevenZipCompressor.CompressBytes(Encoding.Unicode.GetBytes(s));
            byte[] bytes = LZ4.LZ4Codec.Wrap(Encoding.UTF8.GetBytes(s));

            return Convert.ToBase64String(bytes);
        }

        public static string Decompress(string s)
        {
            //byte[] bytes = CLZF2.Decompress(Convert.FromBase64String(s));
            //byte[] bytes = SevenZipExtractor.ExtractBytes(Convert.FromBase64String(s));

            byte[] bytes = LZ4.LZ4Codec.Unwrap(Convert.FromBase64String(s));

            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        #region JWT
        public static string Base64UrlEncode(byte[] input)
        {
            var output = Convert.ToBase64String(input);
            output = output.Split('=')[0]; // Remove any trailing '='s
            output = output.Replace('+', '-'); // 62nd char of encoding
            output = output.Replace('/', '_'); // 63rd char of encoding
            return output;
        }

        public static string CreateToken(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Base64UrlEncode(hashmessage);
            }
        }
        #endregion
    }
}
