using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace db.service
{
    delegate string GetDbConnection(string type);

    public class SqlFactory
    {
        public Dictionary<ConnectionType, string> _setdbType = new Dictionary<ConnectionType, string>();
        public static SqlFactory _this = new SqlFactory();
        private static readonly string _AESKEY = "WkdRprotozldi$Rjwlfk~#@*(%&*(&(@n$n@$n(*&(@(ne^fgf(*&sww&!N(*#NR&*(8%04kfowjstprPgocnd!tlzlemfdi2*qufwhrgof!~%f&*f*^&tlqkfakdi#$";
        private GetDbConnection _connect { get; set; }

        protected Dictionary<ConnectionType, string> Instance(ConnectionType dbType)
        {
            //임시로 해놓음
            if (!_this._setdbType.ContainsKey(dbType))
            {
                string input;
                switch (dbType)
                //switch (ConnectionType.DBCon)
                {
                    case ConnectionType.DBCon:
                        string flag = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("DemoFlag")["Flag"].ToString();
                        if (flag == "Master")
                        {
                            input = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["LiveConnection"].ToString();                            
                        }
                        else
                        {
                            input = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["DefaultConnection"].ToString();
                        }
                        var dbConn = AESDecryptString(input);
                        _this._setdbType.Add(dbType, dbConn);
                        break;

                    case ConnectionType.DBConLive:
                        input = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["LiveConnection"].ToString();
                        _this._setdbType.Add(dbType, AESDecryptString(input));
                        break;
                }
            }

            return _this._setdbType;
        }

        private string GetDbConnection(GetDbConnection connect,string input)
        {
            return connect(input);
        }

        /// <summary>
        /// AES256 암호화
        /// </summary>
        /// <param name="InputString">평문</param>
        /// <returns></returns>
        private string AESEncryptString(string InputString)
        {
            try
            {
                string plainString = InputString.ToString();
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainString);
                byte[] Salt = Encoding.ASCII.GetBytes(_AESKEY.Length.ToString());

                PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(_AESKEY, Salt);
                RijndaelManaged RCipher = new RijndaelManaged();
                MemoryStream memoryStream = new MemoryStream();
                ICryptoTransform encrypt = RCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

                CryptoStream cryptoStream = new CryptoStream(memoryStream, encrypt, CryptoStreamMode.Write);
                cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                cryptoStream.FlushFinalBlock();

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
        private string AESDecryptString(string InputEncrypt)
        {
            try
            {
                string encryptString = InputEncrypt.ToString();

                byte[] encryptBytes = Convert.FromBase64String(encryptString);
                byte[] Salt = Encoding.ASCII.GetBytes(_AESKEY.Length.ToString());
                PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(_AESKEY, Salt);

                RijndaelManaged RCipher = new RijndaelManaged();

                MemoryStream memoryStream = new MemoryStream(encryptBytes);
                ICryptoTransform decrypt = RCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
                CryptoStream cryptoStream = new CryptoStream(memoryStream, decrypt, CryptoStreamMode.Read);

                byte[] plainBytes = new byte[encryptBytes.Length];
                int plainCount = cryptoStream.Read(plainBytes, 0, plainBytes.Length);

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
    }
}
