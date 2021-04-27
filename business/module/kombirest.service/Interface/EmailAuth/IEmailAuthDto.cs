using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Interface.EmailAuth
{
    public interface IEmailAuthDto
    {
        bool SetAuthCode<T>(IEmailAuthCreate create, T context, out Int32 ErrCode, out String ErrMsg, out String AuthCode);

        bool CheckAuthCode<T>(IEmailAuthRead read, T context, out Int32 ErrCode, out String ErrMsg);
    }
}
