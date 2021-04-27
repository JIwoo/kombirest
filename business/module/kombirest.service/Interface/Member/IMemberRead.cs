using Model.kombirest.model.Member;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Interface.Member
{
    public interface IMemberRead
    {
        MemberModel.MyListResult GetMyList<T>(T context, out int ErrCode, out string ErrMsg);

        List<MemberModel.ProductResult> GetMyProduct<T>(T context, out int ErrCode, out string ErrMsg);

        MemberModel.MyListResult GetUserList<T>(T context, out int ErrCode, out string ErrMsg);

        List<MemberModel.MyFollowResult> GetFollow<T>(T context, out int ErrCode, out string ErrMsg);

        List<MemberModel.MyFollowResult> GetUserFollow<T>(T context, out int ErrCode, out string ErrMsg);

        MemberModel.MemberInfoResult GetMemberInfo<T>(T context, out Int32 ErrCode, out String ErrMsg);
    }
}
