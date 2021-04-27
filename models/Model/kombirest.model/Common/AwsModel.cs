using System;
using System.Collections.Generic;
using System.Text;

namespace Model.kombirest.model.Common
{
    public class AwsModel
    {
        public class S3Context
        {
            public string Bucket { get; set; }
            
            public string AccessKey { get; set; }

            public string SecretKey { get; set; }

            public string Path { get;set; }

            public string Domain { get; set; }
        }
    }
}
