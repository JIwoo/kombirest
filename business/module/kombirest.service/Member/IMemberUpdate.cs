using Model.kombirest.model.Member;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Member
{
    public interface IMemberUpdate
    {
        void SetMemberInfo(MemberModel.MemberInfoRequest context, out int ErrCode, out string ErrMsg);
    }
}
