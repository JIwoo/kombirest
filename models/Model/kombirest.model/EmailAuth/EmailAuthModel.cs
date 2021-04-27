using Model.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.kombirest.model.EmailAuth
{
    public class EmailAuthModel
    {
        public class EmailAuthContext
        {
            /// <summary>
            /// 회원아이디
            /// </summary>
            public string MemberId { get; set; }

            public AuthType AuthType { get; set; }
        }

        public class EmailAuthCheckContext
        {
            /// <summary>
            /// 회원아이디
            /// </summary>
            public string MemberId { get; set; }

            /// <summary>
            /// 이메일 인증 코드
            /// </summary>
            public string AuthCode { get; set; }

            public AuthType AuthType { get; set; }
        }
    }
}
