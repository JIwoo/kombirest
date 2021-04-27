using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Globalization;
using System.Text;
using Model.kombirest.model.Activity;
using Microsoft.AspNetCore.Http;
using Model.kombirest.model.Attribute;

namespace Model.kombirest.model.Product
{
    public class ProductModel
    {
        #region request

        public class PrdAddRequest
        {

            public Int64 key { get; set; } = 0;

            [Required(ErrorMessage = "활동명이 없습니다.")]
            [StringLength(10, ErrorMessage = "활동명은 10자까지 가능합니다.")]
            public string actnm { get; set; }

            [Required(ErrorMessage = "제목이 없습니다.")]
            [StringLength(50, ErrorMessage = "게시물 제목은 50자까지 가능합니다.")]
            public string title { get; set; }

            [Required(ErrorMessage = "게시물 내용이 없습니다.")]
            [StringLength(1000, ErrorMessage = "내용은 1000자까지 가능합니다.")]
            public string contents { get; set; }

            public string memberId { get; set; } = string.Empty;

            public string Guid { get; set; } = string.Empty;

            [Required(ErrorMessage = "게시물 이미지가 없습니다.")]
            public List<IFormFile> files { get; set; }

            [Required(ErrorMessage = "아이템이 없습니다.")]
            public List<ActivityModel.ItemInsertContext> items { get; set; }

            //[Range(1, 99999999999, ErrorMessage = "카테고리를 선택해주세요.")]
            public int interCode { get; set; }
        }

        public class PrdUpdateRequest
        {
            [Range(1, 99999999999, ErrorMessage = "게시물이 존재하지 않습니다.")]
            public Int64 Key { get; set; }

            [Required(ErrorMessage = "활동명이 없습니다.")]
            [StringLength(10, ErrorMessage = "활동명은 10자까지 가능합니다.")]
            public string Actnm { get; set; }

            [Required(ErrorMessage = "제목이 없습니다.")]
            [StringLength(50, ErrorMessage = "게시물 제목은 50자까지 가능합니다.")]
            public string Title { get; set; }

            [Required(ErrorMessage = "게시물 내용이 없습니다.")]
            [StringLength(1000, ErrorMessage = "내용은 1000자까지 가능합니다.")]
            public string Contents { get; set; }

            public string memberId { get; set; } = string.Empty;

            public string Guid { get; set; } = string.Empty;

            public List<IFormFile> Files { get; set; }

            [Required(ErrorMessage = "아이템이 없습니다.")]
            public List<ActivityModel.ItemInsertContext> Items { get; set; }

            public List<DeleteFileRequest> Deletes { get; set; }

            //[Range(1, 99999999999, ErrorMessage = "카테고리를 선택해주세요.")]
            public int interCode { get; set; }
        }

        public class DeleteFileRequest
        {
            public Int64 Idx { get; set; }
            public string Key { get; set; }
        }

        public class PrdDetailRequest
        {
            [Range(1, 99999999999, ErrorMessage = "아이디가 없습니다.")]
            public Int64 Key { get; set; }

            public string TokenGuid { get; set; } = string.Empty;

            public string MemberId { get; set; } = string.Empty;
        }

        public class PrdReportRequest
        {
            [Range(1, 99999999999, ErrorMessage = "아이디가 없습니다.")]
            public Int64 Idx { get; set; }

            public int Key { get; set; }

            public string TokenGuid { get; set; } = string.Empty;

            public string MemberId { get; set; } = string.Empty;
        }

        public class PrdDetailSingleRequest
        {
            [Range(1, 99999999999, ErrorMessage = "아이디가 없습니다.")]
            public Int64 Key { get; set; }

            public string TokenGuid { get; set; } = string.Empty;

            public string MemberId { get; set; } = string.Empty;
        }

        public class PrdBoardScrollRequest
        {
            [Range(1, 99999999999, ErrorMessage = "아이디가 없습니다.")]
            public Int64 Key { get; set; }

            public int Page { get; set; }

            public int PageSize { get; set; }
        }

        public class PrdBoardRequest
        {
            public Int64 PrdIdx { get; set; }

            public string NickName { get; set; }

            public string Contents { get; set; }
        }

        public class PrdBoardUpdateRequest
        {
            [Range(0, 99999999999, ErrorMessage = "댓글이 없습니다")]
            public Int64 BoardIdx { get; set; }

            [Range(0, 99999999999, ErrorMessage = "게시물이 없습니다.")]
            public Int64 PrdIdx { get; set; }

            public string MemberId { get; set; } = string.Empty;

            public string MemberNick { get; set; }

            [Required(ErrorMessage = "내용이 없습니다.")]
            public string Contents { get; set; }

            public Int16 Depth { get; set; } = 0;

            public string TokenGuid { get; set; } = string.Empty;
        }

        public class PrdBoardDeleteRequest
        {
            [Range(0, 99999999999, ErrorMessage = "댓글이 없습니다")]
            public Int64 BoardIdx { get; set; }

            [Range(0, 99999999999, ErrorMessage = "게시물이 없습니다.")]
            public Int64 PrdIdx { get; set; }

            public string MemberId { get; set; } = string.Empty;

            public string TokenGuid { get; set; } = string.Empty;
        }

        public class PrdChoiceRequest
        {
            [Range(1, 99999999999, ErrorMessage = "게시물이 존재하지 않습니다.")]
            public Int64 Key { get; set; }

            public string MemberId { get; set; } = string.Empty;

            public string TokenGuid { get; set; } = string.Empty;
        }

        #endregion

        #region context

        public class PrdUpdateContext
        {
            public string PrdType { get; set; }

            public string PrdImg { get; set; }

            public Int64 PrdIdx { get; set; }

            public string PrdContent { get; set; }

            public string PrdNm { get; set; }

            public string ActNm { get; set; }

            public string MemberId { get; set; }

            public int InterIdx { get; set; }

            public string ItemUrl { get; set; }

            public string ItemName { get; set; }

            public string ItemModel { get; set; }

            public string OptionType { get; set; }

            public string OptionFileName { get; set; }

            public string OptionLength { get; set; }

            public string Path { get; set; }

            public string TokenGuid { get; set; }

            public string SaveDate { get; set; }

            public string DeleteIdx { get; set; }
        }

        public class PrdFileUpdateContext
        {
            public string OptionType { get; set; }

            public string OptionFileName { get; set; }

            public string OptionLength { get; set; }
        }

        public class PrdDeletesContext
        {
            public string DeleteIdx { get; set; }
        }

        public class ProductSetContext
        {
            public string ActNm { get; set; }

            public string PrdContent { get; set; }

            public string PrdNm { get; set; }

            public string PrdType { get; set; }

            public string PrdImg { get; set; }

            public string MemberId { get; set; }

            public int InterIdx { get; set; }

            public string ItemUrl { get; set; }

            public string ItemName { get; set; }

            public string ItemModel { get; set; }

            public string OptionType { get; set; }

            public string OptionFileName { get; set; }

            public string OptionLength { get; set; }

            public string Path { get; set; }

            public string SavePath { get; set; }

            public string TokenGuid { get; set; }

            public int ConnectionType { get; set; }
        }

        public class PrdInsertContext
        {
            public string PrdType { get; set; }

            public string PrdImg { get; set; }

            public Int64 PrdIdx { get; set; }

            public Int64 ActIdx { get; set; }

            public string PrdContent { get; set; }

            public string PrdNm { get; set; }

            public string MemberId { get; set; }

            public string FileRealName { get; set; }

            public string FileSaveName { get; set; }

            public long FileSize { get; set; }

            public string FilePath { get; set; }

            public string FileType { get; set; }
        }

        public class PrdListContext
        {
            public string Type { get; set; }

            public string Text { get; set; }

            public int Page { get; set; }

            public int PageSize { get; set; }

            public string MemberId { get; set; }

            public PrdListContext()
            {
                Type = "choice";
                Text = string.Empty;
                Page = 1;
                PageSize = 10;
            }
        }

        public class PrdBoardContext
        {
            public string MemberId { get; set; }

            public Int64 PrdIdx { get; set; }

            public string ReplyNick { get; set; }

            public string Contents { get; set; }

            public Int16 Depth { get; set; } = 0;

            public string Guid { get; set; }
        }

        #endregion

        #region result

        public class PrdDetailResult
        {
            public PrdResult Product { get; set; }

            public List<PrdBoardResult> Board { get; set; }

            public List<PrdOptionResult> Options { get; set; }

            public List<PrdActiveResult> Activity { get; set; }

            public List<PrdActiveItemResult> Items { get; set; }

            public PrdDetailResult()
            {
                Product = new PrdResult();
                Board = new List<PrdBoardResult>();
                Options = new List<PrdOptionResult>();
                Activity = new List<PrdActiveResult>();
                Items = new List<PrdActiveItemResult>();
            }
        }

        public class PrdResult
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

            public string FollowId { get; set; }

            public bool FollowState { get; set; }

            public bool ChoiceState { get; set; }
        }

        public class PrdOptionResult
        {
            public Int64 OptionIdx { get; set; }

            public string ImgPath { get; set; }

            public string ImgType { get; set; }

            public string ImgSaveNm { get; set; }

            public DateTime RegDt { get; set; }
        }

        public class PrdBoardResult
        {
            public Int64 BoardIdx { get; set; }

            public Int64 PrdIdx { get; set; }

            public string MemberId { get; set; }

            public string MemberNick { get; set; }

            public string ReplyId { get; set; }

            public string ReplyNick { get; set; }

            public string Contents { get; set; }

            public Int16 Depth { get; set; }

            public DateTime AlterDt { get; set; }

            public DateTime RegDt { get; set; }

            public string Profile { get; set; }
        }

        public class PrdActiveResult
        {
            public Int64 ActIdx { get; set; }

            public string ActNm { get; set; }

            public string ActContent { get; set; }

            public string CreateId { get; set; }

            public int InterIdx { get; set; }

            public string InterName { get; set; }
        }

        public class PrdActiveItemResult
        {
            public Int64 ItemIdx { get; set; }

            public string PageUrl { get; set; }

            public string ItemNm { get; set; }

            public string Model { get; set; }

            public bool UseYn { get; set; }
        }

        #endregion
    }
}
