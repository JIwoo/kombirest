using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using db.service;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Exceptions;
using kombirest.service.Interface.Commons;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Model.kombirest.model.Common;

namespace kombirest.service.Service.Common
{
    public class Auth : SqlExecute, IAuth
    {
        private GetJwt _jwt { get; set; }
        private static readonly string _secret = "FZDqslOpk0AFjCZiXOYg2BmnJ1CVZuQfdMNvPLasgk^^300g220v@#b=30vksk;d7893j@$#B$#";
        private static readonly string _refreshSecret = "fkal@gop[wqejV[Q{FKZJwkwlGHJW@JvmWWfz)#@*WEVJ#!WRU(RGJkb9i46H$H!@S+WK";        

        private IDictionary<string, object> GetJwt(GetJwt Jwt, string input, out int code, out string msg)
        {
            return Jwt(input, out code, out msg);
        }

        public string SetJwt(string memberId, string uuid = "")
        {
            if (string.IsNullOrEmpty(uuid))
            {
                uuid = Guid.NewGuid().ToString();
            }
            var context = new JwtModelResult
            {
                MemberId = memberId,
                TokenGuid = uuid
            };

            var token = new JwtBuilder()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(_secret)
            .Subject("kombirest.com")
            .AddClaim("exp", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds())
            .AddClaim("id", memberId)
            .AddClaim("uuid", uuid)
            .Encode();

            var result = _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_MemberToken_SET", context, Instance(ConnectionType.DBCon), out int ErrCode, out string ErrMsg);
            if (!result)
            {
                token = string.Empty;
            }
            return token;
        }

        //refreshToken
        public string SetJwt(int day, string memberid)
        {
            day = 14;
            string uuid = Guid.NewGuid().ToString();
            Int64 exp = Convert.ToInt64(DateTimeOffset.UtcNow.AddDays(day).ToUnixTimeSeconds().ToString());
            //Int64 exp = Convert.ToInt64(DateTimeOffset.UtcNow.AddHours(6).ToUnixTimeSeconds().ToString());
            var token = new JwtBuilder()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(_refreshSecret)
            .Subject("kombirest.com")
            .AddClaim("exp", exp)
            .AddClaim("id", memberid)
            .AddClaim("uuid", uuid)
            .Encode();

            var context = new JwtRefreshResult
            {
                MemberId = memberid,
                TokenGuid = uuid,
                ExpDate = exp
            };
            var result = _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_MemberRefreshToken_SET", context, Instance(ConnectionType.DBCon), out int ErrCode, out string ErrMsg);
            if (!result)
            {
                token = string.Empty;
            }
            return token;
        }

        public async Task<string> SetFireBaseToken(string memberid)
        {
            //var asd = new ConfigurationBuilder().AddJsonFile("kombirestFireBase.json").Build().
            //FirebaseApp.Create(new AppOptions()
            //{
            //    Credential = GoogleCredential.FromFile("/kombirestFireBase.json")
            //});

            ISecurity security = new Security256();
            var uid = security.EncryptString(memberid);
            //var uid = memberid;
            string uuid = Guid.NewGuid().ToString();
            var additionalClaims = new Dictionary<string, object>()
            {
                { "uuid", uuid },
                { "id", memberid }
            };

            string customToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(uid, additionalClaims);


            var context = new JwtModelResult
            {
                MemberId = memberid,
                TokenGuid = uuid
            };
            var result = _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_MemberToken_SET", context, Instance(ConnectionType.DBCon), out int ErrCode, out string ErrMsg);
            return customToken;
        }

        public async Task<(string,string,int)> GetFireBaseToken(string accessToken)
        {
            string id = string.Empty, uuid = string.Empty;
            int code = 0;
            try
            {
                //FirebaseApp.Create(new AppOptions()
                //{
                //    Credential = GoogleCredential.FromFile("/kombirestFireBase.json")
                //});

                //bool checkRevoked = true;
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(accessToken);

                //id = decodedToken.Claims["id"].ToString();
                //foreach (KeyValuePair<string, object> items in decodedToken.Claims)
                //{
                //    if (items.Key.ToString() == "id")
                //    {
                //        id = items.Value.ToString();
                //    }
                //    if (items.Key.ToString() == "uuid")
                //    {
                //        uuid = items.Value.ToString();
                //    }
                //}
                id = decodedToken.Claims["id"].ToString(); 
                uuid = decodedToken.Claims["uuid"].ToString();
            }
            catch (FirebaseAuthException ex)
            {
                if (ex.AuthErrorCode == AuthErrorCode.RevokedIdToken)
                {
                    code = -4001;
                    uuid = ex.Message.ToString();
                }
                else if(ex.AuthErrorCode == AuthErrorCode.ExpiredIdToken)
                {
                    code = -4002;
                    uuid = ex.Message.ToString();
                }
                else
                {
                    code = -4003;
                    uuid = ex.Message.ToString();
                }
            }
            return (id, uuid, code);
        }

        public IDictionary<string, object> GetRefreshToken(string refreshToken, out Int32 ErrCode, out String ErrMsg)
        {
            IDictionary<string, object> token = new Dictionary<string, object>();
            try
            {
                ErrCode = 0;
                ErrMsg = "서명완료";

                token = new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(_refreshSecret)
                .MustVerifySignature()
                .Decode<IDictionary<string, object>>(refreshToken);

                if (token["sub"].ToString() != "kombirest.com" || string.IsNullOrEmpty(token["id"].ToString()))
                {
                    ErrCode = -4004;
                    ErrMsg = "토큰 값을 읽는데 실패하였습니다.";
                }

                var context = new JwtModelContext
                {
                    MemberId = token["id"].ToString()
                };
                var result = _Instance.ExecuteProcWithParamsSingle<JwtRefreshResult>("P_MemberRefreshToken_GET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);

            }
            catch (TokenExpiredException)
            {
                ErrCode = -4005;
                ErrMsg = "토큰 유효시간이 만료되었습니다.";
            }
            catch (SignatureVerificationException)
            {
                ErrCode = -4006;
                ErrMsg = "토큰 서명이 유효하지 않습니다.";
            }
            catch (Exception e)
            {
                ErrCode = -4009;
                ErrMsg = e.Message.ToString();
            }

            return token;
        }

        public IDictionary<string, object> GetAccessToken(string accessToken, out Int32 ErrCode, out String ErrMsg)
        {
            IDictionary<string, object> token = new Dictionary<string, object>();
            ErrCode = 0;
            ErrMsg = "서명완료";
            try
            {
                token = new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(_secret)
                .MustVerifySignature()
                .Decode<IDictionary<string, object>>(accessToken);

                if (token["sub"].ToString() != "kombirest.com" && string.IsNullOrEmpty(token["id"].ToString()) || string.IsNullOrEmpty(token["uuid"].ToString()))
                {
                    ErrCode = -4001;
                    ErrMsg = "토큰 값을 읽는데 실패하였습니다.";
                }
            }
            catch (TokenExpiredException)
            {
                ErrCode = -4002;
                ErrMsg = "토큰 유효시간이 만료되었습니다.";
            }
            catch (SignatureVerificationException)
            {
                ErrCode = -4003;
                ErrMsg = "토큰 서명이 유효하지 않습니다.";
            }
            catch (Exception e)
            {
                ErrCode = -4009;
                ErrMsg = e.Message.ToString();
            }

            return token;
        }

        public IDictionary<string, object> GetJwtPayload(string auth, out Int32 ErrCode, out String ErrMsg, out String NewToken)
        {
            NewToken = string.Empty;
            var tokens = auth.Split(",");
            string accessToken = tokens[0].ToString();
            string refreshToken = tokens.Count() > 1 ? tokens[1].ToString() : string.Empty;
            IDictionary<string, object> jsonToken = new Dictionary<string, object>();

            ErrMsg = string.Empty;
            ErrCode = 0;
            //_jwt = GetAccessToken;
            //jsonToken = GetJwt(_jwt, accessToken, out ErrCode, out ErrMsg);

            var decodeToken = GetFireBaseToken(accessToken);
            decodeToken.Wait();
            //Task.WaitAll(decodeToken);
            if(decodeToken.Result.Item3 != 0)
            {
                ErrMsg = "토큰서명fffff이 유효하지 않습니다.";
            }
            if(decodeToken.Result.Item3 == -4002)
            //if (ErrCode == -4002)
            {
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    _jwt = GetRefreshToken;
                    jsonToken = GetJwt(_jwt, refreshToken, out ErrCode, out ErrMsg);
                    if (ErrCode == 0 && !string.IsNullOrEmpty(jsonToken["id"].ToString()))
                    {
                        //string newAccessToken = SetJwt(jsonToken["id"].ToString());
                        //NewToken = newAccessToken;
                        //_jwt = GetAccessToken;
                        //jsonToken = GetJwt(_jwt, newAccessToken, out ErrCode, out ErrMsg);

                        var newAccessToken = SetFireBaseToken(jsonToken["id"].ToString());
                        newAccessToken.Wait();
                        NewToken = newAccessToken.Result.ToString();
                        decodeToken = GetFireBaseToken(NewToken);
                        decodeToken.Wait();
                    }
                }
            }


            jsonToken.Add("id", decodeToken.Result.Item1.ToString());
            jsonToken.Add("uuid", decodeToken.Result.Item2.ToString());
            return jsonToken;
        }

        public IDictionary<string, object> GetJwtPayload(string accessToken, string refreshToken, out Int32 ErrCode, out String ErrMsg, out String NewToken)
        {
            NewToken = string.Empty;
            IDictionary<string, object> jsonToken = new Dictionary<string, object>();
            //_jwt = GetAccessToken;
            //jsonToken = GetJwt(_jwt, accessToken, out ErrCode, out ErrMsg);
            //ErrMsg = string.Empty;
            var decodeToken = GetFireBaseToken(accessToken);
            decodeToken.Wait();

            ErrCode = decodeToken.Result.Item3;
            ErrMsg = decodeToken.Result.Item2.ToString();

            if(ErrCode == 0)
            {
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    _jwt = GetRefreshToken;
                    GetJwt(_jwt, refreshToken, out ErrCode, out ErrMsg);
                }
                else
                {
                    ErrCode = -4004;
                }
            }

            //if (ErrCode == -4002)
            //{
            //    if (!string.IsNullOrEmpty(refreshToken))
            //    {
            //        //_jwt = GetRefreshToken;
            //        //jsonToken = GetJwt(_jwt, refreshToken, out ErrCode, out ErrMsg);
            //        //if (ErrCode == 0 && !string.IsNullOrEmpty(jsonToken["id"].ToString()))
            //        //{
            //        //    string newAccessToken = SetJwt(jsonToken["id"].ToString());
            //        //    NewToken = newAccessToken;
            //        //    _jwt = GetAccessToken;
            //        //    jsonToken = GetJwt(_jwt, newAccessToken, out ErrCode, out ErrMsg);
            //        //}
            //    }
            //}
            //jsonToken.Add("id", decodeToken.Result.Item1);
            //jsonToken.Add("uuid", decodeToken.Result.Item2);



            jsonToken["id"] = decodeToken.Result.Item1;
            jsonToken["uuid"] = decodeToken.Result.Item2;
            //ErrCode = -4002;
            //ErrMsg = jsonToken["uuid"].ToString();
            return jsonToken;
        }

        public bool GetGuidToken(string memberId, string guid, out int ErrCode, out string ErrMsg)
        {
            bool result = true;
            var context = new JwtModelContext
            {
                MemberId = memberId
            };
            var data = _Instance.ExecuteProcWithParamsSingle<JwtModelResult>("P_MemberToken_GET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
            if (data.TokenGuid != guid)
            {
                ErrCode = -4008;
                ErrMsg = "토큰 서명이 맞지 않습니다.";
                result = false;
            }
            return result;
        }
    }
}
