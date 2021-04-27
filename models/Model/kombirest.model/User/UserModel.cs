using System;
using System.Collections.Generic;
using System.Text;

namespace Model.kombirest.model.User
{
    public class UserModel
    {
        public class UserSearchRequest
        {
            public string Nick { get; set; }
        }

        public class UserSearchResult
        {
            public string Nick { get; set; }

            public string Profile { get; set; }

            public string ToUser { get; set; }
        }

        public class UserMyFriendResult
        {
            public string Nick { get; set; }

            public short InviteState { get; set; }

            public string Profile { get; set; }

            public string ToUser { get; set; }
        }

        public class UserFriendAddContext
        {
            public string MemberId { get; set; }

            public string TokenGuid { get; set; }

            public string Nick { get; set; }
        }

        public class UserFriendListContext
        {
            public string MemberId { get; set; }

            public string TokenGuid { get; set; }
        }
    }
}
