using Model.kombirest.model.Login;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Interface.Login
{
    public interface ILoginCreate
    {
        LoginLogModel.LoginLogResultContext SetLoginLog<T>(T context, out Int32 ErrCode, out String ErrMsg);

        LoginLogModel.LoginLogResultContext SetLoginLogDetail<T>(T context, out Int32 ErrCode, out String ErrMsg);

        bool SetJoin<T>(T context, out Int32 ErrCode, out String ErrMsg);
    }
}
