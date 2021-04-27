using System;
using System.Collections.Generic;
using System.Text;
using db.service;
using kombirest.service.Interface.EmailAuth;

namespace kombirest.service.Service.EmailAuth
{
    public class EmailAuthCreate: SqlExecute, IEmailAuthCreate
    {
        public bool SetAuthCode<T>(T context, out Int32 ErrCode, out String ErrMsg, out String AuthCode)
        {
            var resultContext = _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_AuthCode_SET", context, Instance(ConnectionType.DBCon),
                                                                                        out ErrCode, out ErrMsg, out AuthCode);

            return resultContext;
        }
    }
}
