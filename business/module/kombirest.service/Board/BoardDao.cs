using db.service;
using Model.kombirest.model.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Board
{
    public class BoardDao : SqlExecute, IBoardInsert, IBoardList
    {
        public void SetBoard(ProductModel.PrdBoardContext context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 댓글등록
            //--------------------------------

            _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_ProductBoard_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }

        public List<ProductModel.PrdBoardResult> GetBoard(ProductModel.PrdBoardScrollRequest context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 댓글조회
            //--------------------------------

            var data = _Instance.ExecuteProcWithParams<ProductModel.PrdBoardResult>("P_ProductBoard_GET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
            return data;
        }

        public void SetBoardUpdate(ProductModel.PrdBoardUpdateRequest context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 댓글수정
            //--------------------------------

            _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_ProductBoardUpdate_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }

        public void SetBoardDelete(ProductModel.PrdBoardDeleteRequest context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 댓글삭제
            //--------------------------------

            _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_ProductBoardDelete_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }
    }
}
