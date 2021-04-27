using Microsoft.AspNetCore.Http;
using Model.kombirest.model.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace kombirest.service.Interface.Commons
{
    public interface IAws
    {
        AwsModel.S3Context GetS3();

        void SetS3Delete(List<IFormFile> files, string fileDate);

        void SetS3Delete(string Key, string VersionId);

        Task S3upload(List<IFormFile> files, string fileDate, string filePath);

        Task S3upload(List<IFormFile> files, string fileDate);
    }
}
