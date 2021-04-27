using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace kombirest.service.Interface.Commons
{
    public interface IAuth
    {
        string SetJwt(string memberId, string uuid = "");

        string SetJwt(int day, string memberId);

        IDictionary<string, object> GetJwtPayload(string token, out Int32 ErrCode, out String ErrMsg, out String NewToken);

        IDictionary<string, object> GetJwtPayload(string accessToken, string refreshToken, out Int32 ErrCode, out String ErrMsg, out String NewToken);

        IDictionary<string, object> GetRefreshToken(string refreshToken, out Int32 ErrCode, out String ErrMsg);

        IDictionary<string, object> GetAccessToken(string accessToken, out Int32 ErrCode, out String ErrMsg);


        bool GetGuidToken(string memberId, string guid, out int ErrCode, out string ErrMsg);

        Task<string> SetFireBaseToken(string memberid);

        Task<(string, string, int)> GetFireBaseToken(string accessToken);
    }
}
