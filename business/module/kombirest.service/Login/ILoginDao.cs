using Model.kombirest.model.Join;
using Model.kombirest.model.Login;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Login
{
    public interface ILoginDao
    {
        /// <summary>
        /// 회원 로그인
        /// </summary>
        LoginResultContext GetLoginResult(LoginContext context, out Int32 ErrCode, out String ErrMsg);

        LoginLogModel.LoginLogResultContext SetLoginLog(LoginLogModel.LoginLogContext context, out Int32 ErrCode, out String ErrMsg);

        //void LoginResultDetail(LoginContext context, out Int32 ErrCode, out String ErrMsg);
        LoginResultContext LoginResultDetail(LoginContext context, out Int32 ErrCode, out String ErrMsg);
        LoginLogModel.LoginLogResultContext SetLoginLogDetail(LoginLogModel.LoginLogContext context, out Int32 ErrCode, out String ErrMsg);

        bool SetJoin(JoinContext context, out Int32 ErrCode, out String ErrMsg);
    }
}
