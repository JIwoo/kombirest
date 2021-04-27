using db.service;
using Model.kombirest.model.EmailAuth;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
//using kombirest.service.Commons;
using kombirest.service.Interface.Commons;

namespace kombirest.service.EmailAuth
{
    public class EmailAuthDao : SqlExecute, IEmailAuthDao
    {
        //private readonly ISecurity _security;

        //public EmailAuthDao(ISecurity security)
        //{
        //    _security = security;
        //}

        public bool SetAuthCode(EmailAuthModel.EmailAuthContext context, out Int32 ErrCode, out String ErrMsg, out String AuthCode)
        {
            var resultContext = _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_AuthCode_SET", context, Instance(ConnectionType.DBCon), 
                                                                                        out ErrCode, out ErrMsg, out AuthCode);

            return resultContext;
        }

        public bool CheckAuthCode(EmailAuthModel.EmailAuthCheckContext context, out Int32 ErrCode, out String ErrMsg)
        {
            var resultContext = _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_CheckAuthResult_GET", context, Instance(ConnectionType.DBCon),
                                                                                        out ErrCode, out ErrMsg);

            return resultContext;
        }
    }
}
