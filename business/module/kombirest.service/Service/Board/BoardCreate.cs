using System;
using System.Collections.Generic;
using System.Text;
using db.service;
using kombirest.service.Interface.Board;

namespace kombirest.service.Service.Board
{
    public class BoardCreate: SqlExecute, IBoardCreate
    {
        public void SetBoard<T>(T context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 댓글등록
            //--------------------------------

            _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_ProductBoard_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }
    }
}
