using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using db.service;
using kombirest.service.Interface.Product;
using Model.kombirest.model.Common;
using static Model.kombirest.model.Product.ProductModel;

namespace kombirest.service.Service.Product
{
    public class ProductDelete: SqlExecute, IProductDelete
    {        
        public async Task<PrdDeletesContext> GetDeletes(List<DeleteFileRequest> deletes)
        {
            //--------------------------------
            // 삭제할 옵션들 필드별로 하나의문자열로 파싱 "," 로 구분
            //--------------------------------

            var idx = new StringBuilder();
            await Task.Run(() =>
            {
                if (deletes != null && deletes.Count > 0)
                {
                    int count = 1;
                    int deleteCount = deletes.Count;
                    deletes.ForEach(tmp =>
                    {
                        idx.Append(tmp.Idx);

                        if (deleteCount > count)
                        {
                            idx.Append(",");
                        }
                        count++;
                    });
                }
            });
            return new PrdDeletesContext
            {
                DeleteIdx = idx.ToString()
            };
        }

        public void SetDelete(AwsModel.S3Context s3, PrdUpdateRequest request)
        {
            using var client = new AmazonS3Client(s3.AccessKey, s3.SecretKey, RegionEndpoint.APNortheast2);
            request.Deletes.ForEach(tmp =>
            {
                DeleteObjectRequest request = new DeleteObjectRequest
                {
                    BucketName = s3.Bucket,
                    Key = tmp.Key
                    //VersionId = versionID
                };

                client.DeleteObjectAsync(request);
            });
        }

        public void SetDelete(AwsModel.S3Context s3, List<PrdOptionResult> request)
        {
            using var client = new AmazonS3Client(s3.AccessKey, s3.SecretKey, RegionEndpoint.APNortheast2);
            request.ForEach(tmp =>
            {
                DeleteObjectRequest request = new DeleteObjectRequest
                {
                    BucketName = s3.Bucket,
                    Key = s3.Path + tmp.ImgSaveNm
                };

                client.DeleteObjectAsync(request);
            });
        }

        public void SetDelete(AwsModel.S3Context s3, string key)
        {
            using var client = new AmazonS3Client(s3.AccessKey, s3.SecretKey, RegionEndpoint.APNortheast2);
            DeleteObjectRequest request = new DeleteObjectRequest
            {
                BucketName = s3.Bucket,
                Key = s3.Path + key
            };
            client.DeleteObjectAsync(request);
        }

        public void SetDeleteDb<T>(T context, out int ErrCode, out string ErrMsg)
        {
            _Instance.ExecuteProcWithParams<T>("P_ProductDelete_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }

        public List<T> GetDeleteImage<T>(PrdDetailRequest context, out int ErrCode, out string ErrMsg)
        {
            return _Instance.ExecuteProcWithParams<T>("P_ProductImage_GET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }
    }
}
