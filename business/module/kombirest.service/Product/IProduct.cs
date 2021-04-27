using Model.kombirest.model.Activity;
using Model.kombirest.model.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Product
{
    public interface IProduct
    {
        List<ProductModel.PrdResult> GetProductList(ProductModel.PrdListContext context, out int ErrCode, out string ErrMsg);
        
        ProductModel.PrdDetailResult GetProductDetail(ProductModel.PrdDetailSingleRequest context, out int ErrCode, out string ErrMsg);

    }
}
