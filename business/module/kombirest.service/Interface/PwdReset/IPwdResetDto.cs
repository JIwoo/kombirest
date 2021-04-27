using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Interface.PwdReset
{
    public interface IPwdResetDto
    {
        public bool SetPwdReset<T>(IPwdResetUpdate update, T context, out Int32 ErrCode, out String ErrMsg);
    }
}
