using System;
using System.Collections.Generic;
using System.Text;
using kombirest.service.Interface.EmailAuth;

namespace kombirest.service.Service.EmailAuth
{
    public class EmailAuthDto: IEmailAuthDto
    {
        public bool SetAuthCode<T>(IEmailAuthCreate create, T context, out Int32 ErrCode, out String ErrMsg, out String AuthCode)
        {
            return create.SetAuthCode(context, out ErrCode, out ErrMsg, out AuthCode);
        }

        public bool CheckAuthCode<T>(IEmailAuthRead read, T context, out Int32 ErrCode, out String ErrMsg)
        {
            return read.CheckAuthCode(context, out ErrCode, out ErrMsg);
        }
    }
}
