using Model.kombirest.model.Login;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Interface.Login
{
    public interface ILoginDto
    {
        LoginResultContext LoginResultDetail(ILoginRead read, LoginContext context, out Int32 ErrCode, out String ErrMsg);

        LoginLogModel.LoginLogResultContext SetLoginLogDetail<T>(ILoginCreate create, T context, out Int32 ErrCode, out String ErrMsg);

        bool SetJoin<T>(ILoginCreate create, T context, out Int32 ErrCode, out String ErrMsg);
    }
}
