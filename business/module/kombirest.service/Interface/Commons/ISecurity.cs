using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Interface.Commons
{
    public interface ISecurity
    {
        string EncryptString(string InputString);

        string DecryptString(string InputEncrypt);

        string GenerateString(string inputStr);
    }
}
