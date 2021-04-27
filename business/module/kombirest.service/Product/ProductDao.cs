using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using db.service;
using Microsoft.AspNetCore.Http;
using Model.kombirest.model.Product;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon;
using Microsoft.Extensions.Configuration;
using kombirest.service.Commons;
using Model.kombirest.model.Common;
using Microsoft.AspNetCore.Http.Internal;
using Amazon.S3.Model;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using System.Reflection.Metadata.Ecma335;
using Dapper;
using System.Data.SqlClient;
using System.Data;
using Model.kombirest.model.Activity;

namespace kombirest.service.Product
{
    public class ProductDao : SqlExecute, IProductInsert, IProductUpdate, IProduct, IProductDelete
    {
        private AwsModel.S3Context _s3;

        public ProductDao()
        {
            _s3 = new AwsModel.S3Context();
            GetS3();
        }

        public AwsModel.S3Context GetS3()
        {
            IConfigurationSection data = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("s3Connection");
            _s3.Bucket = data["bucket"].ToString();
            _s3.AccessKey = data["access"].ToString();
            _s3.SecretKey = data["secret"].ToString();
            _s3.Path = data["path"].ToString();
            _s3.Domain = data["domain"].ToString();
            return _s3;
        }



        #region List
        public ProductModel.PrdDetailResult GetProductDetail(ProductModel.PrdDetailSingleRequest context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 게시물상세 조회 
            // 각테이블별로 키기준 조회하여 json형태로 저장
            // 굳이 SP에서 조인하는등의 복잡한 쿼리를 하지않는다.
            //--------------------------------

            var result = new ProductModel.PrdDetailResult();
            var detailData = _Instance.ExecuteProcWithMultiple("P_ProductDetail_GET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);

            if (detailData[0] != null) result.Product = JsonConvert.DeserializeObject<List<ProductModel.PrdResult>>(detailData[0])[0];
            if (detailData[1] != null) result.Options = JsonConvert.DeserializeObject<List<ProductModel.PrdOptionResult>>(detailData[1]);
            if (detailData[2] != null) result.Board = JsonConvert.DeserializeObject<List<ProductModel.PrdBoardResult>>(detailData[2]);
            if (detailData[3] != null) result.Activity = JsonConvert.DeserializeObject<List<ProductModel.PrdActiveResult>>(detailData[3]);
            if (detailData[4] != null) result.Items = JsonConvert.DeserializeObject<List<ProductModel.PrdActiveItemResult>>(detailData[4]);


            return result;
        }

        public List<ProductModel.PrdResult> GetProductList(ProductModel.PrdListContext context, out int ErrCode, out string ErrMsg)
        {
            return _Instance.ExecuteProcWithParams<ProductModel.PrdResult>("P_ProductList_GET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }
        #endregion

        #region Insert

        public void SetProduct(ProductModel.PrdAddRequest request, string fileDate, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 1. 이미지 s3 업로드
            // 2. 아이템,이미지 DB목록 파싱
            // 3. DB INSERT
            //--------------------------------

            ErrCode = -1;
            ErrMsg = "시스템오류";
            //string fileDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            //IAws upload = new AwsProduct();
            //Task contextfiles = upload.S3upload(request.files, fileDate);
            Task contextfiles = S3upload(request.files, fileDate);
            Task<ActivityModel.ItemInsertContext> items = GetItems(request.items);            
            Task<ProductModel.PrdFileUpdateContext> options = GetOptions(request.files);

            Task.WaitAll(contextfiles, items, options);
            //Task.WaitAll(contextfiles);

            if (items != null && options != null)
            {
                var s3Path = new StringBuilder();
                s3Path.Append(_s3.Domain);
                s3Path.Append(_s3.Path);
                s3Path.Append(fileDate);

                var context = new ProductModel.ProductSetContext
                {
                    ActNm = request.actnm,
                    PrdContent = request.contents,
                    PrdNm = request.title,
                    PrdImg = request.files.Select(c => c.FileName).FirstOrDefault(),
                    PrdType = request.files.Select(c => c.ContentType).FirstOrDefault(),
                    MemberId = request.memberId,
                    ItemUrl = items.Result.PageUrl,
                    ItemModel = items.Result.Model,
                    ItemName = items.Result.ItemNm,
                    OptionFileName = options.Result.OptionFileName,
                    OptionLength = options.Result.OptionLength,
                    OptionType = options.Result.OptionType,
                    Path = s3Path.ToString(),
                    SavePath = fileDate,
                    TokenGuid = request.Guid,
                    ConnectionType = 1
                };

                _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_Product_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
            }
        }

        public async Task S3upload(List<IFormFile> files, string fileDate)
        {
            //--------------------------------
            // s3 업로드
            //--------------------------------
            //_s3.AccessKey, _s3.SecretKey, RegionEndpoint.APNortheast2

            if(files != null && files.Count > 0)
            {
                var config = new AmazonS3Config
                {
                    Timeout = TimeSpan.FromSeconds(30),
                    RegionEndpoint = RegionEndpoint.APNortheast2
                };
                using var client = new AmazonS3Client(_s3.AccessKey, _s3.SecretKey, config);
                var fileTransferUtility = new TransferUtility(client);
                var s3Path = new StringBuilder();

                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        //var filePath = Path.GetTempFileName();
                        s3Path.Append(_s3.Path);
                        s3Path.Append(fileDate);
                        s3Path.Append("_");
                        s3Path.Append(formFile.FileName);

                        //using (Stream fileStream = new FileStream("C:/Logs", FileMode.Create))
                        //{
                        //await formFile.CopyToAsync(fileStream);
                        //}

                        //using (var memoa = new MemoryStream())
                        //{
                        //    await formFile.CopyToAsync(memoa);
                        //}
                        //var filePath = Path.GetTempFileName();
                        //var filePath = Path.Combine("C:\\Logs");


                        //await formFile.CopyToAsync(stream);

                        //using (var memoryStream = new MemoryStream())
                        //{
                        //    await formFile.CopyToAsync(memoryStream);

                        //    memoryStream.Position = 0;
                        //    var uploadRequest = new TransferUtilityUploadRequest
                        //    {
                        //        InputStream = memoryStream,
                        //        Key = s3Path.ToString(),
                        //        BucketName = _s3.Bucket,
                        //        CannedACL = S3CannedACL.PublicRead
                        //    };
                        //    fileTransferUtility = new TransferUtility(client);
                        //    await fileTransferUtility.UploadAsync(uploadRequest);

                        //    //if (memoryStream.Length > 0)
                        //    //{
                        //    //    var uploadRequest = new TransferUtilityUploadRequest
                        //    //    {
                        //    //        InputStream = memoryStream,
                        //    //        Key = s3Path.ToString(),
                        //    //        BucketName = _s3.Bucket,
                        //    //        CannedACL = S3CannedACL.PublicRead
                        //    //    };
                        //    //    fileTransferUtility = new TransferUtility(client);
                        //    //    await fileTransferUtility.UploadAsync(uploadRequest);
                        //    //}
                        //}



                        using (var memoryStream = new MemoryStream())
                        {
                            await formFile.CopyToAsync(memoryStream);
                            var uploadRequest = new TransferUtilityUploadRequest
                            {
                                InputStream = memoryStream,
                                Key = s3Path.ToString(),
                                BucketName = _s3.Bucket,
                                CannedACL = S3CannedACL.PublicRead
                            };
                            fileTransferUtility = new TransferUtility(client);
                            await fileTransferUtility.UploadAsync(uploadRequest);
                        }

                        //using (var stream = System.IO.File.Create(filePath))
                        //{
                        //    await formFile.CopyToAsync(stream);

                        //    var uploadRequest = new TransferUtilityUploadRequest
                        //    {
                        //        InputStream = stream,
                        //        Key = s3Path.ToString(),
                        //        BucketName = _s3.Bucket,
                        //        CannedACL = S3CannedACL.PublicRead
                        //    };
                        //    fileTransferUtility = new TransferUtility(client);
                        //    await fileTransferUtility.UploadAsync(uploadRequest);
                        //}





                        //using (var newMemoryStream = new MemoryStream())
                        //{
                        //var buf = new byte[newMemoryStream.Length];
                        //newMemoryStream.Read(buf, 0, buf.Length);

                        //var asdasd = new PutObjectRequest
                        //{
                        //    InputStream = newMemoryStream,
                        //    Key = s3Path.ToString(),
                        //    BucketName = _s3.Bucket,
                        //    CannedACL = S3CannedACL.PublicRead,
                        //    ContentType = formFile.ContentType
                        //};

                        //using (var fileToUpload = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        //{
                        //    await fileTransferUtility.UploadAsync(fileToUpload, _s3.Bucket, s3Path.ToString());
                        //}

                        //await formFile.CopyToAsync(newMemoryStream);

                        //var uploadRequest = new TransferUtilityUploadRequest
                        //{
                        //    InputStream = newMemoryStream,
                        //    Key = s3Path.ToString(),
                        //    BucketName = _s3.Bucket,
                        //    CannedACL = S3CannedACL.PublicRead
                        //};
                        //fileTransferUtility = new TransferUtility(client);
                        //await fileTransferUtility.UploadAsync(uploadRequest);
                        //}

                    }
                    s3Path = new StringBuilder();
                }
            }            
        }

        public async Task PrdInsert(List<IFormFile> files, string fileDate)
        {
            //--------------------------------
            // s3 업로드
            //--------------------------------

            using var client = new AmazonS3Client(_s3.AccessKey, _s3.SecretKey, RegionEndpoint.APNortheast2);
            var fileTransferUtility = new TransferUtility(client);
            var s3Path = new StringBuilder();

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    ////var filePath = Path.GetTempFileName();
                    //var filePath = Path.Combine("C:Logs");

                    using (var stream = System.IO.File.Create(""))
                    {
                        await formFile.CopyToAsync(stream);
                    }


                    //using (var stream = new MemoryStream(Convert.FromBase64String(formFile.OpenReadStream().ToString())))
                    //{
                    //    //await formFile.CopyToAsync(stream);
                    //    var uploadRequest = new TransferUtilityUploadRequest
                    //    {
                    //        InputStream = stream,
                    //        Key = s3Path.ToString(),
                    //        BucketName = _s3.Bucket,
                    //        CannedACL = S3CannedACL.PublicRead
                    //    };
                    //    fileTransferUtility = new TransferUtility(client);
                    //    //await fileTransferUtility.UploadAsync(uploadRequest);

                    //}

                    //using var newMemoryStream = new MemoryStream();
                    //formFile.CopyTo(newMemoryStream);

                    //s3Path.Append(_s3.Path);
                    //s3Path.Append(fileDate);
                    //s3Path.Append("_");
                    //s3Path.Append(formFile.FileName);

                    //var uploadRequest = new TransferUtilityUploadRequest
                    //{
                    //    InputStream = stream,
                    //    Key = s3Path.ToString(),
                    //    BucketName = _s3.Bucket,
                    //    CannedACL = S3CannedACL.PublicRead
                    //};
                    //fileTransferUtility = new TransferUtility(client);
                    //await fileTransferUtility.UploadAsync(uploadRequest);

                    //PutObjectRequest request = new PutObjectRequest
                    //{
                    //    BucketName = _s3.Bucket,
                    //    Key = s3Path.ToString(),
                    //    ContentBody = "This is the content body!"
                    //};
                    //PutObjectResponse response = await client.PutObjectAsync(request);
                    //var sd = response.VersionId;
                }
                s3Path.Length = 0;
            }
        }

        public async Task<Dictionary<int, string>> PrdInsert(ProductModel.PrdAddRequest request, string fileDate, string member)
        {
            //--------------------------------
            // 첫번째 SetPrd : 게시물관련저장
            // 두번째 SetPrd : 활동관련저장 (현재 쓰지않음 로직변경)
            //--------------------------------

            int ErrCode = 0;
            string ErrMsg = string.Empty;
            var result = new Dictionary<int, string>();

            var task = new Task<Dictionary<int, string>>(() =>
            {
                SetPrd(request, fileDate, member, out ErrCode, out ErrMsg);
                result.Add(ErrCode, ErrMsg);
                return result;
            });
            task.Start();
            await task;

            return result;
        }

        public void SetPrd(ProductModel.PrdAddRequest context, string fileDate, string member, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // s3에 업로드된 이미지정보를 DB에 저장한다. (사용하지않음)
            //--------------------------------

            ErrCode = 0;
            ErrMsg = string.Empty;

            //_Instance.ExecuteProduct("P_Product_SET", context, Instance(ConnectionType.DBCon),member, _s3.Domain, _s3.Path, fileDate,  out ErrCode, out ErrMsg);

            using (var connection = new SqlConnection(Instance(ConnectionType.DBCon)[0].ToString()))
            {
                var dPara = new DynamicParameters();
                var data = new ProductModel.PrdInsertContext();
                int count = 0, idx = 0;

                var s3Path = new StringBuilder();

                foreach (var tmp in context.files)
                {
                    s3Path.Append(_s3.Domain);
                    s3Path.Append(_s3.Path);
                    s3Path.Append(fileDate);
                    s3Path.Append("_");
                    s3Path.Append(tmp.FileName);

                    data.PrdIdx = idx;
                    data.PrdImg = s3Path.ToString();
                    data.PrdType = tmp.ContentType;
                    data.ActIdx = context.key;
                    data.PrdContent = context.contents;
                    data.PrdNm = context.title;
                    data.MemberId = member;
                    data.FilePath = s3Path.ToString();
                    data.FileRealName = tmp.FileName;
                    data.FileSaveName = tmp.FileName;
                    data.FileSize = tmp.Length;
                    data.FileType = tmp.ContentType;

                    dPara.AddDynamicParams(data);
                    dPara.Add("ErrCode", 0, DbType.Int32, ParameterDirection.InputOutput);
                    dPara.Add("ErrMsg", "", DbType.String, ParameterDirection.InputOutput);

                    connection.Execute("P_Product_SET", dPara, commandType: CommandType.StoredProcedure);

                    ErrCode = dPara.Get<Int32>("@ErrCode");
                    ErrMsg = dPara.Get<String>("@ErrMsg");

                    if (ErrCode < 0)
                    {
                        break;
                    }

                    if (count == 0 && ErrCode > 0)
                    {
                        idx = ErrCode;
                    }
                    count++;
                    dPara = new DynamicParameters();
                    s3Path.Length = 0;
                }
                ErrCode = 0;
            }
        }

        public void SetPrd(Int64 key, List<ActivityModel.ItemInsertContext> items, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 활동에 조합된 아이템들 복사 -> 삭제 -> 새로등록 순으로쿼리 (현재 사용하지않음)
            //--------------------------------

            ErrCode = 0;
            ErrMsg = string.Empty;
            using (var connection = new SqlConnection(Instance(ConnectionType.DBCon)[0].ToString()))
            {
                var dPara = new DynamicParameters();
                int count = 0;
                foreach (var data in items)
                {
                    dPara.Add("@SetType", count == 0 ? 0 : 1);
                    dPara.Add("@ActIdx", key);
                    dPara.Add("@ItemNm", data.ItemNm);
                    dPara.Add("@PageUrl", data.PageUrl);
                    dPara.Add("ErrCode", 0, DbType.Int32, ParameterDirection.InputOutput);
                    dPara.Add("ErrMsg", "", DbType.String, ParameterDirection.InputOutput);


                    connection.Execute("P_ProductActivity_SET", dPara, commandType: CommandType.StoredProcedure);

                    ErrCode = dPara.Get<Int32>("@ErrCode");
                    ErrMsg = dPara.Get<String>("@ErrMsg");

                    if (ErrCode != 0)
                    {
                        break;
                    }
                    dPara = new DynamicParameters();
                    count++;
                }
            }
        }

        public async Task<ActivityModel.ItemInsertContext> GetItems(List<ActivityModel.ItemInsertContext> items)
        {
            //--------------------------------
            // 아이템들 필드별로 하나의문자열로 파싱 "," 로 구분
            //--------------------------------

            var url = new StringBuilder();
            var model = new StringBuilder();
            var name = new StringBuilder();

            await Task.Run(() =>
            {
                int count = 1;
                int itemCount = items.Count;
                items.ForEach(tmp =>
                {
                    url.Append(tmp.PageUrl);
                    model.Append(tmp.Model);
                    name.Append(tmp.ItemNm);
                    if (itemCount > count)
                    {
                        url.Append(",");
                        model.Append(",");
                        name.Append(",");
                    }
                    count++;
                });
            });
            return new ActivityModel.ItemInsertContext
            {
                PageUrl = url.ToString(),
                Model = model.ToString(),
                ItemNm = name.ToString()
            };
        }

        public async Task<ProductModel.PrdFileUpdateContext> GetOptions(List<IFormFile> options)
        {
            //--------------------------------
            // 게시물이미지들 필드별로 하나의문자열로 파싱 "," 로 구분
            //--------------------------------

            var filename = new StringBuilder();
            var type = new StringBuilder();
            var length = new StringBuilder();

            await Task.Run(() =>
            {
                if(options != null && options.Count > 0)
                {
                    int count = 1;
                    int optionCount = options.Count;

                    options.ForEach(tmp =>
                    {
                        filename.Append(tmp.FileName);
                        type.Append(tmp.ContentType);
                        length.Append(tmp.Length);
                        if (optionCount > count)
                        {
                            filename.Append(",");
                            type.Append(",");
                            length.Append(",");
                        }
                        count++;
                    });
                }
            });
            return new ProductModel.PrdFileUpdateContext
            {
                OptionFileName = filename.ToString(),
                OptionLength = length.ToString(),
                OptionType = type.ToString()
            };
        }

        public async Task<ProductModel.PrdDeletesContext> GetDeletes(List<ProductModel.DeleteFileRequest> deletes)
        {
            //--------------------------------
            // 삭제할 옵션들 필드별로 하나의문자열로 파싱 "," 로 구분
            //--------------------------------

            var idx = new StringBuilder();
            await Task.Run(() =>
            {
                if(deletes != null && deletes.Count > 0)
                {
                    int count = 1;
                    int deleteCount = deletes.Count;
                    deletes.ForEach(tmp =>
                    {
                        idx.Append(tmp.Idx);

                        if (deleteCount > count)
                        {
                            idx.Append(",");
                        }
                        count++;
                    });
                }
            });
            return new ProductModel.PrdDeletesContext
            {
                DeleteIdx = idx.ToString()
            };
        }
        #endregion

        #region Update
        public void SetPrdUpdate(ProductModel.PrdUpdateRequest request, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 1. 이미지 s3 업로드, 아이템,이미지 DB목록 파싱 병렬처리            
            // 2. 기존 s3 이미지 조회
            // 3. DB UPDATE
            // 4. 기존 s3 객체 삭제
            //--------------------------------

            ErrCode = -1;
            ErrMsg = "시스템오류";
            string fileDate = DateTime.Now.ToString("yyyyMMddHHmmss");

            Task contextfiles = S3upload(request.Files, fileDate);
            Task<ActivityModel.ItemInsertContext> items = GetItems(request.Items);
            Task<ProductModel.PrdFileUpdateContext> options = GetOptions(request.Files);
            Task<ProductModel.PrdDeletesContext> deletes = GetDeletes(request.Deletes);

            Task.WaitAll(contextfiles,items, options, deletes);

            if (items != null)
            {                
                var s3Path = new StringBuilder();
                s3Path.Append(_s3.Domain);
                s3Path.Append(_s3.Path);
                s3Path.Append(fileDate);

                var optionsContext = new ProductModel.PrdDetailRequest
                {
                    Key = request.Key
                };
                var BeginResult = _Instance.ExecuteProcWithParams<ProductModel.PrdOptionResult>("P_ProductImage_GET", optionsContext, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);

                ProductModel.PrdUpdateContext context = new ProductModel.PrdUpdateContext
                {
                    PrdIdx = request.Key,
                    PrdContent = request.Contents,
                    PrdNm = request.Title,
                    PrdImg = request.Files == null ? null : request.Files.Select(c => c.FileName).FirstOrDefault(),
                    PrdType = request.Files == null ? null : request.Files.Select(c => c.ContentType).FirstOrDefault(),
                    ActNm = request.Actnm,
                    MemberId = request.memberId,
                    ItemUrl = items.Result.PageUrl,
                    ItemModel = items.Result.Model,
                    ItemName = items.Result.ItemNm,
                    OptionFileName = options.Result.OptionFileName,
                    OptionLength = options.Result.OptionLength,
                    OptionType = options.Result.OptionType,
                    DeleteIdx = deletes.Result.DeleteIdx,
                    Path = s3Path.ToString(),
                    SaveDate = fileDate,
                    TokenGuid = request.Guid
                };

                _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_ProductUpdate_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);

                if(ErrCode == 0 && request.Deletes != null && request.Deletes.Count > 0)
                {
                    //var AfterResult = _Instance.ExecuteProcWithParams<ProductModel.PrdOptionResult>("P_ProductImage_GET", optionsContext, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg); 
                    //using var client = new AmazonS3Client(_s3.AccessKey, _s3.SecretKey, RegionEndpoint.APNortheast2);
                    //BeginResult.ForEach(tmp =>
                    ////request.Deletes.ForEach(tmp =>
                    //{
                    //    DeleteObjectRequest request = new DeleteObjectRequest
                    //    {
                    //        BucketName = _s3.Bucket,
                    //        Key = _s3.Path + tmp.ImgSaveNm
                    //        //VersionId = versionID
                    //    };

                    //    client.DeleteObjectAsync(request);
                    //});
                }
            }
        }

        public void SetProductChoice(ProductModel.PrdChoiceRequest context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 좋아요 등록  
            //--------------------------------
            _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_ProductChoice_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }

        public void SetProductReport(ProductModel.PrdReportRequest context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 신고하기  
            //--------------------------------
            _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_ProductReport_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }

        #endregion

        #region Delete
        public void SetPrdDelete(ProductModel.PrdDetailRequest context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 1.기존 이미지 DB 조회
            // 2.DB에 DELETE            
            // 3.s3에 있는 객체 삭제
            //--------------------------------

            var optionsContext = new ProductModel.PrdDetailRequest
            {
                Key = context.Key
            };
            var s3Result = _Instance.ExecuteProcWithParams<ProductModel.PrdOptionResult>("P_ProductImage_GET", optionsContext, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);

            _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_ProductDelete_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);

            if (ErrCode == 0)
            {
                using var client = new AmazonS3Client(_s3.AccessKey, _s3.SecretKey, RegionEndpoint.APNortheast2);
                s3Result.ForEach(tmp =>
                {
                    DeleteObjectRequest request = new DeleteObjectRequest
                    {
                        BucketName = _s3.Bucket,
                        Key = _s3.Path + tmp.ImgSaveNm
                    };

                    client.DeleteObjectAsync(request);
                });
            }
        }

        public void SetDelete(List<IFormFile> files, string fileDate)
        {
            using var client = new AmazonS3Client(_s3.AccessKey, _s3.SecretKey, RegionEndpoint.APNortheast2);
            var s3Path = new StringBuilder();
            files.ForEach(tmp =>
            {
                s3Path.Append(_s3.Path);
                s3Path.Append(fileDate);
                s3Path.Append("_");
                s3Path.Append(tmp.FileName);

                DeleteObjectRequest request = new DeleteObjectRequest
                {
                    BucketName = _s3.Bucket,
                    Key = s3Path.ToString()                    
                };

                client.DeleteObjectAsync(request);
            });
        }

        #endregion        
    }
}
