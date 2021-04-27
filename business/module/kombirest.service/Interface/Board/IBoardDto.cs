using Model.kombirest.model.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Interface.Board
{
    public interface IBoardDto
    {
        void SetBoard<T>(IBoardCreate insert, T context, out int ErrCode, out string ErrMsg);

        List<ProductModel.PrdBoardResult> GetBoard<T>(IBoardRead read, T context, out int ErrCode, out string ErrMsg);

        void SetBoardUpdate<T>(IBoardUpdate update, T context, out int ErrCode, out string ErrMsg);

        void SetBoardDelete<T>(IBoardDelete delete, T context, out int ErrCode, out string ErrMsg);
    }
}
