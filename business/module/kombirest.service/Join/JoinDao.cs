using db.service;
using Model.kombirest.model.Join;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Join
{
    public class JoinDao : SqlFactory, IJoinDao
    {
        private static readonly Lazy<JoinDao> _joinDao = new Lazy<JoinDao>(() => new JoinDao());
        public static JoinDao _Instance { get { return _joinDao.Value; } }

        public bool SetJoin(JoinContext context, out Int32 ErrCode, out String ErrMsg)
        {
            var resultContext = SqlExecute._Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_MemberJoin_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);

            return resultContext;
        }
    }
}
