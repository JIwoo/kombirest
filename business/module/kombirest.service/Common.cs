using db.service;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Exceptions;
using kombirest.service.Commons;
using Microsoft.AspNetCore.Http;
using Model.Enum;
using Model.kombirest.model.Common;
using Model.kombirest.model.EmailAuth;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace kombirest.service
{
    delegate IDictionary<string, object> GetJwt(string input, out int code, out string msg);

    public class Common : SqlExecute
    {
        //private GetJwt _jwt { get; set; }
        //private static readonly string _AESKEY = "WkdRprotozldi$Rjwlfk~#@*(%&*(&(@n$n@$n(*&(@(ne^fgf(*&sww&!N(*#NR&*(8%04kfowjstprPgocnd!tlzlemfdi2*qufwhrgof!~%f&*f*^&tlqkfakdi#$";
        //private static readonly string _Secret = "FZDqslOpk0AFjCZiXOYg2BmnJ1CVZuQfdMNvPLasgk^^300g220v@#b=30vksk;d7893j@$#B$#";
        //private static readonly string _refreshSecret = "fkal@gop[wqejV[Q{FKZJwkwlGHJW@JvmWWfz)#@*WEVJ#!WRU(RGJkb9i46H$H!@S+WK";
        //private readonly IHttpContextAccessor _accessor;
        //public  Common(IHttpContextAccessor Accessor)
        //{
        //    _accessor = Accessor;
        //}

        //#region 암호화/복호화 로직        

        ///// <summary>
        ///// AES256 암호화
        ///// </summary>
        ///// <param name="InputString">평문</param>
        ///// <returns></returns>
        //public string AESEncryptString(string InputString)
        //{
        //    try
        //    {
        //        string plainString = InputString.ToString();

        //        //평문 바이트로 변환
        //        byte[] plainBytes = Encoding.UTF8.GetBytes(plainString);
        //        byte[] Salt = Encoding.ASCII.GetBytes(_AESKEY.Length.ToString());
        //        PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(_AESKEY, Salt);

        //        //린달 알고리즘
        //        RijndaelManaged RCipher = new RijndaelManaged();

        //        //메모리스트림 생성
        //        MemoryStream memoryStream = new MemoryStream();
        //        ICryptoTransform encrypt = RCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

        //        //키와 백터값으로 암호화 스트림 생성
        //        CryptoStream cryptoStream = new CryptoStream(memoryStream, encrypt, CryptoStreamMode.Write);
        //        cryptoStream.Write(plainBytes, 0, plainBytes.Length);
        //        cryptoStream.FlushFinalBlock();

        //        //메모리스트림에 담겨있는 암호바이트 배열을 담음
        //        byte[] encryptBytes = memoryStream.ToArray();
        //        string encryptString = Convert.ToBase64String(encryptBytes);

        //        cryptoStream.Close();
        //        memoryStream.Close();

        //        return encryptString;
        //    }
        //    catch (Exception) { return string.Empty; }
        //}

        ///// <summary>
        ///// AES256 복호화
        ///// </summary>
        ///// <param name="InputEncrypt">암호문</param>
        ///// <returns></returns>
        //public string AESDecryptString(string InputEncrypt)
        //{
        //    try
        //    {
        //        string encryptString = InputEncrypt.ToString();

        //        //암호문 바이트로 변환
        //        byte[] encryptBytes = Convert.FromBase64String(encryptString);
        //        byte[] Salt = Encoding.ASCII.GetBytes(_AESKEY.Length.ToString());
        //        PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(_AESKEY, Salt);

        //        //린달 알고리즘
        //        RijndaelManaged RCipher = new RijndaelManaged();

        //        //메모리스트림 생성
        //        MemoryStream memoryStream = new MemoryStream(encryptBytes);
        //        ICryptoTransform decrypt = RCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

        //        //키와 백터값으로 암호화 스트림 생성
        //        CryptoStream cryptoStream = new CryptoStream(memoryStream, decrypt, CryptoStreamMode.Read);

        //        //복호화
        //        byte[] plainBytes = new byte[encryptBytes.Length];
        //        int plainCount = cryptoStream.Read(plainBytes, 0, plainBytes.Length);

        //        //복호화된 바이트 배열 String으로 변환
        //        string plainString = Encoding.UTF8.GetString(plainBytes, 0, plainCount);

        //        cryptoStream.Close();
        //        memoryStream.Close();

        //        return plainString;
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.Message.ToString();
        //        return string.Empty;
        //    }
        //}

        ///// <summary>
        ///// SHA256 단방향 암호화
        ///// </summary>
        //public string GenerateSHA256String(string inputStr)
        //{
        //    using SHA256 sha256Hash = SHA256.Create();
        //    byte[] hash = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(inputStr));
        //    StringBuilder builder = new StringBuilder();
        //    foreach (byte b in hash)
        //    {
        //        builder.AppendFormat("{0:x2}", b);
        //    }
        //    return builder.ToString();
        //}
        //#endregion

        //#region JWT         

        //private IDictionary<string, object> GetJwt(GetJwt Jwt, string input, out int code, out string msg)
        //{
        //    return Jwt(input, out code, out msg);
        //}

        ////accessToken
        //public string SetJwt(string memberId, string uuid = "")
        //{
        //    if(string.IsNullOrEmpty(uuid))
        //    {
        //        uuid = Guid.NewGuid().ToString();
        //    }
        //    var context = new JwtModelResult
        //    {
        //        MemberId = memberId,
        //        TokenGuid = uuid
        //    };

        //    var token = new JwtBuilder()                
        //    .WithAlgorithm(new HMACSHA256Algorithm()) 
        //    .WithSecret(_Secret)
        //    .Subject("kombirest.com")
        //    //.AddClaim("exp", DateTimeOffset.UtcNow.AddSeconds(3).ToUnixTimeSeconds())
        //    .AddClaim("exp", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds())
        //    .AddClaim("id", memberId)    
        //    .AddClaim("uuid", uuid)
        //    .Encode();

        //    var result = _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_MemberToken_SET", context, Instance(ConnectionType.DBCon), out int ErrCode, out string ErrMsg);
        //    if (!result)
        //    {
        //        token = string.Empty;
        //    }
        //    return token;
        //}

        ////refreshToken
        //public string SetJwt(int day, string memberid)
        //{
        //    string uuid = Guid.NewGuid().ToString();
        //    Int64 exp = Convert.ToInt64(DateTimeOffset.UtcNow.AddDays(14).ToUnixTimeSeconds().ToString());
        //    var token = new JwtBuilder()
        //    .WithAlgorithm(new HMACSHA256Algorithm()) 
        //    .WithSecret(_refreshSecret)
        //    .Subject("kombirest.com")
        //    .AddClaim("exp", exp)
        //    .AddClaim("id", memberid)
        //    .AddClaim("uuid", uuid)
        //    .Encode();

        //    var context = new JwtRefreshResult
        //    {
        //        MemberId = memberid,
        //        TokenGuid = uuid,
        //        ExpDate = exp
        //    };
        //    var result = _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_MemberRefreshToken_SET", context, Instance(ConnectionType.DBCon), out int ErrCode, out string ErrMsg);
        //    if (!result)
        //    {
        //        token = string.Empty;
        //    }
        //    return token;
        //}

        //public IDictionary<string, object> GetRefreshToken(string refreshToken, out Int32 ErrCode, out String ErrMsg)
        //{            
        //    IDictionary<string, object> token = new Dictionary<string, object>();
        //    try
        //    {
        //        ErrCode = 0;
        //        ErrMsg = "서명완료";

        //        token = new JwtBuilder()
        //        .WithAlgorithm(new HMACSHA256Algorithm())
        //        .WithSecret(_refreshSecret)
        //        .MustVerifySignature()
        //        .Decode<IDictionary<string, object>>(refreshToken);
               
        //        if (token["sub"].ToString() != "kombirest.com" || string.IsNullOrEmpty(token["id"].ToString()))
        //        {
        //            ErrCode = -4004;
        //            ErrMsg = "토큰 값을 읽는데 실패하였습니다.";
        //        }

        //        var context = new JwtModelContext
        //        {
        //            MemberId = token["id"].ToString()
        //        };
        //        var result = _Instance.ExecuteProcWithParamsSingle<JwtRefreshResult>("P_MemberRefreshToken_GET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);

        //        //if(result == null || token["uuid"].ToString() != result.TokenGuid)
        //        //{
        //        //    ErrCode = -4006;
        //        //    ErrMsg = "토큰 서명이 유효하지 않습니다.";
        //        //}
        //        //else
        //        //{
        //        //    if(Convert.ToInt64(token["exp"]) != result.ExtDate)
        //        //    {
        //        //        ErrCode = -4006;
        //        //        ErrMsg = "토큰 서명이 유효하지 않습니다.";
        //        //    }
        //        //}

        //    }
        //    catch (TokenExpiredException)
        //    {
        //        ErrCode = -4005;
        //        ErrMsg = "토큰 유효시간이 만료되었습니다.";
        //    }
        //    catch (SignatureVerificationException)
        //    {
        //        ErrCode = -4006;
        //        ErrMsg = "토큰 서명이 유효하지 않습니다.";
        //    }
        //    catch (Exception e)
        //    {
        //        ErrCode = -9999;
        //        ErrMsg = e.Message.ToString();
        //    }

        //    return token;
        //}

        //public IDictionary<string, object> GetAccessToken(string accessToken, out Int32 ErrCode, out String ErrMsg)
        //{
        //    IDictionary<string, object> token = new Dictionary<string, object>();
        //    ErrCode = 0;
        //    ErrMsg = "서명완료";
        //    try
        //    {
        //        token = new JwtBuilder()
        //        .WithAlgorithm(new HMACSHA256Algorithm())
        //        .WithSecret(_Secret)
        //        .MustVerifySignature()
        //        .Decode<IDictionary<string, object>>(accessToken);

        //        if (token["sub"].ToString() != "kombirest.com" && string.IsNullOrEmpty(token["id"].ToString()) || string.IsNullOrEmpty(token["uuid"].ToString()))
        //        {
        //            ErrCode = -4001;
        //            ErrMsg = "토큰 값을 읽는데 실패하였습니다.";
        //        }
        //    }
        //    catch (TokenExpiredException)
        //    {
        //        ErrCode = -4002;
        //        ErrMsg = "토큰 유효시간이 만료되었습니다.";
        //    }
        //    catch (SignatureVerificationException)
        //    {
        //        ErrCode = -4003;
        //        ErrMsg = "토큰 서명이 유효하지 않습니다.";
        //    }
        //    catch (Exception e)
        //    {
        //        ErrCode = -9999;
        //        ErrMsg = e.Message.ToString();
        //    }

        //    return token;
        //}

        ///// <summary>
        ///// 토큰 안에 있는 데이터 가져오기 
        ///// </summary>
        //public IDictionary<string, object> GetJwtPayload(string auth, out Int32 ErrCode, out String ErrMsg, out String NewToken)
        //{
        //    NewToken = string.Empty;
        //    var tokens = auth.Split(",");
        //    string accessToken = tokens[0].ToString();
        //    string refreshToken = tokens.Count() > 1 ? tokens[1].ToString() : string.Empty;
        //    IDictionary<string, object> jsonToken = new Dictionary<string, object>();

        //    _jwt = GetAccessToken;
        //    jsonToken = GetJwt(_jwt, accessToken, out ErrCode, out ErrMsg);

        //    if (ErrCode == -4002)
        //    {
        //        if (!string.IsNullOrEmpty(refreshToken))
        //        {
        //            _jwt = GetRefreshToken;
        //            jsonToken = GetJwt(_jwt, refreshToken, out ErrCode, out ErrMsg);
        //            if (ErrCode == 0 && !string.IsNullOrEmpty(jsonToken["id"].ToString()))
        //            {
        //                string newAccessToken = SetJwt(jsonToken["id"].ToString());
        //                NewToken = newAccessToken;
        //                _jwt = GetAccessToken;
        //                jsonToken = GetJwt(_jwt, newAccessToken, out ErrCode, out ErrMsg);                    
        //            }
        //        }
        //    }
        //    return jsonToken;
        //}

        //public bool GetGuidToken(string memberId, string guid, out int ErrCode, out string ErrMsg)
        //{
        //    bool result = true;
        //    var context = new JwtModelContext
        //    {
        //        MemberId = memberId
        //    };
        //    var data = _Instance.ExecuteProcWithParamsSingle<JwtModelResult>("P_MemberToken_GET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        //    if(data.TokenGuid != guid)
        //    {
        //        ErrCode = -4008;
        //        ErrMsg = "토큰 서명이 맞지 않습니다.";
        //        result = false ;
        //    }
        //    return result;
        //}
        //#endregion

        //#region info

        ///// <summary>
        ///// Google SMTP을 이용한 이메일 전송
        ///// </summary>
        ////public static void SendEmail(string toEmail, string title, string content, out Int32 ErrCode, out String ErrMsg)
        //public void CallSMTP(string toEmail, string title, string content, out Int32 ErrCode, out String ErrMsg)
        //{
        //    var mailMessage = new MailMessage();
        //    var fromMail = "Kombirest@gmail.com";

        //    try
        //    {
        //        // 보내는 사람 메일, 이름, 인코딩(UTF-8)
        //        mailMessage.From = new MailAddress(fromMail, "Kombirest", System.Text.Encoding.UTF8);

        //        // 받는 사람 메일
        //        mailMessage.To.Add(toEmail);

        //        // 메일 제목
        //        mailMessage.Subject = title;

        //        // 본문 내용
        //        mailMessage.Body = content;

        //        // 본문 내용 포멧의 타입 (true의 경우 Html 포멧으로)
        //        mailMessage.IsBodyHtml = true;

        //        // 메일 제목과 본문의 인코딩 타입(UTF-8)
        //        mailMessage.SubjectEncoding = Encoding.UTF8;
        //        mailMessage.BodyEncoding = Encoding.UTF8;

        //        SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
        //        smtpClient.UseDefaultCredentials = false; // 시스템에 설정된 인증 정보를 사용하지 않는다.
        //        smtpClient.Port = 587; // smtp 포트
        //        //smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network; // 이걸 하지 않으면 Gmail에 인증을 받지 못한다.

        //        // fromEmail Decrypt Pwd >> kombirest@gmail.com 암호화 비밀번호 
        //        var decPwd = AESDecryptString("JtdZjAxNyh1Tws7ubNcTRA==");
        //        smtpClient.Credentials = new NetworkCredential(fromMail, decPwd);

        //        // SSL 사용 여부
        //        smtpClient.EnableSsl = true;

        //        // 발송
        //        smtpClient.Send(mailMessage);

        //        ErrCode = 0;
        //        ErrMsg = string.Empty;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrCode = -1;
        //        ErrMsg = ex.Message.ToString();
        //    }
        //}


        ///// <summary>
        ///// 인증코드 이메일 전송 
        ///// </summary>
        ///// <param name="models"></param>
        ////public static void SendEmailOfAuthCode(SendEmailContext context, out Int32 ErrCode, out String ErrMsg)
        //public void SendEmailOfAuthCode(SendEmailContext context, out Int32 ErrCode, out String ErrMsg)
        //{
        //    try
        //    {
        //        /* [1] 비밀번호 변경 폼 작성 */

        //        // 보내는 이메일 주소
        //        var toEmail = context.MemberId;

        //        // 제목
        //        var title = string.Empty; 
        //        if(context.AuthType == AuthType.FindPwd)
        //        {
        //            title = $"[Kombirest] 비밀번호 재설정을 위한 메일입니다. ";
        //        }
        //        else
        //        {
        //            title = $"[Kombirest] 회원가입 인증 메일입니다. ";
        //        }

        //        // 비밀번호 변경 폼 가져오기 
        //        var content = GetAuthForm(context.ServerMapPath, context.AuthType);

        //        // MemberId & AuthCode 암호화
        //        var encEmail = AESEncryptString(context.MemberId);
        //        var encAuthCode = AESEncryptString(context.AuthCode);

        //        // UrlEncode >> url에 + 기호가 공백으로 표현되는것을 방지
        //        var param = $"?MemberId={System.Web.HttpUtility.UrlEncode(encEmail)}&AuthCode={System.Web.HttpUtility.UrlEncode(encAuthCode)}&AuthType={context.AuthType}";

        //        var protocol = "http";

        //        // 기존 if (HttpContext.Current.Request.Url.ToString().IndexOf("https") > -1) // 0
        //        if (_accessor.HttpContext.Request.Protocol.ToString().IndexOf("https") > -1) // 0
        //        {
        //            protocol = "https";
        //        }
        //        // 기존 var url = $"{protocol}://{HttpContext.Current.Request.Url.Authority}/Setting/ChangePwd{param}"; // 결과값 "localhost:44370"
        //        //var url = $"{protocol}://{_accessor.HttpContext.Request.Host.Value}/Setting/ChangePwd{param}"; //"localhost:44370"
        //        var url = $"{protocol}://{_accessor.HttpContext.Request.Host.Value}/api/EmailAuth/EmailAuthCheck{param}"; //"localhost:44370"
        //        content = content.Replace("[[URL]]", url);

        //        /* [2] 이메일 SMTP 호출 */
        //        CallSMTP(toEmail, title, content, out ErrCode, out ErrMsg);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrCode = -1;
        //        ErrMsg = ex.Message;
        //    }
        //}

        ///// <summary>
        ///// 인증 폼 가져오기 
        ///// </summary>
        //public string GetAuthForm(string ServerMapPath, AuthType AuthType)
        //{
        //    var resultContent = string.Empty;
        //    var contentUrl = string.Empty;

        //    if (AuthType == AuthType.FindPwd)
        //    {
        //        contentUrl = "Content\\EmailContent\\ResetPwd.html";
        //    }
        //    else
        //    {
        //        contentUrl = "Content\\EmailContent\\JoinAuth.html";
        //    }

        //    if (!string.IsNullOrEmpty(contentUrl))
        //    {
        //        // 예상 결과 값 "C:\\Users\\lhk74\\source\\repos\\TestMvc5\\TestMvc5\\Content\\EmailContent\\ResetPwd.html"
        //        // 기존 resultContent = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath(contentUrl));
        //        resultContent = File.ReadAllText(Path.Combine(ServerMapPath, contentUrl));
        //    }

        //    return resultContent;
        //}

        //#endregion
    }
}
