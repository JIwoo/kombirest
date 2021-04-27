using db.service;
using kombirest.service.Interface.Login;
using Model.kombirest.model.Login;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Service.Login
{
    public class LoginCreate: SqlExecute, ILoginCreate
    {
        public LoginLogModel.LoginLogResultContext SetLoginLog<T>(T context, out Int32 ErrCode, out String ErrMsg)
        {
            var resultContext = _Instance.ExecuteProcWithParamsSingle<LoginLogModel.LoginLogResultContext>("P_LoginLog_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
            return resultContext;
        }

        public LoginLogModel.LoginLogResultContext SetLoginLogDetail<T>(T context, out Int32 ErrCode, out String ErrMsg)
        {
            var result = SetLoginLog(context, out ErrCode, out ErrMsg);
            return result;
        }

        public bool SetJoin<T>(T context, out Int32 ErrCode, out String ErrMsg)
        {
            var resultContext = _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_MemberJoin_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
            return resultContext;
        }
    }
}
