using kombirest.service.Interface.PwdReset;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Service.PwdReset
{
    public class PwdResetDto: IPwdResetDto
    {
        public bool SetPwdReset<T>(IPwdResetUpdate update, T context, out Int32 ErrCode, out String ErrMsg)
        {
            return update.SetPwdReset(context, out ErrCode, out ErrMsg);
        }
    }
}
