//using Microsoft.AspNetCore.Http;
//using Model.kombirest.model.EmailAuth;
//using System;
//using System.Net.Mail;
//using System.Text;
//using kombirest.service;
//using kombirest.service.Commons;
//using System.IO;

//namespace kombirest.service.EmailAuth
//{
//    //public class EmailAuthService : EmailAuthDao
//    public class EmailAuthService
//    {
//        //private static readonly IEmailAuthDao _dao = EmailAuthDao._Instance;
//        //private static readonly Lazy<EmailAuthService> _service = new Lazy<EmailAuthService>(() => new EmailAuthService());
//        //public static new EmailAuthService _Instance { get { return _service.Value; } }
//        private readonly IHttpContextAccessor _httpContextAccessor;
//        private readonly ISecurity _security;
        
//        public EmailAuthService(IHttpContextAccessor httpContextAccessor, ISecurity security)
//        {
//            _httpContextAccessor = httpContextAccessor;
//            _security = security;
//        }

//        /// <summary>
//        /// 비밀번호 초기화 폼 가져오기 
//        /// </summary>
//        public static string GetPwdResetForm(string ServerMapPath)
//        {
//            var content = string.Empty;
//            var resultContent = string.Empty;
//            var contentUrl = "/Content/EmailContent/ResetPwd.html";
//            //var contentUrl = "Content\\EmailContent\\ResetPwd.html";

//            if (!string.IsNullOrEmpty(contentUrl))
//            {
//                // 예상 결과 값 "C:\\Users\\lhk74\\source\\repos\\TestMvc5\\TestMvc5\\Content\\EmailContent\\ResetPwd.html"
//                // 기존 resultContent = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath(contentUrl));
//                resultContent = Path.Combine(ServerMapPath, contentUrl); 
//            }

//            return resultContent;
//        }

//        /// <summary>
//        /// Google SMTP을 이용한 이메일 전송
//        /// </summary>
//        //public static void SendEmail(string toEmail, string title, string content, out Int32 ErrCode, out String ErrMsg)
//        public void SendEmail(string toEmail, string title, string content, out Int32 ErrCode, out String ErrMsg)
//        {
//            var mailMessage = new MailMessage();
//            var fromMail = "Kombirest@gmail.com";

//            try
//            {
//                // 보내는 사람 메일, 이름, 인코딩(UTF-8)
//                mailMessage.From = new MailAddress(fromMail, "Kombirest", System.Text.Encoding.UTF8);

//                // 받는 사람 메일
//                mailMessage.To.Add(toEmail);

//                // 메일 제목
//                mailMessage.Subject = title;

//                // 본문 내용
//                mailMessage.Body = content;

//                // 본문 내용 포멧의 타입 (true의 경우 Html 포멧으로)
//                mailMessage.IsBodyHtml = true;

//                // 메일 제목과 본문의 인코딩 타입(UTF-8)
//                mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
//                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;

//                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
//                //smtpClient.UseDefaultCredentials = false; // 시스템에 설정된 인증 정보를 사용하지 않는다.
//                smtpClient.Port = 587; // smtp 포트
//                //smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network; // 이걸 하지 않으면 Gmail에 인증을 받지 못한다.

//                // fromEmail Decrypt Pwd >> kombirest@gmail.com 암호화 비밀번호 
//                var decPwd = _security.AESDecryptString("JtdZjAxNyh1Tws7ubNcTRA==");
//                smtpClient.Credentials = new System.Net.NetworkCredential(fromMail, decPwd);

//                // SSL 사용 여부
//                smtpClient.EnableSsl = true;

//                // 발송
//                smtpClient.Send(mailMessage);

//                ErrCode = 0;
//                ErrMsg = string.Empty;
//            }
//            catch (Exception ex)
//            {
//                ErrCode = -1;
//                ErrMsg = ex.Message.ToString();
//            }
//        }


//        /// <summary>
//        /// 인증코드 이메일 전송 
//        /// </summary>
//        /// <param name="models"></param>
//        //public static void SendEmailOfAuthCode(SendEmailContext context, out Int32 ErrCode, out String ErrMsg)
//        public void SendEmailOfAuthCode(SendEmailContext context, out Int32 ErrCode, out String ErrMsg)
//        {
//            try
//            {
//                /* [1] 비밀번호 변경 폼 작성 */

//                // 보내는 이메일 주소
//                var toEmail = context.MemberId;

//                // 이메일 제목
//                var title = $"[Kombirest] 비밀번호 재설정을 위한 메일입니다. ";

//                // 비밀번호 변경 폼 가져오기 
//                var content = GetPwdResetForm(context.ServerMapPath);

//                // MemberId & AuthCode 암호화
//                var encEmail = _security.AESEncryptString(context.MemberId);
//                var encAuthCode = _security.AESEncryptString(context.AuthCode);

//                // UrlEncode >> url에 + 기호가 공백으로 표현되는것을 방지
//                var param = $"?Email={System.Web.HttpUtility.UrlEncode(encEmail)}&AuthCode={System.Web.HttpUtility.UrlEncode(encAuthCode)}";
//                //var protocol = "http";

//                //if (HttpContext.Current.Request.Url.ToString().IndexOf("https") > -1) // 0
//                // TODO 지원하지 않는 버전
//                //if (_httpContextAccessor.HttpContext.Request.Protocol.ToString().IndexOf("https") > -1) // 0
//                //{
//                //    protocol = "https";
//                //}

//                //var url = $"{protocol}://{HttpContext.Current.Request.Url.Authority}/Setting/ChangePwd{param}"; // 결과값 "localhost:44370"
//                // TODO 지원하지 않는 버전
//                //var url = $"{protocol}://{_httpContextAccessor.HttpContext.Request.Host.Value}/Setting/ChangePwd{param}"; //"localhost:44370"
//                //content = content.Replace("[[URL]]", url);

//                /* [2] 이메일 SMTP 호출 */
//                SendEmail(toEmail, title, content, out ErrCode, out ErrMsg);
//            }
//            catch (Exception ex)
//            {
//                ErrCode = -1;
//                ErrMsg = ex.Message;
//            }
//        }
//    }
//}
