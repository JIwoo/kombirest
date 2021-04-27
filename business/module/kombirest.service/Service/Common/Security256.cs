using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using db.service;
using kombirest.service.Interface.Commons;

namespace kombirest.service.Service.Common
{
    public class Security256: SqlExecute, ISecurity
    {
        private static readonly string _AESKEY = "WkdRprotozldi$Rjwlfk~#@*(%&*(&(@n$n@$n(*&(@(ne^fgf(*&sww&!N(*#NR&*(8%04kfowjstprPgocnd!tlzlemfdi2*qufwhrgof!~%f&*f*^&tlqkfakdi#$";

        /// <summary>
        /// AES256 암호화
        /// </summary>
        /// <param name="InputString">평문</param>
        /// <returns></returns>
        public string EncryptString(string InputString)
        {
            try
            {
                string plainString = InputString.ToString();

                //평문 바이트로 변환
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainString);
                byte[] Salt = Encoding.ASCII.GetBytes(_AESKEY.Length.ToString());
                PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(_AESKEY, Salt);

                //린달 알고리즘
                RijndaelManaged RCipher = new RijndaelManaged();

                //메모리스트림 생성
                MemoryStream memoryStream = new MemoryStream();
                ICryptoTransform encrypt = RCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

                //키와 백터값으로 암호화 스트림 생성
                CryptoStream cryptoStream = new CryptoStream(memoryStream, encrypt, CryptoStreamMode.Write);
                cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                cryptoStream.FlushFinalBlock();

                //메모리스트림에 담겨있는 암호바이트 배열을 담음
                byte[] encryptBytes = memoryStream.ToArray();
                string encryptString = Convert.ToBase64String(encryptBytes);

                cryptoStream.Close();
                memoryStream.Close();

                return encryptString;
            }
            catch (Exception) { return string.Empty; }
        }

        /// <summary>
        /// AES256 복호화
        /// </summary>
        /// <param name="InputEncrypt">암호문</param>
        /// <returns></returns>
        public string DecryptString(string InputEncrypt)
        {
            try
            {
                string encryptString = InputEncrypt.ToString();

                //암호문 바이트로 변환
                byte[] encryptBytes = Convert.FromBase64String(encryptString);
                byte[] Salt = Encoding.ASCII.GetBytes(_AESKEY.Length.ToString());
                PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(_AESKEY, Salt);

                //린달 알고리즘
                RijndaelManaged RCipher = new RijndaelManaged();

                //메모리스트림 생성
                MemoryStream memoryStream = new MemoryStream(encryptBytes);
                ICryptoTransform decrypt = RCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

                //키와 백터값으로 암호화 스트림 생성
                CryptoStream cryptoStream = new CryptoStream(memoryStream, decrypt, CryptoStreamMode.Read);

                //복호화
                byte[] plainBytes = new byte[encryptBytes.Length];
                int plainCount = cryptoStream.Read(plainBytes, 0, plainBytes.Length);

                //복호화된 바이트 배열 String으로 변환
                string plainString = Encoding.UTF8.GetString(plainBytes, 0, plainCount);

                cryptoStream.Close();
                memoryStream.Close();

                return plainString;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return string.Empty;
            }
        }

        /// <summary>
        /// SHA256 단방향 암호화
        /// </summary>
        public string GenerateString(string inputStr)
        {
            using SHA256 sha256Hash = SHA256.Create();
            byte[] hash = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(inputStr));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in hash)
            {
                builder.AppendFormat("{0:x2}", b);
            }
            return builder.ToString();
        }
    }
}
