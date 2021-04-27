using Model.kombirest.model.Login;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Interface.Login
{
    public interface ILoginRead
    {
        LoginResultContext GetLoginResult<T>(T context, out Int32 ErrCode, out String ErrMsg);

        LoginResultContext LoginResultDetail(LoginContext context, out Int32 ErrCode, out String ErrMsg);
    }
}
