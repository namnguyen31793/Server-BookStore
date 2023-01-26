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
    public class BillingSecurityManager
    {

        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
        public static string CreateMD5NoSpace(string source_str)
        {
            MD5 encrypter = new MD5CryptoServiceProvider();
            Byte[] original_bytes = ASCIIEncoding.Default.GetBytes(source_str);
            Byte[] encoded_bytes = encrypter.ComputeHash(original_bytes);
            return BitConverter.ToString(encoded_bytes).Replace("-", "").ToLower();

        }

        public static string CreateMD5NoSpaceUniCode(string data)
        {
            UTF8Encoding encoding1 = new UTF8Encoding();
            MD5CryptoServiceProvider provider1 = new MD5CryptoServiceProvider();
            byte[] buffer1 = encoding1.GetBytes(data);
            byte[] buffer2 = provider1.ComputeHash(buffer1);
            return BitConverter.ToString(buffer2).Replace("-", "").ToLower();
        }

        public static string HMACSHA256(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.ASCII.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64DeCode(string data)
        {
            try
            {
                System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();

                byte[] todecode_byte = Convert.FromBase64String(data);
                int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                char[] decoded_char = new char[charCount];
                utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
                string result = new String(decoded_char);
                return result;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string TripleDESEncrypt(string key, string data)
        {
            //data = data.Trim();
            byte[] bytes = Encoding.UTF8.GetBytes(key);
            string text = BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(bytes)).Replace("-", "").ToLower();
            byte[] bytes2 = Encoding.UTF8.GetBytes(text.Substring(0, 24));
            TripleDES tripleDES = TripleDES.Create();
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Key = bytes2;
            tripleDES.GenerateIV();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, tripleDES.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(Encoding.UTF8.GetBytes(data), 0, Encoding.UTF8.GetByteCount(data));
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

        public static string TripdeDESEnCriptWhyPay(string seckKey, string data)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            // Create a Byte array to store the encryption to return.
            byte[] key;
            // Required UTF8 Encoding used to encode the input value to a usable state.
            UTF8Encoding textencoder = new UTF8Encoding();

            // let the show begin.
            key = md5.ComputeHash(textencoder.GetBytes(seckKey));

            // Destroy objects that aren't needed.
            md5.Clear();
            md5 = null;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(data);

            byte[] first8byte = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                first8byte[i] = key[i];
            }
            //Take first 8 bytes of $key and append them to the end of $key.
            key = key.Concat(first8byte).ToArray();

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = key;
            //mode of operation. there are other 4 modes. We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray = cTransform.TransformFinalBlock
            (toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string TripdeDESDecryptedWhyPay(string seckKey, string data)
        {
            //Generate a key from a hash

            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            byte[] key = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(seckKey));

            byte[] toDecryptArray = Convert.FromBase64String(data);

            byte[] first8byte = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                first8byte[i] = key[i];
            }
            //Take first 8 bytes of $key and append them to the end of $key.
            key = key.Concat(first8byte).ToArray();

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = key;
            //mode of operation. there are other 4 modes. We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray = cTransform.TransformFinalBlock
            (toDecryptArray, 0, toDecryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the decrypted data into unreadable string format
            return System.Text.Encoding.UTF8.GetString(resultArray);
        }

        #region-----RSA
        public static string DecryptRSA(string keyString, string message, int keySize)
        {
            string decryptedText;
            using (var rsa = new RSACryptoServiceProvider(keySize))
            {
                try
                {
                    var base64Encrypted = message;
                    rsa.FromXmlString(keyString);
                    //// Convert the text from string to byte array for decryption 
                    ASCIIEncoding byteConverter = new ASCIIEncoding();

                    var resultBytes = Convert.FromBase64String(base64Encrypted);
                    var decryptedBytes = rsa.Decrypt(resultBytes, false);
                    var decryptedData = Encoding.UTF8.GetString(decryptedBytes);
                    //// Create an aux array to store all the encrypted bytes 

                    decryptedText = byteConverter.GetString(decryptedBytes);
                }
                finally
                {
                    //// Clear the RSA key container, deleting generated keys. 
                    rsa.PersistKeyInCsp = false;
                }
            }
            return decryptedText;
        }
        #endregion
    }
}
