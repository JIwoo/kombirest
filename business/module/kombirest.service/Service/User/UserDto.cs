using kombirest.service.Interface.User;
using System;
using System.Collections.Generic;
using System.Text;
using static Model.kombirest.model.User.UserModel;

namespace kombirest.service.Service.User
{
    public class UserDto: IUserDto
    {
        #region read
        public List<UserSearchResult> GetUserList<T>(IUserRead read, T context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 회원 닉네임 조회
            //--------------------------------
            return read.GetUserList(context, out ErrCode, out ErrMsg);
        }

        public List<UserMyFriendResult> GetMyFriendList<T>(IUserRead read, T context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 내가초대한 회원들 조회
            //--------------------------------
            return read.GetMyFriendList(context, out ErrCode, out ErrMsg);
        }
        #endregion

        #region create
        public bool SetUserActivityFriend<T>(IUserCreate create, T context, out Int32 ErrCode, out String ErrMsg)
        {
            //--------------------------------
            // 회원초대 
            //--------------------------------
            return create.SetUserActivityFriend(context, out ErrCode, out ErrMsg);
        }
        #endregion
    }
}
