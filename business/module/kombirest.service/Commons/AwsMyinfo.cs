using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Model.kombirest.model.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace kombirest.service.Commons
{
    public class AwsMyinfo : IAws
    {
        private AwsModel.S3Context _s3;

        public AwsMyinfo()
        {
            _s3 = new AwsModel.S3Context();
        }
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

        public void SetS3Delete()
        {

        }

        public async Task S3upload(List<IFormFile> files, string fileDate)
        {
            if (files != null)
            {
                foreach (var form in files)
                {
                    if(form.Length > 0)
                    {
                        var config = new AmazonS3Config
                        {
                            Timeout = TimeSpan.FromSeconds(30),
                            RegionEndpoint = RegionEndpoint.APNortheast2
                        };
                        GetS3();
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
