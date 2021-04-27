using Microsoft.AspNetCore.Http;
using Model.kombirest.model.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Product
{
    public interface IProductDelete
    {
        void SetPrdDelete(ProductModel.PrdDetailRequest context, out int ErrCode, out string ErrMsg);

        void SetDelete(List<IFormFile> files, string fileDate);
    }
}
