using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using db.service;
using kombirest.service.Interface.Commons;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Model.kombirest.model.Common;

namespace kombirest.service.Service.Common
{
    public class AwsMyinfo: SqlExecute, IAws
    {
        private AwsModel.S3Context _s3;

        public AwsMyinfo()
        {
            GetS3();
        }

        public AwsModel.S3Context GetS3()
        {
            _s3 = new AwsModel.S3Context();
            IConfigurationSection data = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("s3Connection");
            _s3.Bucket = data["bucket"].ToString();
            _s3.AccessKey = data["access"].ToString();
            _s3.SecretKey = data["secret"].ToString();
            //_s3.Path = data["path"].ToString();
            _s3.Path = "profile/";
            _s3.Domain = data["domain"].ToString();
            return _s3;
        }

        public void SetS3Delete(List<IFormFile> files, string fileDate)
        {

        }

        public void SetS3Delete(string Key, string VersionId)
        {
            GetS3();
            using var client = new AmazonS3Client(_s3.AccessKey, _s3.SecretKey, RegionEndpoint.APNortheast2);
            DeleteObjectRequest request = new DeleteObjectRequest
            {
                BucketName = _s3.Bucket,
                Key = _s3.Path + Key
            };

            client.DeleteObjectAsync(request);
        }

        public async Task S3upload(List<IFormFile> files, string fileDate, string filePath)
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
    }
}
