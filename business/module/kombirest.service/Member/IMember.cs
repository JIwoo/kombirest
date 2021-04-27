using System;
using System.Collections.Generic;
using System.Text;
using Model.kombirest.model.EmailAuth;
using Model.kombirest.model.Member;

namespace kombirest.service.Member
{
    public interface IMember
    {
        MemberModel.MyListResult GetMyList(MemberModel.MyListRequest context, out int ErrCode, out string ErrMsg);

        //List<MemberModel.ProductResult> GetUserList(MemberModel.UserListRequest context, out int ErrCode, out string ErrMsg);
        MemberModel.MyListResult GetUserList(MemberModel.UserListRequest context, out int ErrCode, out string ErrMsg);

        List<MemberModel.ProductResult> GetMyProduct(MemberModel.MyListRequest context, out int ErrCode, out string ErrMsg);

        List<MemberModel.MyFollowResult> GetFollow(MemberModel.MyFollowRequest context, out int ErrCode, out string ErrMsg);

        List<MemberModel.MyFollowResult> GetUserFollow(MemberModel.UserFollowRequest context, out int ErrCode, out string ErrMsg);

        bool SetAuthCode(EmailAuthModel.EmailAuthContext context, out Int32 ErrCode, out String ErrMsg, out String AuthCode);

        MemberModel.MemberInfoResult GetMemberInfo(MemberModel.MemberInfoContext context, out Int32 ErrCode, out String ErrMsg);
    }
}
