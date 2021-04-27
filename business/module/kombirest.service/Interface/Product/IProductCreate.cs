using Microsoft.AspNetCore.Http;
using Model.kombirest.model.Activity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Model.kombirest.model.Product.ProductModel;

namespace kombirest.service.Interface.Product
{
    public interface IProductCreate
    {
        void SetProduct<T>(T context, out int ErrCode, out string ErrMsg);

        Task<ActivityModel.ItemInsertContext> GetItems(List<ActivityModel.ItemInsertContext> items);

        Task<PrdFileUpdateContext> GetOptions(List<IFormFile> options);

        string GetParseFileName(string value);
    }
}
