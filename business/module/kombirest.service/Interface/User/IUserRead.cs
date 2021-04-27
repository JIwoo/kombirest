using System;
using System.Collections.Generic;
using System.Text;
using static Model.kombirest.model.User.UserModel;

namespace kombirest.service.Interface.User
{
    public interface IUserRead
    {
        List<UserSearchResult> GetUserList<T>(T context, out int ErrCode, out string ErrMsg);

        List<UserMyFriendResult> GetMyFriendList<T>(T context, out int ErrCode, out string ErrMsg);
    }
}
