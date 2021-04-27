using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using db.service;
using kombirest.service.Interface.Member;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace kombirest.service.Service.Member
{
    public class MemberUpdate: SqlExecute, IMemberUpdate
    {
        public void SetMemberInfo<T>(T context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 프로필수정
            //--------------------------------
            _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_MemberInfo_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }
    }
}
