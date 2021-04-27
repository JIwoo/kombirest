using db.service;
using kombirest.service.Interface.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Service.User
{
    public class UserCreate: SqlExecute, IUserCreate
    {
        public bool SetUserActivityFriend<T>(T context, out Int32 ErrCode, out String ErrMsg)
        {
            var resultContext = _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_UserActivityFriend_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
            return resultContext;
        }
    }
}
