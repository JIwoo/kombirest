using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using db.service;
using kombirest.service.Commons;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Model.kombirest.model.Common;
using Model.kombirest.model.EmailAuth;
using Model.kombirest.model.Member;
using Newtonsoft.Json;
using static Model.kombirest.model.EmailAuth.EmailAuthModel;

namespace kombirest.service.Member
{
    public class MemberDao : SqlExecute, IMember, IMemberDelete, IMemberInsert, IMemberUpdate
    {
        private AwsModel.S3Context _s3;

        public MemberDao()
        {
            _s3 = new AwsModel.S3Context();
            GetS3();
        }
        #region list

        public MemberModel.MyListResult GetMyList(MemberModel.MyListRequest context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 1. 나의 게시물리스트 조회
            // 2. 팔로워,팔로잉 정보
            //--------------------------------
            var result = new MemberModel.MyListResult();

            var List = _Instance.ExecuteProcWithMultiple("P_MyList_GET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);

            if (List[0] != null) result.Info = JsonConvert.DeserializeObject<List<MemberModel.InfoResult>>(List[0])[0];
            if (List[1] != null) result.Product = JsonConvert.DeserializeObject<List<MemberModel.ProductResult>>(List[1]);

            return result;
        }

        public List<MemberModel.ProductResult> GetMyProduct(MemberModel.MyListRequest context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 1. 나의 게시물리스트 조회
            //--------------------------------
            return _Instance.ExecuteProcWithParams<MemberModel.ProductResult>("P_MyProduct_GET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }

        public MemberModel.MyListResult GetUserList(MemberModel.UserListRequest context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 1. 유저정보 ( 팔로우,팔로워 등도 포함?? )       
            // 2. 유저의 게시물리스트
            //--------------------------------
            var result = new MemberModel.MyListResult();

            var List = _Instance.ExecuteProcWithMultiple("P_FollowUserList_GET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);

            if (List[0] != null) result.Info = JsonConvert.DeserializeObject<List<MemberModel.InfoResult>>(List[0])[0];
            if (List[1] != null) result.Product = JsonConvert.DeserializeObject<List<MemberModel.ProductResult>>(List[1]);

            return result;

            //return _Instance.ExecuteProcWithParams<MemberModel.ProductResult>("P_UserList_GET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }

        public List<MemberModel.MyFollowResult> GetFollow(MemberModel.MyFollowRequest context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 1. 팔로잉 게시물 조회
            //--------------------------------
            return _Instance.ExecuteProcWithParams<MemberModel.MyFollowResult>("P_Follow_GET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }

        public List<MemberModel.MyFollowResult> GetUserFollow(MemberModel.UserFollowRequest context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 1. 팔로잉 게시물 조회
            //--------------------------------
            return _Instance.ExecuteProcWithParams<MemberModel.MyFollowResult>("P_UserFollow_GET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }

        public MemberModel.MemberInfoResult GetMemberInfo(MemberModel.MemberInfoContext context, out Int32 ErrCode, out String ErrMsg)
        {
            //--------------------------------
            // 1. 회원 프로필 조회
            //--------------------------------
            return _Instance.ExecuteProcWithParamsSingle<MemberModel.MemberInfoResult>("P_MemberInfo_GET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }

        #endregion

        #region insert

        public bool SetAuthCode(EmailAuthModel.EmailAuthContext context, out Int32 ErrCode, out String ErrMsg, out String AuthCode)
        {
            var resultContext = _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_AuthCode_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg, out AuthCode);
            return resultContext;
        }

        public void SetFollow(MemberModel.MyFollowSetRequest context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 1. 팔로우,팔로워 추가
            //--------------------------------
            _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_Follow_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }

        #endregion

        #region update

        public void SetMemberInfo(MemberModel.MemberInfoRequest request, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 1. 회원프로필 있으면 업로드
            // 2. DB저장
            // 3. 회원프로필 새로등록이면 기존꺼 삭제
            //--------------------------------

            ErrCode = 0;
            ErrMsg = string.Empty;

            string Path = string.Empty, VersionId = string.Empty,
            fileDate = DateTime.Now.ToString("yyyyMMddHHmmss");

            Task<string> Profile = SetProfile(request.Files, fileDate);
            Task.WaitAll(Profile);

            if (!string.IsNullOrEmpty(Profile.Result))
            {
                VersionId = Profile.Result;
            }

            var deleteContext = new MemberModel.MemberInfoContext
            {
                MemberId = request.MemberId,
                TokenGuid = request.TokenGuid
            };

            if (request.Files != null)
            {
                var data = _Instance.ExecuteProcWithParamsSingle<MemberModel.MemberProfileResult>("P_MemberInfoDelete_GET", deleteContext, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
                SetS3Delete(data.Key, data.VersionId);
            }
            
            if(ErrCode == 0)
            {
                var s3Path = new StringBuilder();
                if (request.Files != null)
                {
                    s3Path.Append(_s3.Domain);
                    s3Path.Append(_s3.Path);
                    s3Path.Append(fileDate);
                    s3Path.Append("_");
                    s3Path.Append(request.Files.FileName);
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
                    SaveNm = request.Files == null ? null : fileDate + "_" + request.Files.FileName,
                    VersionId = VersionId
                };
                _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_MemberInfo_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
            }
        }

        public async Task<string> SetProfile(IFormFile file, string fileDate)
        {
            PutObjectResponse response = new PutObjectResponse();
            if (file != null)
            {
                var config = new AmazonS3Config
                {
                    Timeout = TimeSpan.FromSeconds(30),
                    RegionEndpoint = RegionEndpoint.APNortheast2
                };

                using var client = new AmazonS3Client(_s3.AccessKey, _s3.SecretKey, config);
                var s3Path = new StringBuilder();
                s3Path.Append(_s3.Path);
                s3Path.Append(fileDate);
                s3Path.Append("_");
                s3Path.Append(file.FileName);

                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    PutObjectRequest request = new PutObjectRequest()
                    {
                        InputStream = memoryStream,
                        BucketName = _s3.Bucket,
                        Key = s3Path.ToString()
                    };
                    response = await client.PutObjectAsync(request);
                }
            }
            return response.VersionId;
        }

        #endregion

        #region delete

        public void SetFollowDisconnect(MemberModel.MyFollowDisconnectRequest context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 1. 팔로우,팔로워 해제
            //--------------------------------
            _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_FollowDisconnect_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }

        public void SetS3Delete(string Key, string VersionId)
        {
            using var client = new AmazonS3Client(_s3.AccessKey, _s3.SecretKey, RegionEndpoint.APNortheast2);            
            DeleteObjectRequest request = new DeleteObjectRequest
            {
                BucketName = _s3.Bucket,
                Key = _s3.Path + Key,
                //VersionId = VersionId
            };

            client.DeleteObjectAsync(request);
        }

        #endregion

        public AwsModel.S3Context GetS3()
        {
            IConfigurationSection data = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("s3Connection");
            _s3.Bucket = data["bucket"].ToString();
            _s3.AccessKey = data["access"].ToString();
            _s3.SecretKey = data["secret"].ToString();
            //_s3.Path = data["path"].ToString();
            _s3.Path = "profile/";
            _s3.Domain = data["domain"].ToString();
            return _s3;
        }
    }
}
