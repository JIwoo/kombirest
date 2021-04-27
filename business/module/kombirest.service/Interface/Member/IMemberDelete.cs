using Model.kombirest.model.Member;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Interface.Member
{
    public interface IMemberDelete
    {
        MemberModel.MemberProfileResult GetMemberDelete<T>(T deleteContext, out int ErrCode, out string ErrMsg);

        void SetFollowDisconnect<T>(T context, out int ErrCode, out string ErrMsg);
    }
}
