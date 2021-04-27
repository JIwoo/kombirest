using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Model.kombirest.model.Member
{
    public class MemberModel
    {
        #region request

        public class MyListRequest
        {
            public string MemberId { get; set; } = string.Empty;

            public string TokenGuid { get; set; } = string.Empty;

            public int Page { get; set; }

            public int PageSize { get; set; }
        }

        public class MyFollowRequest
        {
            public string MemberId { get; set; } = string.Empty;

            public string TokenGuid { get; set; } = string.Empty;

            [Required(ErrorMessage = "조회할수 없습니다.")]
            public string Ftype { get; set; }

            [Range(1, 99999999999, ErrorMessage = "조회할수 없습니다.")]
            public int Page { get; set; }

            [Range(1, 99999999999, ErrorMessage = "조회할수 없습니다.")]
            public int PageSize { get; set; }
        }

        public class UserFollowRequest
        {
            [Required(ErrorMessage = "회원이 존재하지 않습니다.")]
            public string MemberNick { get; set; }

            [Required(ErrorMessage = "조회할수 없습니다.")]
            public string Ftype { get; set; }

            [Range(1, 99999999999, ErrorMessage = "조회할수 없습니다.")]
            public int Page { get; set; }

            [Range(1, 99999999999, ErrorMessage = "조회할수 없습니다.")]
            public int PageSize { get; set; }

            public string MemberId { get; set; } = string.Empty;

            public string TokenGuid { get; set; } = string.Empty;
        }

        public class MyListTypeRequest
        {
            public string MemberId { get; set; } = string.Empty;

            public string Type { get; set; }
        }

        public class MyFollowSetRequest
        {
            public string FollowNick { get; set; }

            public string MemberId { get; set; } = string.Empty;

            public string TokenGuid { get; set; } = string.Empty;
        }

        public class MyFollowDisconnectRequest
        {
            public string FollowNick { get; set; }

            public string Type { get; set; }

            public string MemberId { get; set; } = string.Empty;

            public string TokenGuid { get; set; } = string.Empty;
        }


        public class UserListRequest
        {
            public string UserNick { get; set; }

            public string MemberId { get; set; } = string.Empty;

            public string TokenGuid { get; set; } = string.Empty;

            public int Page { get; set; }

            public int PageSize { get; set; }
        }

        public class MemberInfoRequest
        {
            public string MemberId { get; set; } = string.Empty;

            public string TokenGuid { get; set; } = string.Empty;

            public string NickNm { get; set; }

            public int Gender { get; set; }

            public int Age { get; set; }

            public string Birth { get; set; }

            public IFormFile Files { get; set; }
        }

        public class MemberInfosRequest
        {
            public string MemberId { get; set; } = string.Empty;

            public string TokenGuid { get; set; } = string.Empty;

            public string NickNm { get; set; }

            public int Gender { get; set; }

            public int Age { get; set; }

            public string Birth { get; set; }

            public List<IFormFile> Files { get; set; }
        }

        #endregion

        #region context

        public class MemberInfoContext
        {
            public string MemberId { get; set; }

            public string TokenGuid { get; set; }
        }

        public class MemberInfoSetContext
        {
            public string MemberId { get; set; }

            public string TokenGuid { get; set; }

            public string NickNm { get; set; }

            public int Gender { get; set; }

            public int Age { get; set; }

            public string Birth { get; set; }

            public string Profile { get; set; }

            public string SaveNm { get; set; }

            public string VersionId { get; set; }
        }

        #endregion

        #region result

        public class MyListResult
        {
            public InfoResult Info { get; set; }

            public List<ProductResult> Product { get; set; }
        }


        public class InfoResult
        {
            public string MemberId { get; set; }

            public string Img { get; set; }

            public string NickName { get; set; }

            public Int16 FollowingState { get; set; }

            public Int16 FollowerState { get; set; }

            public int FollowerCount { get; set; }

            public int FollowingCount { get; set; }
        }

        public class ProductResult
        {
            public Int64 PrdIdx { get; set; }

            public string PrdType { get; set; }

            public string PrdContent { get; set; }

            public string PrdNm { get; set; }

            public string PrdImg { get; set; }

            public string MemberId { get; set; }

            public int Hit { get; set; }

            public int Report { get; set; }

            public int Choice { get; set; }

            public DateTime AlterDt { get; set; }

            public int BoardCount { get; set; }

            public string NickNm { get; set; }

            public string Profile { get; set; }

            public string ActNm { get; set; }

            public int ItemCount { get; set; }
        }

        public class MyFollowResult
        {
            public Int64 FollowIdx { get; set; }

            public string MemberId { get; set; }

            public string MemberNick { get; set; }

            public string FollowId { get; set; }

            public string FollowNick { get; set; }

            public string FollowProfile { get; set; }

            public DateTime RegDt { get; set; }

            public string MyFollowState { get; set; }
        }

        public class MemberInfoResult
        {
            public string MemberId { get; set; }

            public string Profile { get; set; }

            public string ProfileSaveNm { get; set; }

            public string NickNm { get; set; }

            public Int16 Gender { get; set; }

            public Int16 Age { get; set; }

            public string Birth { get; set; }

            public Int64 InfoIdx { get; set; }
        }

        public class MemberProfileResult 
        {
            public string Key { get; set; }

            public string VersionId { get; set; }
        }

        public class MemberPutResult
        {
            public string Nick { get; set; }

            public string Profile { get; set; }

            public string AccessToken { get; set; }
        }

        #endregion        
    }
}
