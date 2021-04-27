using System;
using System.Collections.Generic;
using System.Text;
using db.service;
using kombirest.service.Interface.Board;

namespace kombirest.service.Service.Board
{
    public class BoardUpdate: SqlExecute, IBoardUpdate
    {
        public void SetBoardUpdate<T>(T context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 댓글수정
            //--------------------------------

            _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_ProductBoardUpdate_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }

        public void SetProductReport<T>(T context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 신고하기  
            //--------------------------------
            _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_ProductReport_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }
    }
}
