using Model.kombirest.model.PwdReset;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.PwdReset
{
    public interface IPwdResetDao
    {
        bool SetPwdReset(PwdResetContext context, out Int32 ErrCode, out String ErrMsg);
    }
}
