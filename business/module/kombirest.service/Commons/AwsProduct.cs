using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
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
    public class AwsProduct : IAws
    {
        private AwsModel.S3Context _s3;
        public AwsModel.S3Context GetS3()
        {
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
            //--------------------------------
            // s3 업로드
            //--------------------------------
            //_s3.AccessKey, _s3.SecretKey, RegionEndpoint.APNortheast2

            if (files != null && files.Count > 0)
            {
                _s3 = this.GetS3();
                var config = new AmazonS3Config
                {
                    Timeout = TimeSpan.FromSeconds(30),
                    RegionEndpoint = RegionEndpoint.APNortheast2
                };
                using var client = new AmazonS3Client(_s3.AccessKey, _s3.SecretKey, config);
                var fileTransferUtility = new TransferUtility(client);
                var s3Path = new StringBuilder();

                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        s3Path.Append(_s3.Path);
                        s3Path.Append(fileDate);
                        s3Path.Append("_");
                        s3Path.Append(formFile.FileName);
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
