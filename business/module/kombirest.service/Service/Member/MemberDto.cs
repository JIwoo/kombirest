using kombirest.service.Interface.Commons;
using kombirest.service.Interface.Member;
using kombirest.service.Service.Common;
using Model.kombirest.model.Member;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace kombirest.service.Service.Member
{
    public class MemberDto: IMemberDto
    {
        #region read

        public MemberModel.MyListResult GetMyList<T>(IMemberRead read, T context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 1. 나의 게시물리스트 조회
            // 2. 팔로워,팔로잉 정보
            //--------------------------------            
            return read.GetMyList(context, out ErrCode, out ErrMsg);
        }

        public List<MemberModel.ProductResult> GetMyProduct<T>(IMemberRead read, T context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 1. 나의 게시물리스트 조회
            //--------------------------------
            return read.GetMyProduct(context, out ErrCode, out ErrMsg);
        }

        public MemberModel.MyListResult GetUserList<T>(IMemberRead read, T context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 1. 유저정보 ( 팔로우,팔로워 등도 포함?? )       
            // 2. 유저의 게시물리스트
            //--------------------------------
            return read.GetUserList(context, out ErrCode, out ErrMsg);
        }

        public List<MemberModel.MyFollowResult> GetFollow<T>(IMemberRead read, T context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 1. 팔로잉 게시물 조회
            //--------------------------------
            return read.GetFollow(context, out ErrCode, out ErrMsg);
        }

        public List<MemberModel.MyFollowResult> GetUserFollow<T>(IMemberRead read, T context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 1. 팔로잉 게시물 조회
            //--------------------------------
            return read.GetUserFollow(context, out ErrCode, out ErrMsg);
        }

        public MemberModel.MemberInfoResult GetMemberInfo<T>(IMemberRead read, T context, out Int32 ErrCode, out String ErrMsg)
        {
            //--------------------------------
            // 1. 회원 프로필 조회
            //--------------------------------
            return read.GetMemberInfo(context, out ErrCode, out ErrMsg);
        }

        #endregion

        #region create

        public bool SetAuthCode<T>(IMemberCreate create, T context, out Int32 ErrCode, out String ErrMsg, out String AuthCode)
        {
            //--------------------------------
            // 1. 보안코드
            //--------------------------------
            return create.SetAuthCode(context, out ErrCode, out ErrMsg, out AuthCode);
        }

        public void SetFollow<T>(IMemberCreate create, T context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 1. 팔로우,팔로워 추가
            //--------------------------------
            create.SetFollow(context, out ErrCode, out ErrMsg);
        }

        #endregion

        #region update

        public void SetMemberInfo(MemberModel.MemberInfosRequest request, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 1. 회원프로필 있으면 업로드
            // 2. DB저장
            // 3. 회원프로필 새로등록이면 기존꺼 삭제
            //--------------------------------

            ErrCode = 0;
            ErrMsg = string.Empty;

            string Path = string.Empty, VersionId = string.Empty, fileName = string.Empty,
            fileDate = DateTime.Now.ToString("yyyyMMddHHmmss");

            IAws upload = new AwsMyinfo();

            Task Profile = upload.S3upload(request.Files, fileDate);
            Task.WaitAll(Profile);


            var deleteContext = new MemberModel.MemberInfoContext
            {
                MemberId = request.MemberId,
                TokenGuid = request.TokenGuid
            };

            if (request.Files != null)
            {
                foreach(var file in request.Files)
                {
                    fileName = file.FileName;
                }

                IMemberDelete delete = new MemberDelete();
                var data = delete.GetMemberDelete(deleteContext, out ErrCode, out ErrMsg);
                if(ErrCode == 0)
                {
                    upload.SetS3Delete(data.Key, data.VersionId);
                }               
            }

            if (ErrCode == 0)
            {              
                var s3Path = new StringBuilder();
                if (request.Files != null)
                {                    
                    var _s3 = upload.GetS3();
                    s3Path.Append(_s3.Domain);
                    s3Path.Append(_s3.Path);
                    s3Path.Append(fileDate);
                    s3Path.Append("_");
                    s3Path.Append(fileName);
                }
                Path = s3Path.ToString();

                var context = new MemberModel.MemberInfoSetContext
                {
                    MemberId = request.MemberId,
                    TokenGuid = request.TokenGuid,
                    NickNm = request.NickNm,
                    Age = request.Age,
                    Birth = request.Birth,
                    Gender = request.Gender,
                    Profile = Path,
                    SaveNm = request.Files == null ? null : fileDate + "_" + fileName,
                    VersionId = VersionId
                };

                IMemberUpdate update = new MemberUpdate();
                update.SetMemberInfo(context, out ErrCode, out ErrMsg);
            }
        }

        #endregion

        #region delete

        public void SetFollowDisconnect<T>(IMemberDelete delete, T context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 1. 팔로우,팔로워 해제
            //--------------------------------
            delete.SetFollowDisconnect(context, out ErrCode, out ErrMsg);
        }

        #endregion
    }
}
