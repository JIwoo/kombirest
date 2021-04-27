using System;
using System.Collections.Generic;
using System.Text;
using db.service;
using kombirest.service.Interface.EmailAuth;

namespace kombirest.service.Service.EmailAuth
{
    public class EmailAuthRead: SqlExecute, IEmailAuthRead
    {
        public bool CheckAuthCode<T>(T context, out Int32 ErrCode, out String ErrMsg)
        {
            var resultContext = _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_CheckAuthResult_GET", context, Instance(ConnectionType.DBCon),
                                                                                        out ErrCode, out ErrMsg);

            return resultContext;
        }
    }
}
