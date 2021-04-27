using Model.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.kombirest.model.PwdReset
{
    /// <summary>
    /// [CASE 1]. 로그인 Off > 비밀번호 찾기
    /// [CASE 2]. 로그인 ON  > 비밀번호 변경
    /// -> [CASE 2] 의 경우 Email,AuthCode 값이 필요가 없기 때문에 기본값을 null로 할당 
    /// </summary>
    public class PwdResetContext
    {

        /// <summary>
        /// 아이디
        /// </summary>
        public string MemberId { get; set; } = string.Empty;

        /// <summary>
        /// 기존 비밀번호 
        /// </summary>
        public string OldPwd { get; set; } = string.Empty;

        /// <summary>
        /// 새 비밀번호
        /// </summary>
        public string Pwd { get; set; }

        /// <summary>
        /// 새 비밀번호 확인
        /// </summary>
        public string RePwd { get; set; }

        public PwdResetType ResetType { get; set; }
    }
}
