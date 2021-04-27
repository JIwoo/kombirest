using Model.kombirest.model.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Interface.Board
{
    public interface IBoardRead
    {
        List<ProductModel.PrdBoardResult> GetBoard<T>(T context, out int ErrCode, out string ErrMsg);
    }
}
