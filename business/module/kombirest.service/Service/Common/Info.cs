using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using db.service;
using kombirest.service.Interface.Commons;
using Microsoft.AspNetCore.Http;
using Model.Enum;
using Model.kombirest.model.EmailAuth;

namespace kombirest.service.Service.Common
{
    public class Info: SqlExecute, IInfo
    {
        private readonly IHttpContextAccessor _accessor = new HttpContextAccessor();

        public void CallSMTP(string toEmail, string title, string content, out Int32 ErrCode, out String ErrMsg)
        {
            //--------------------------------
            // From :                   보내는 사람 메일, 이름, 인코딩(UTF-8)
            // To :                     받는 사람 메일
            // Subject :                메일 제목
            // Body :                   본문 내용
            // IsBodyHtml :             본문 내용 포멧의 타입 (true의 경우 Html 포멧으로)
            // SubjectEncoding :        메일 제목과 본문의 인코딩 타입(UTF-8)
            // BodyEncoding :           메일 제목과 본문의 인코딩 타입(UTF-8)
            // smtpClient :
            // UseDefaultCredentials :  설정된 인증 정보를 사용하지 않는다.
            // Port :                   smtp 포트
            //--------------------------------

            var mailMessage = new MailMessage();
            var fromMail = "Kombirest@gmail.com";

            try
            {
                mailMessage.From = new MailAddress(fromMail, "Kombirest", System.Text.Encoding.UTF8);
                mailMessage.To.Add(toEmail);
                mailMessage.Subject = title;
                mailMessage.Body = content;
                mailMessage.IsBodyHtml = true;
                mailMessage.SubjectEncoding = Encoding.UTF8;
                mailMessage.BodyEncoding = Encoding.UTF8;

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                smtpClient.UseDefaultCredentials = false; 
                smtpClient.Port = 587; // smtp 포트     
                ISecurity sec = new Security256();
                var decPwd = sec.DecryptString("JtdZjAxNyh1Tws7ubNcTRA==");
                smtpClient.Credentials = new NetworkCredential(fromMail, decPwd);

                smtpClient.EnableSsl = true;
                smtpClient.Send(mailMessage);

                ErrCode = 0;
                ErrMsg = string.Empty;
            }
            catch (Exception ex)
            {
                ErrCode = -1;
                ErrMsg = ex.Message.ToString();
            }
        }


        /// <summary>
        /// 인증코드 이메일 전송 
        /// </summary>
        /// <param name="models"></param>
        //public static void SendEmailOfAuthCode(SendEmailContext context, out Int32 ErrCode, out String ErrMsg)
        public void SendEmailOfAuthCode(SendEmailContext context, out Int32 ErrCode, out String ErrMsg)
        {
            //--------------------------------
            // toEmail :                보내는 이메일 주소
            // title :                  이메일 제목
            // content :                비밀번호 변경 폼 가져오기 
            // encEmail :               MemberId & AuthCode 암호화
            // encAuthCode :            MemberId & AuthCode 암호화
            // param :                  UrlEncode >> url에 + 기호가 공백으로 표현되는것을 방지
            //--------------------------------

            try
            {
                ISecurity sec = new Security256();

                var toEmail = context.MemberId;
                /* 이메일 Form 작성 */
                var title = string.Empty;
                if (context.AuthType == AuthType.FindPwd)
                {
                    title = $"[Kombirest] 비밀번호 재설정을 위한 메일입니다. ";
                }
                else
                {
                    title = $"[Kombirest] 회원가입 인증 메일입니다. ";
                }

                var content = GetAuthForm(context.ServerMapPath, context.AuthType);

                var encEmail = sec.EncryptString(context.MemberId);
                var encAuthCode = sec.EncryptString(context.AuthCode);
                var param = $"?MemberId={System.Web.HttpUtility.UrlEncode(encEmail)}&AuthCode={System.Web.HttpUtility.UrlEncode(encAuthCode)}&AuthType={context.AuthType}";
                var protocol = "http";

                //if (_accessor.HttpContext.Request.Protocol.ToString().IndexOf("https") > -1) // 0
                //{
                //    protocol = "https";
                //}
#if !DEBUG
                protocol = "https";
#endif 

                var url = $"{protocol}://{_accessor.HttpContext.Request.Host.Value}/api/EmailAuth/EmailAuthCheck{param}"; //"localhost:44370"
                content = content.Replace("[[URL]]", url);

                /* 이메일 SMTP 호출 */
                CallSMTP(toEmail, title, content, out ErrCode, out ErrMsg);
                
            }
            catch (Exception ex)
            {
                ErrCode = -1;
                ErrMsg = ex.Message;
            }
        }

        public string GetAuthForm(string ServerMapPath, AuthType AuthType)
        {
            var resultContent = string.Empty;
            var contentUrl = string.Empty;

            if (AuthType == AuthType.FindPwd)
            {
                contentUrl = "Content\\EmailContent\\ResetPwd.html";
            }
            else
            {
                contentUrl = "Content\\EmailContent\\JoinAuth.html";
            }

            return File.ReadAllText(Path.Combine(ServerMapPath, contentUrl)); 
        }
    }
}
