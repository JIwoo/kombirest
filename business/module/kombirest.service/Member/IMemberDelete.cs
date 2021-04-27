using Model.kombirest.model.Member;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Member
{
    public interface IMemberDelete
    {
        void SetFollowDisconnect(MemberModel.MyFollowDisconnectRequest context, out int ErrCode, out string ErrMsg);
    }
}
