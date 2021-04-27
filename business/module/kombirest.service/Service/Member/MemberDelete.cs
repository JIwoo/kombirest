using db.service;
using kombirest.service.Interface.Member;
using Model.kombirest.model.Member;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Service.Member
{
    public class MemberDelete: SqlExecute, IMemberDelete
    {
        public MemberModel.MemberProfileResult GetMemberDelete<T>(T deleteContext, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 1. 삭제할 프로필사진 조회
            //--------------------------------
            var data = _Instance.ExecuteProcWithParamsSingle<MemberModel.MemberProfileResult>("P_MemberInfoDelete_GET", deleteContext, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
            return data;
        }

        public void SetFollowDisconnect<T>(T context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 1. 팔로우,팔로워 해제
            //--------------------------------
            _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_FollowDisconnect_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }
    }
}
