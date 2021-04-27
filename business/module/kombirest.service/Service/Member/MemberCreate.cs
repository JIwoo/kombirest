using db.service;
using kombirest.service.Interface.Member;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Service.Member
{
    public class MemberCreate: SqlExecute, IMemberCreate
    {
        public bool SetAuthCode<T>(T context, out Int32 ErrCode, out String ErrMsg, out String AuthCode)
        {
            var resultContext = _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_AuthCode_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg, out AuthCode);
            return resultContext;
        }

        public void SetFollow<T>(T context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 1. 팔로우,팔로워 추가
            //--------------------------------
            _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_Follow_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }
    }
}
