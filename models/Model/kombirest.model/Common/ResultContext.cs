using System;
using System.Collections.Generic;
using System.Text;

namespace Model.kombirest.model.Common
{
    public class ResultContext
    {
        /// <summary>
        /// 에러코드
        /// </summary>
        //public int ErrCode { get; set; } = 0;
        public int Code { get; set; } = 0;

        /// <summary>
        /// 에러메세지
        /// </summary>
        //public string ErrMsg { get; set; } = string.Empty;
        public string Msg { get; set; } = string.Empty;

        public Object Datas { get; set; }

        public ResultContext()
        {
            Datas = new Object();
        }
    }

    public class ResultPostContext
    {
        public string AccessToken { get; set; }
    }

    public class LoginResultContext
    {
        /// <summary>
        /// 에러코드
        /// </summary>
        //public int ErrCode { get; set; } = 0;
        public int Code { get; set; } = 0;

        /// <summary>
        /// 에러메세지
        /// </summary>
        //public string ErrMsg { get; set; } = string.Empty;
        public string Msg { get; set; } = string.Empty;

        public string AccessToken { get; set; }

        //public string RefreshToken { get; set; }       
        
        public string NickName { get; set; }

        public string Profile { get; set; }
    }
}
