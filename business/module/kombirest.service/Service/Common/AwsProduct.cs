using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using db.service;
using kombirest.service.Interface.Commons;
using kombirest.service.Interface.Product;
using kombirest.service.Service.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Model.kombirest.model.Common;

namespace kombirest.service.Service.Common
{
    public class AwsProduct: SqlExecute, IAws
    {
        private AwsModel.S3Context _s3;

        public AwsModel.S3Context GetS3()
        {
            _s3 = new AwsModel.S3Context();
            IConfigurationSection data = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("s3Connection");
            _s3.Bucket = data["bucket"].ToString();
            _s3.AccessKey = data["access"].ToString();
            _s3.SecretKey = data["secret"].ToString();
            _s3.Path = data["path"].ToString();
            _s3.Domain = data["domain"].ToString();
            return _s3;
        }

        public void SetS3Delete(List<IFormFile> files, string fileDate)
        {
            using var client = new AmazonS3Client(_s3.AccessKey, _s3.SecretKey, RegionEndpoint.APNortheast2);
            var s3Path = new StringBuilder();
            files.ForEach(tmp =>
            {
                s3Path.Append(_s3.Path);
                s3Path.Append(fileDate);
                s3Path.Append("_");
                s3Path.Append(tmp.FileName);

                DeleteObjectRequest request = new DeleteObjectRequest
                {
                    BucketName = _s3.Bucket,
                    Key = s3Path.ToString()
                };

                client.DeleteObjectAsync(request);
            });
        }

        public void SetS3Delete(string Key, string VersionId)
        {

        }

        public async Task S3upload(List<IFormFile> files, string fileDate)
        {
            if (files != null)
            {
                foreach (var form in files)
                {
                    if (form.Length > 0)
                    {
                        var config = new AmazonS3Config
                        {
                            Timeout = TimeSpan.FromSeconds(30),
                            RegionEndpoint = RegionEndpoint.APNortheast2
                        };

                        using var client = new AmazonS3Client(_s3.AccessKey, _s3.SecretKey, config);
                        var s3Path = new StringBuilder();
                        s3Path.Append(_s3.Path);
                        s3Path.Append(fileDate);
                        s3Path.Append("_");
                        s3Path.Append(form.FileName);

                        using (var memoryStream = new MemoryStream())
                        {
                            await form.CopyToAsync(memoryStream);
                            PutObjectRequest request = new PutObjectRequest()
                            {
                                InputStream = memoryStream,
                                BucketName = _s3.Bucket,
                                Key = s3Path.ToString()
                            };
                            await client.PutObjectAsync(request);
                        }
                    }
                }
            }
        }

        public async Task S3upload(List<IFormFile> files, string fileDate, string filePath)
        {
            //--------------------------------
            // s3 업로드
            //--------------------------------
            //_s3.AccessKey, _s3.SecretKey, RegionEndpoint.APNortheast2

            if (files != null && files.Count > 0)
            {
                //AwsModel.S3Context _s3 = GetS3();
                GetS3();
                var config = new AmazonS3Config
                {
                    Timeout = TimeSpan.FromSeconds(30),
                    RegionEndpoint = RegionEndpoint.APNortheast2
                };
                using var client = new AmazonS3Client(_s3.AccessKey, _s3.SecretKey, config);
                var fileTransferUtility = new TransferUtility(client);
                var s3Path = new StringBuilder();
                string fileName;
                IProductCreate create = new ProductCreate();
                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        fileName = create.GetParseFileName(formFile.FileName);
                        s3Path.Append(_s3.Path);
                        s3Path.Append(filePath);
                        s3Path.Append("/");
                        s3Path.Append(fileDate);
                        s3Path.Append("_");
                        s3Path.Append(fileName);
                        using (var memoryStream = new MemoryStream())
                        {
                            await formFile.CopyToAsync(memoryStream);
                            var uploadRequest = new TransferUtilityUploadRequest
                            {
                                InputStream = memoryStream,
                                Key = s3Path.ToString(),
                                BucketName = _s3.Bucket,
                                CannedACL = S3CannedACL.PublicRead
                            };
                            fileTransferUtility = new TransferUtility(client);
                            await fileTransferUtility.UploadAsync(uploadRequest);
                        }
                    }
                    s3Path = new StringBuilder();
                }
            }
        }
    }
}
