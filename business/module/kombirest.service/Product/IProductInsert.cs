using Microsoft.AspNetCore.Http;
using Model.kombirest.model.Activity;
using Model.kombirest.model.Product;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace kombirest.service.Product
{
    public interface IProductInsert
    {
        Task PrdInsert(List<IFormFile> files, string fileDate);

        void SetProduct(ProductModel.PrdAddRequest request, string fileDate, out int ErrCode, out string ErrMsg);

        Task S3upload(List<IFormFile> files, string fileDate);

        Task<Dictionary<int, string>> PrdInsert(ProductModel.PrdAddRequest request, string fileDate, string member);

        void SetPrd(Int64 key, List<ActivityModel.ItemInsertContext> items, out int ErrCode, out string ErrMsg);


        void SetPrd(ProductModel.PrdAddRequest context, string fileDate, string member, out int ErrCode, out string ErrMsg);
    }
}
