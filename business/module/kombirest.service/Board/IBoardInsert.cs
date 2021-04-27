using Model.kombirest.model.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Board
{
    public interface IBoardInsert
    {
        void SetBoard(ProductModel.PrdBoardContext context, out int ErrCode, out string ErrMsg);

        void SetBoardUpdate(ProductModel.PrdBoardUpdateRequest context, out int ErrCode, out string ErrMsg);

        void SetBoardDelete(ProductModel.PrdBoardDeleteRequest context, out int ErrCode, out string ErrMsg);        
    }
}
