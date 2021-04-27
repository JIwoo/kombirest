using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Interface.Member
{
    public interface IMemberCreate
    {
        bool SetAuthCode<T>(T context, out Int32 ErrCode, out String ErrMsg, out String AuthCode);

        void SetFollow<T>(T context, out int ErrCode, out string ErrMsg);
    }
}
