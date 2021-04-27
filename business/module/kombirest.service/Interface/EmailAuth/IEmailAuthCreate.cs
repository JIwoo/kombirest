using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Interface.EmailAuth
{
    public interface IEmailAuthCreate
    {
        bool SetAuthCode<T>(T context, out Int32 ErrCode, out String ErrMsg, out String AuthCode);
    }
}
