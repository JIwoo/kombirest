using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Interface.PwdReset
{
    public interface IPwdResetUpdate
    {
        bool SetPwdReset<T>(T context, out Int32 ErrCode, out String ErrMsg);
    }
}
