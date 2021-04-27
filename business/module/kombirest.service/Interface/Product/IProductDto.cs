using Model.kombirest.model.Product;
using System;
using System.Collections.Generic;
using System.Text;
using static Model.kombirest.model.Product.ProductModel;

namespace kombirest.service.Interface.Product
{
    public interface IProductDto
    {
        void SetProduct(PrdAddRequest request, string fileDate, string filePath, out int ErrCode, out string ErrMsg);

        void SetPrdUpdate(PrdUpdateRequest request, out int ErrCode, out string ErrMsg);

        void SetProductReport<T>(IProductUpdate update, T context, out int ErrCode, out string ErrMsg);

        List<PrdResult> GetProductList<T>(IProductRead read, T context, out int ErrCode, out string ErrMsg);

        PrdDetailResult GetProductDetail<T>(IProductRead read, T context, out int ErrCode, out string ErrMsg);

        void SetProductChoice<T>(IProductUpdate update, T context, out int ErrCode, out string ErrMsg);

        void SetPrdDelete<T>(IProductDelete delete, T context, List<PrdOptionResult> s3Result, out int ErrCode, out string ErrMsg);
    }
}
