using System;
using System.Collections.Generic;
using System.Text;
using kombirest.service.Interface.Board;
using kombirest.service.Service.Board;
using Model.kombirest.model.Product;

namespace kombirest.service.Service.Board
{
    public class BoardDto: IBoardDto
    {
        public void SetBoard<T>(IBoardCreate insert, T context, out int ErrCode, out string ErrMsg)
        {
            insert.SetBoard(context, out ErrCode, out ErrMsg);
        }

        public List<ProductModel.PrdBoardResult> GetBoard<T>(IBoardRead read, T context, out int ErrCode, out string ErrMsg)
        {
            return read.GetBoard(context, out ErrCode, out ErrMsg);
        }

        public void SetBoardUpdate<T>(IBoardUpdate update, T context, out int ErrCode, out string ErrMsg)
        {
            update.SetBoardUpdate(context, out ErrCode, out ErrMsg);
        }

        public void SetBoardDelete<T>(IBoardDelete delete, T context, out int ErrCode, out string ErrMsg)
        {
            delete.SetBoardDelete(context, out ErrCode, out ErrMsg);
        }
    }
}
