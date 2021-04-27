using db.service;
using Model.kombirest.model.Login;
using Model.Enum;
using System;
using System.Collections.Generic;
using System.Text;
//using kombirest.service.Commons;
using kombirest.service.Interface.Commons;
using Model.kombirest.model.Join;
using kombirest.service.Service.Common;

namespace kombirest.service.Login
{
    public class LoginDao : SqlExecute, ILoginDao
    {
        //, IDisposable
        //private bool disposed = false;

        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this); //Finalize()
        //}

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!disposed)
        //    {
        //        if (disposing)
        //        {

        //        }
        //        disposed = true;
        //    }            
        //}



        //private static Lazy<LoginDao> _loginDao = new Lazy<LoginDao>(() => new LoginDao());

        //public static LoginDao _Instance { get { return _loginDao.Value; }}

        //private readonly ISecurity _security;
        //private readonly IInfo _info;

        //public LoginDao(ISecurity security, IInfo info)
        //{
        //    _security = security;
        //    _info = info;
        //    //Dispose(false);
        //}

        public LoginResultContext GetLoginResult(LoginContext context, out Int32 ErrCode, out String ErrMsg)
        {
            var resultContext = SqlExecute._Instance.ExecuteProcWithParamsSingle<LoginResultContext>("P_MemberLogin_GET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);

            return resultContext;
        }

        public LoginLogModel.LoginLogResultContext SetLoginLog(LoginLogModel.LoginLogContext context, out Int32 ErrCode, out String ErrMsg)
        {
            //var resultContext = SqlExecute._Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_LoginLog_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
            // 
            var resultContext = SqlExecute._Instance.ExecuteProcWithParamsSingle<LoginLogModel.LoginLogResultContext>("P_LoginLog_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
            return resultContext;
        }

        //public void LoginResultDetail(LoginContext context, out Int32 ErrCode, out String ErrMsg)
        //{
        //    // 로그인 처리 
        //    // 비밀번호 암호화
        //    context.MemberPwd = _security.GenerateSHA256String(context.MemberPwd);
        //    var resultContext = GetLoginResult(context, out ErrCode, out ErrMsg);

        //    if (ErrCode == 0)
        //    {
        //        if (!(resultContext.State.Equals(MemberStateType.정상) || resultContext.State.Equals(MemberStateType.마킹)))
        //        {
        //            ErrCode = -1;
        //            ErrMsg = $"{resultContext.State} 회원";
        //        }
        //        else
        //        {
        //            ErrMsg = resultContext.NickNm;
        //        }
        //    }            
        //}

        public LoginResultContext LoginResultDetail(LoginContext context, out Int32 ErrCode, out String ErrMsg)
        {
            // 로그인 처리 
            // 비밀번호 암호화

            ISecurity _security = new Security256();
            //context.MemberPwd = _security.GenerateSHA256String(context.MemberPwd);
            context.MemberPwd = _security.GenerateString(context.MemberPwd);
            var resultContext = GetLoginResult(context, out ErrCode, out ErrMsg);

            if (ErrCode == 0)
            {
                if (!resultContext.State.Equals(MemberStateType.Normal))
                {
                    ErrCode = -1;
                    ErrMsg = "Kombirest@gmail.com 으로 문의 후 이용 부탁드립니다.";
                    switch (resultContext.State)
                    {
                        case MemberStateType.Pending:
                            ErrMsg = "이메일 인증이 필요합니다.";
                            break;
                        case MemberStateType.Stop:
                            ErrMsg = "정지된 회원입니다.";
                            break;
                    }
                }
                //else
                //{
                //    ErrMsg = resultContext.NickNm;
                //}
            }
            return resultContext;
        }

        public LoginLogModel.LoginLogResultContext SetLoginLogDetail(LoginLogModel.LoginLogContext context, out Int32 ErrCode, out String ErrMsg)
        {
            var result = SetLoginLog(context, out ErrCode, out ErrMsg);
            return result;
        }

        public bool SetJoin(JoinContext context, out Int32 ErrCode, out String ErrMsg)
        {
            var resultContext = _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_MemberJoin_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);

            return resultContext;
        }
    }
}
