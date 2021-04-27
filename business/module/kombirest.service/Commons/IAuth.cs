using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Commons
{
    public interface IAuth
    {
        /// <summary>
        /// 토큰 생성하기 
        /// </summary>
        /// <param name="Email">사용자 이메일</param>
        string SetJwt(string memberId, string uuid = "");

        string SetJwt(int day, string memberId);


        /// <summary>
        /// 토큰 안에 있는 데이터 가져오기 
        /// </summary>
        IDictionary<string, object> GetJwtPayload(string token, out Int32 ErrCode, out String ErrMsg, out String NewToken);

        IDictionary<string, object> GetRefreshToken(string refreshToken, out Int32 ErrCode, out String ErrMsg);

        IDictionary<string, object> GetAccessToken(string accessToken, out Int32 ErrCode, out String ErrMsg);


        bool GetGuidToken(string memberId, string guid, out int ErrCode, out string ErrMsg);
    }
}
