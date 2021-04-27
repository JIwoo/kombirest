using Model.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.kombirest.model.Login
{
    public class LoginResultContext
    {
        /// <summary>
        /// 회원아이디 (이메일)
        /// </summary>
        public string MemberId { get; set; }

        /// <summary>
        /// db에서 생성한 토큰값
        /// </summary>
        public string Guid { get; set; }

        public string NickNm { get; set; }

        public string Profile { get; set; }

        /// <summary>
        /// 회원상태 (1: 정상, 2: 마킹, 3: 정지, 4: 탈퇴)
        /// </summary>
        public MemberStateType State { get; set; }

    }
}
