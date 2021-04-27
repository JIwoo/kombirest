using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Interface.User
{
    public interface IUserCreate
    {
        bool SetUserActivityFriend<T>(T context, out Int32 ErrCode, out String ErrMsg);
    }
}
