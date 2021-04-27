using Model.kombirest.model.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Board
{
    public interface IBoardList
    {
        List<ProductModel.PrdBoardResult> GetBoard(ProductModel.PrdBoardScrollRequest context, out int ErrCode, out string ErrMsg);
    }
}
