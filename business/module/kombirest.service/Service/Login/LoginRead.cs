using db.service;
using kombirest.service.Interface.Commons;
using kombirest.service.Interface.Login;
using kombirest.service.Service.Common;
using Model.Enum;
using Model.kombirest.model.Login;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Service.Login
{
    public class LoginRead: SqlExecute, ILoginRead
    {
        public LoginResultContext GetLoginResult<T>(T context, out Int32 ErrCode, out String ErrMsg)
        {
            var resultContext = _Instance.ExecuteProcWithParamsSingle<LoginResultContext>("P_MemberLogin_GET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);

            return resultContext;
        }

        public LoginResultContext LoginResultDetail(LoginContext context, out Int32 ErrCode, out String ErrMsg)
        {
            //--------------------------------
            // 1. 로그인처리
            // 2. 비밀번호 암호화
            //--------------------------------
            ISecurity _security = new Security256();
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
            }
            return resultContext;
        }
    }
}
