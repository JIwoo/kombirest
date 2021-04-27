using db.service;
using Model.kombirest.model.PwdReset;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.PwdReset
{
    public class PwdResetDao : SqlFactory, IPwdResetDao
    {
        private static Lazy<PwdResetDao> _pwdResetDao = new Lazy<PwdResetDao>(() => new PwdResetDao());
        public static PwdResetDao _Instance { get { return _pwdResetDao.Value; } }

        public bool SetPwdReset(PwdResetContext context, out Int32 ErrCode, out String ErrMsg)
        {
            var resultContext = SqlExecute._Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_PwdReset_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);

            return resultContext;
        }
    }
}
