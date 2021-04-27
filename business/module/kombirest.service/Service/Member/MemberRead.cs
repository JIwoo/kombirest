using db.service;
using kombirest.service.Interface.Member;
using Model.kombirest.model.Member;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Service.Member
{
    public class MemberRead: SqlExecute, IMemberRead
    {
        public MemberModel.MyListResult GetMyList<T>(T context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 1. 나의 게시물리스트 조회
            // 2. 팔로워,팔로잉 정보
            //--------------------------------
            var result = new MemberModel.MyListResult();

            var List = _Instance.ExecuteProcWithMultiple("P_MyList_GET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);

            if (List[0] != null) result.Info = JsonConvert.DeserializeObject<List<MemberModel.InfoResult>>(List[0])[0];
            if (List[1] != null) result.Product = JsonConvert.DeserializeObject<List<MemberModel.ProductResult>>(List[1]);

            return result;
        }

        public List<MemberModel.ProductResult> GetMyProduct<T>(T context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 1. 나의 게시물리스트 조회
            //--------------------------------
            return _Instance.ExecuteProcWithParams<MemberModel.ProductResult>("P_MyProduct_GET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }

        public MemberModel.MyListResult GetUserList<T>(T context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 1. 유저정보 ( 팔로우,팔로워 등도 포함?? )       
            // 2. 유저의 게시물리스트
            //--------------------------------
            var result = new MemberModel.MyListResult();

            var List = _Instance.ExecuteProcWithMultiple("P_FollowUserList_GET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);

            if (List[0] != null) result.Info = JsonConvert.DeserializeObject<List<MemberModel.InfoResult>>(List[0])[0];
            if (List[1] != null) result.Product = JsonConvert.DeserializeObject<List<MemberModel.ProductResult>>(List[1]);

            return result;
        }

        public List<MemberModel.MyFollowResult> GetFollow<T>(T context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 1. 팔로잉 게시물 조회
            //--------------------------------
            return _Instance.ExecuteProcWithParams<MemberModel.MyFollowResult>("P_Follow_GET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }

        public List<MemberModel.MyFollowResult> GetUserFollow<T>(T context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 1. 팔로잉 게시물 조회
            //--------------------------------
            return _Instance.ExecuteProcWithParams<MemberModel.MyFollowResult>("P_UserFollow_GET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }

        public MemberModel.MemberInfoResult GetMemberInfo<T>(T context, out Int32 ErrCode, out String ErrMsg)
        {
            //--------------------------------
            // 1. 회원 프로필 조회
            //--------------------------------
            return _Instance.ExecuteProcWithParamsSingle<MemberModel.MemberInfoResult>("P_MemberInfo_GET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }
    }
}
