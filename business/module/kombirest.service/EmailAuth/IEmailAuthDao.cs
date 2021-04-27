using Model.kombirest.model.EmailAuth;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.EmailAuth
{
    public interface IEmailAuthDao
    {
        /// <summary>
        /// 이메일 인증코드 생성
        /// </summary>
        bool CheckAuthCode(EmailAuthModel.EmailAuthCheckContext context, out Int32 ErrCode, out String ErrMsg);
    }
}
