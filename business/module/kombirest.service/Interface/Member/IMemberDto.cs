using Model.kombirest.model.Member;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Interface.Member
{
    public interface IMemberDto
    {
        MemberModel.MyListResult GetMyList<T>(IMemberRead read, T context, out int ErrCode, out string ErrMsg);

        List<MemberModel.ProductResult> GetMyProduct<T>(IMemberRead read, T context, out int ErrCode, out string ErrMsg);

        MemberModel.MyListResult GetUserList<T>(IMemberRead read, T context, out int ErrCode, out string ErrMsg);

        List<MemberModel.MyFollowResult> GetFollow<T>(IMemberRead read, T context, out int ErrCode, out string ErrMsg);

        List<MemberModel.MyFollowResult> GetUserFollow<T>(IMemberRead read, T context, out int ErrCode, out string ErrMsg);

        MemberModel.MemberInfoResult GetMemberInfo<T>(IMemberRead read, T context, out Int32 ErrCode, out String ErrMsg);

        bool SetAuthCode<T>(IMemberCreate create, T context, out Int32 ErrCode, out String ErrMsg, out String AuthCode);

        void SetFollow<T>(IMemberCreate create, T context, out int ErrCode, out string ErrMsg);

        void SetMemberInfo(MemberModel.MemberInfosRequest request, out int ErrCode, out string ErrMsg);

        void SetFollowDisconnect<T>(IMemberDelete delete, T context, out int ErrCode, out string ErrMsg);
    }
}
