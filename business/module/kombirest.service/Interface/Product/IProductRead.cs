using Model.kombirest.model.Product;
using System;
using System.Collections.Generic;
using System.Text;
using static Model.kombirest.model.Product.ProductModel;

namespace kombirest.service.Interface.Product
{
    public interface IProductRead
    {
        List<PrdResult> GetProductList<T>(T context, out int ErrCode, out string ErrMsg);

        PrdDetailResult GetProductDetail<T>(T context, out int ErrCode, out string ErrMsg);

        List<PrdOptionResult> GetDeleteImage<T>(T context, out int ErrCode, out string ErrMsg);
    }
}
