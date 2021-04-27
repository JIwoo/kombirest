using System;
using System.Collections.Generic;
using System.Text;
using static Model.kombirest.model.User.UserModel;

namespace kombirest.service.Interface.User
{
    public interface IUserDto
    {
        List<UserSearchResult> GetUserList<T>(IUserRead read, T context, out int ErrCode, out string ErrMsg);

        List<UserMyFriendResult> GetMyFriendList<T>(IUserRead read, T context, out int ErrCode, out string ErrMsg);

        bool SetUserActivityFriend<T>(IUserCreate create, T context, out Int32 ErrCode, out String ErrMsg);
    }
}
