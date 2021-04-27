using kombirest.service.Interface.Login;
using Model.kombirest.model.Login;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Service.Login
{
    public class LoginDto: ILoginDto
    {
        public LoginResultContext LoginResultDetail(ILoginRead read, LoginContext context, out Int32 ErrCode, out String ErrMsg)
        {
            return read.LoginResultDetail(context, out ErrCode, out ErrMsg);
        }

        public LoginLogModel.LoginLogResultContext SetLoginLogDetail<T>(ILoginCreate create, T context, out Int32 ErrCode, out String ErrMsg)
        {
            return create.SetLoginLogDetail(context, out ErrCode, out ErrMsg);
        }

        public bool SetJoin<T>(ILoginCreate create, T context, out Int32 ErrCode, out String ErrMsg)
        {
            return create.SetJoin(context, out ErrCode, out ErrMsg);
        }
    }
}
