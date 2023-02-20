using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace GetIp.Ultils
{
    public class Security
    {
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
    }
}