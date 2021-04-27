using System;
using System.Collections.Generic;
using System.Text;

namespace Model.kombirest.model.Login
{
    public class LoginContext
    {
        /// <summary>
        /// 회원 아이디
        /// </summary>
        public string MemberId { get; set; }

        /// <summary>
        /// 회원 비밀번호
        /// </summary>
        public string MemberPwd { get; set; }

        /// <summary>
        /// 자동 로그인 여부
        /// </summary>
        public bool AutoLogin { get; set; }

       public class AutoLoginContext
        {
            public string Token { get; set; }
        }
    }
}
