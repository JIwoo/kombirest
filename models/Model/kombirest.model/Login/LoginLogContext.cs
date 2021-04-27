using Model.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.kombirest.model.Login
{
    public class LoginLogContext
    {
        /// <summary>
        /// 이메일
        /// </summary>
        public string MemberId { get; set; }

        /// <summary>
        /// 접속아이피 
        /// </summary>
        public string LoginIP { get; set; }

        /// <summary>
        /// 접속환경 정보 
        /// </summary>
        public string Agent { get; set; }

        /// <summary>
        /// 운영체제(OS)
        /// </summary>
        public string Platform { get; set; }

        /// <summary>
        /// 브라우저 타입 
        /// </summary>
        //public string BrowserType { get; set; }

        /// <summary>
        /// 1: 로그인 , 2: 로그아웃
        /// </summary>
        public LoginStateType State { get; set; }
    }
}
