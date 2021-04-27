using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Commons
{
    public interface ISecurity
    {
        /// <summary>
        /// AES256 암호화
        /// </summary>
        /// <param name="InputString">평문</param>
        /// <returns></returns>
        string AESEncryptString(string InputString);

        /// <summary>
        /// AES256 복호화
        /// </summary>
        /// <param name="InputEncrypt">암호문</param>
        /// <returns></returns>
        string AESDecryptString(string InputEncrypt);

        /// <summary>
        /// SHA256 단방향 암호화
        /// </summary>
        string GenerateSHA256String(string inputStr);
    }
}
