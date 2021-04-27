using System;
using System.Collections.Generic;
using System.Text;
using db.service;
using kombirest.service.Interface.Board;
using Model.kombirest.model.Product;

namespace kombirest.service.Service.Board
{
    public class BoardRead: SqlExecute, IBoardRead
    {
        public List<ProductModel.PrdBoardResult> GetBoard<T>(T context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 댓글조회
            //--------------------------------

            var data = _Instance.ExecuteProcWithParams<ProductModel.PrdBoardResult>("P_ProductBoard_GET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
            return data;
        }
    }
}
