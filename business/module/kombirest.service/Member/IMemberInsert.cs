using Model.kombirest.model.Member;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Member
{
    public interface IMemberInsert
    {
        void SetFollow(MemberModel.MyFollowSetRequest context, out int ErrCode, out string ErrMsg);        
    }
}
