using Model.Enum;
using Model.kombirest.model.EmailAuth;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Commons
{
    public interface IInfo
    {

        void CallSMTP(string toEmail, string title, string content, out Int32 ErrCode, out String ErrMsg);

        void SendEmailOfAuthCode(SendEmailContext context, out Int32 ErrCode, out String ErrMsg);

        public string GetAuthForm(string ServerMapPath, AuthType AuthType);
    }
}
