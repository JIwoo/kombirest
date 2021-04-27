using db.service;
using kombirest.service.Interface.User;
using System;
using System.Collections.Generic;
using System.Text;
using static Model.kombirest.model.User.UserModel;

namespace kombirest.service.Service.User
{
    public class UserRead: SqlExecute, IUserRead
    {
        public List<UserSearchResult> GetUserList<T>(T context, out int ErrCode, out string ErrMsg)
        {
            return _Instance.ExecuteProcWithParams<UserSearchResult>("P_UserSearch_GET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }

        public List<UserMyFriendResult> GetMyFriendList<T>(T context, out int ErrCode, out string ErrMsg)
        {
            return _Instance.ExecuteProcWithParams<UserMyFriendResult>("P_UserMyList_GET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }
    }
}
