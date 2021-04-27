using System;
using System.Collections.Generic;
using System.Text;
using db.service;
using kombirest.service.Interface.Board;

namespace kombirest.service.Service.Board
{
    public class BoardDelete : SqlExecute,IBoardDelete
    {
        public void SetBoardDelete<T>(T context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 댓글삭제
            //--------------------------------

            _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_ProductBoardDelete_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }
    }
}
