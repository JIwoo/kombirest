using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Interface.EmailAuth
{
    public interface IEmailAuthRead
    {
        bool CheckAuthCode<T>(T context, out Int32 ErrCode, out String ErrMsg);
    }
}
