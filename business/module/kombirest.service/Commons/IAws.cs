using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Model.kombirest.model.Common;

namespace kombirest.service.Commons
{
    public interface IAws
    {
        AwsModel.S3Context GetS3();

        void SetS3Delete();        

        Task S3upload(List<IFormFile> files, string fileDate);
    }
}
