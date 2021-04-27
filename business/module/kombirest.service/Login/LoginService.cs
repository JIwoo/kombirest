//using Model.Enum;
//using Model.kombirest.model.Login;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using kombirest.service;
//using kombirest.service.Commons;
//using System.Security.Cryptography;

//namespace kombirest.service.Login
//{
//    public class LoginService
//        //: LoginDao
//    {
//        private readonly ISecurity _security;
//        private readonly IInfo _info;

//        //private readonly ILoginDao _dao = LoginDao._Instance;
//        //private static readonly Lazy<LoginService> _service = new Lazy<LoginService>(() => new LoginService());
//        //public static new LoginService _Instance { get { return _service.Value; } }

//        public LoginService(ISecurity security, IInfo info)
//        {
//            _security = security;
//            _info = info;
//        } 

//        public void LoginResultDetail(LoginContext context, out Int32 ErrCode, out String ErrMsg)
//        {
//            // 로그인 처리 
//            var resultContext = _dao.GetLoginResult(context, out ErrCode, out ErrMsg);

//            if (ErrCode == 0)
//            {
//                // 입력받은 패스워드 암호화 후 비교
//                if (!_security.GenerateSHA256String(context.MemberPwd).Equals(resultContext.MemberPwd))
//                {
//                    ErrCode = -1;
//                    ErrMsg = "비밀번호가 일치하지 않습니다.";
//                }
//                // 회원상태 
//                else if (!(resultContext.State.Equals(MemberStateType.정상) || resultContext.State.Equals(MemberStateType.마킹)))
//                {
//                    ErrCode = -1;
//                    ErrMsg = $"{resultContext.State} 회원";
//                }
//            }
//        }

//        public bool SetLoginLogDetail(string memberId, out Int32 ErrCode, out String ErrMsg)
//        {
//            var context = new LoginLogContext();
//            context.MemberId = memberId;
//            context.LoginIP = _info.GetIpAddress();
//            context.MemberId = _info.GetIpAddress();
//            context.Platform = _info.GetIpAddress();
//            context.BrowserType = _info.GetIpAddress();
//            context.State = LoginStateType.Login;

//            var result = _dao.SetLoginLog(context, out ErrCode, out ErrMsg);
//            return result;
//        }
//    }
//}
