using System;
using System.Collections.Generic;
using System.Text;

namespace Model.kombirest.model.Common
{
    public class JwtModelResult
    {
        public string MemberId { get; set; }

        public string TokenGuid { get; set; } = string.Empty;
    }

    public class JwtRefreshResult
    {
        public string MemberId { get; set; }

        public string TokenGuid { get; set; }

        public Int64 ExpDate { get; set; }
    }

    public class JwtModelContext
    {
        public string MemberId { get; set; }
    }
}
