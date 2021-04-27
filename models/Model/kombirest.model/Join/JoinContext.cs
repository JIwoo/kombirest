using Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Model.kombirest.model.Join
{
    public class JoinContext
    {
        /// <summary>
        /// 회원아이디 (이메일)
        /// </summary>
        public string MemberId { get; set; }

        /// <summary>
        /// 비밀번호
        /// </summary>
        public string MemberPwd { get; set; }

        /// <summary>
        /// 닉네임
        /// </summary>
        public string NickNm { get; set; }

        /// <summary>
        /// 회원성별 (1: 남자, 2: 여자)
        /// </summary>
        public GenderType Gender { get; set; } = 0;

        /// <summary>
        /// 회원나이
        /// </summary>
        public short Age { get; set; } = 0;

        /// <summary>
        /// 회원생일
        /// </summary>
        public short Birth { get; set; } = 0;

        /// <summary>
        /// 생년 
        /// </summary>
        //public string BirthYear { get; set; }

        /// <summary>
        /// 생월
        /// </summary>
        //public string BirthMonth { get; set; }

        /// <summary>
        /// 생일
        /// </summary>
        //public string BirthDate { get; set; }
    }
}
