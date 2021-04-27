using Model.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.kombirest.model.EmailAuth
{
    /// <summary>
    /// 이메일 인증 전송 모델
    /// </summary>
    public class SendEmailContext
    {
        /// <summary>
        /// 회원아이디
        /// </summary>
        public string MemberId { get; set; }

        /// <summary>
        /// 인증코드
        /// </summary>
        public string AuthCode { get; set; }

        // 파일을 다루기 위한 절대경로 값 
        public string ServerMapPath { get; set; }

        public AuthType AuthType { get; set; }
    }
}
