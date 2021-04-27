using Model.kombirest.model.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Model.kombirest.model.Product.ProductModel;

namespace kombirest.service.Interface.Product
{
    public interface IProductDelete
    {
        Task<PrdDeletesContext> GetDeletes(List<DeleteFileRequest> deletes);

        void SetDelete(AwsModel.S3Context s3, PrdUpdateRequest request);

        void SetDelete(AwsModel.S3Context s3, List<PrdOptionResult> request);

        void SetDelete(AwsModel.S3Context s3, string key);

        void SetDeleteDb<T>(T context, out int ErrCode, out string ErrMsg);

        List<T> GetDeleteImage<T>(PrdDetailRequest context, out int ErrCode, out string ErrMsg);
    }
}
