using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Interface.Member
{
    public interface IMemberUpdate
    {
        void SetMemberInfo<T>(T context, out int ErrCode, out string ErrMsg);
    }
}
