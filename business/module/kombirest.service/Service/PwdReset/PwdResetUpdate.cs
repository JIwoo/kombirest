using db.service;
using kombirest.service.Interface.PwdReset;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Service.PwdReset
{
    public class PwdResetUpdate: SqlExecute, IPwdResetUpdate
    {
        public bool SetPwdReset<T>(T context, out Int32 ErrCode, out String ErrMsg)
        {
            var resultContext = _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_PwdReset_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);

            return resultContext;
        }
    }
}
