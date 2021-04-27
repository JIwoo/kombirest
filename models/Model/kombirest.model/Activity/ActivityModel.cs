using System;
using System.Collections.Generic;
using System.Text;

namespace Model.kombirest.model.Activity
{
    public class ActivityModel
    {
        public class ActivityInsertContext
        {
            /// <summary>
            /// 회원아이디
            /// </summary>
            public string MemberId { get; set; } = string.Empty;

            /// <summary>
            /// 활동명
            /// </summary>
            public string ActNm { get; set; }

            public List<ItemInsertContext> Items { get; set; }
        }

        public class ActivityBaseInsertContext
        {
            /// <summary>
            /// 회원아이디
            /// </summary>
            public string MemberId { get; set; } = string.Empty;

            /// <summary>
            /// 활동명
            /// </summary>
            public string ActNm { get; set; }
        }

        public class ActivityCopyContext
        {
            /// <summary>
            /// 회원아이디
            /// </summary>
            public string MemberId { get; set; }

            /// <summary>
            /// 활동명
            /// </summary>
            public Int64 Key { get; set; }
        }

        public class ActivityContext
        {
            /// <summary>
            /// 회원아이디
            /// </summary>
            public string MemberId { get; set; }
        }

        public class ActivityResultContext
        {
            /// <summary>
            /// 활동 고유키
            /// </summary>
            public long ActIdx { get; set; }

            /// <summary>
            /// 활동명
            /// </summary>
            /// 
            public String ActNm { get; set; }
        }

        public class ActivityDeleteContext
        {
            /// <summary>
            /// 활동고유키
            /// </summary>
            public long ActIdx { get; set; }
        }

        public class ItemInsertContext
        {
            public string PageUrl { get; set; }

            public string ItemNm { get; set; }

            public string Model { get; set; }
        }
    }
}
