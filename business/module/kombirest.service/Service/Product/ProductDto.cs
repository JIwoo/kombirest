using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kombirest.service.Interface.Commons;
using kombirest.service.Service.Common;
using kombirest.service.Interface.Product;
using Model.kombirest.model.Activity;
using Model.kombirest.model.Product;
using static Model.kombirest.model.Product.ProductModel;

namespace kombirest.service.Service.Product
{
    public class ProductDto: IProductDto
    {
        public void SetProduct(PrdAddRequest request, string fileDate, string filePath, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 1. 이미지 s3 업로드
            // 2. 아이템,이미지 DB목록 파싱
            // 3. DB INSERT
            //--------------------------------

            ErrCode = -1;
            ErrMsg = "시스템오류";
            IAws upload = new AwsProduct();
            IProductCreate create = new ProductCreate();

            Task contextfiles = upload.S3upload(request.files, fileDate, filePath);
            Task<ActivityModel.ItemInsertContext> items = create.GetItems(request.items);
            Task<PrdFileUpdateContext> options = create.GetOptions(request.files);

            Task.WaitAll(contextfiles, items, options);
            if (items != null && options != null)
            {
                var s3Path = new StringBuilder();
                var s3SavePath = new StringBuilder();

                var s3 = upload.GetS3();
                s3Path.Append(s3.Domain);
                s3Path.Append(s3.Path);
                s3Path.Append(filePath);
                s3Path.Append("/");
                s3Path.Append(fileDate);

                s3SavePath.Append(filePath);
                s3SavePath.Append("/");
                s3SavePath.Append(fileDate);

                string fileName = create.GetParseFileName(request.files.Select(c => c.FileName).FirstOrDefault());
                var context = new ProductSetContext
                {
                    ActNm = request.actnm,
                    PrdContent = request.contents,
                    PrdNm = request.title,
                    PrdImg = fileName,
                    PrdType = request.files.Select(c => c.ContentType).FirstOrDefault(),
                    MemberId = request.memberId,
                    InterIdx = request.interCode,
                    ItemUrl = items.Result.PageUrl,
                    ItemModel = items.Result.Model,
                    ItemName = items.Result.ItemNm,
                    OptionFileName = options.Result.OptionFileName,
                    OptionLength = options.Result.OptionLength,
                    OptionType = options.Result.OptionType,
                    Path = s3Path.ToString(),
                    //SavePath = fileDate,
                    SavePath = s3SavePath.ToString(),
                    TokenGuid = request.Guid,
                    ConnectionType = 1
                };
                create.SetProduct(context, out ErrCode, out ErrMsg);
            }
        }

        public void SetPrdUpdate(PrdUpdateRequest request, out int ErrCode, out string ErrMsg)
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
            string filePath = DateTime.Now.ToString("yyyyMMdd");
            IAws upload = new AwsProduct();
            IProductCreate create = new ProductCreate();
            IProductDelete delete = new ProductDelete();

            Task contextfiles = upload.S3upload(request.Files, fileDate, filePath);
            Task<ActivityModel.ItemInsertContext> items = create.GetItems(request.Items);
            Task<PrdFileUpdateContext> options = create.GetOptions(request.Files);
            Task<PrdDeletesContext> deletes = delete.GetDeletes(request.Deletes);

            Task.WaitAll(contextfiles, items, options, deletes);

            if (items != null)
            {
                var optionsContext = new PrdDetailRequest
                {
                    Key = request.Key
                };

                IProductRead read = new ProductRead();
                var BeginResult = read.GetDeleteImage(optionsContext, out ErrCode, out ErrMsg);

                var s3Path = new StringBuilder();
                var s3SavePath = new StringBuilder();

                var s3 = upload.GetS3();
                s3Path.Append(s3.Domain);
                s3Path.Append(s3.Path);
                s3Path.Append(filePath);
                s3Path.Append("/");
                s3Path.Append(fileDate);

                s3SavePath.Append(filePath);
                s3SavePath.Append("/");
                s3SavePath.Append(fileDate);

                PrdUpdateContext context = new PrdUpdateContext
                {
                    PrdIdx = request.Key,
                    PrdContent = request.Contents,
                    PrdNm = request.Title,
                    ActNm = request.Actnm,
                    MemberId = request.memberId,
                    InterIdx = request.interCode,
                    ItemUrl = items.Result.PageUrl,
                    ItemModel = items.Result.Model,
                    ItemName = items.Result.ItemNm,
                    OptionFileName = options.Result.OptionFileName,
                    OptionLength = options.Result.OptionLength,
                    OptionType = options.Result.OptionType,
                    DeleteIdx = deletes.Result.DeleteIdx,
                    Path = s3Path.ToString(),
                    //SaveDate = fileDate,
                    SaveDate = s3SavePath.ToString(),
                    TokenGuid = request.Guid
                };

                IProductUpdate update = new ProductUpdate();
                update.SetPrdUpdate(context, out ErrCode, out ErrMsg);

                if (ErrCode == 0 && request.Deletes != null && request.Deletes.Count > 0)
                {
                    var AfterResult = read.GetDeleteImage(optionsContext, out ErrCode, out ErrMsg);

                    BeginResult.ForEach(tmp =>
                    {
                        if(!AfterResult.Select(c => c.ImgSaveNm).Contains(tmp.ImgSaveNm))
                        {
                            delete.SetDelete(s3, request);
                        }
                    });                    
                }
            }
        }

        public void SetProductReport<T>(IProductUpdate update, T context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 신고하기  
            //--------------------------------
            update.SetProductReport(context, out ErrCode, out ErrMsg);
        }

        public List<PrdResult> GetProductList<T>(IProductRead read, T context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 게시물 리스트 조회  
            //--------------------------------
            return read.GetProductList(context, out ErrCode, out ErrMsg);
        }

        public PrdDetailResult GetProductDetail<T>(IProductRead read, T context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 게시물상세 조회 
            // 각테이블별로 키기준 조회하여 json형태로 저장
            // 굳이 SP에서 조인하는등의 복잡한 쿼리를 하지않는다.
            //--------------------------------
            return read.GetProductDetail(context, out ErrCode, out ErrMsg);
        }

        public void SetProductChoice<T>(IProductUpdate update, T context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 좋아용 등록  
            //--------------------------------
            update.SetProductChoice(context, out ErrCode, out ErrMsg);
        }

        public void SetPrdDelete<T>(IProductDelete delete, T context, List<PrdOptionResult> s3Result, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 1.기존 이미지 DB 조회 
            // 2.DB에 DELETE            
            // 3.s3에 있는 객체 삭제
            //--------------------------------

            delete.SetDeleteDb(context, out ErrCode, out ErrMsg);
            if (ErrCode == 0)
            {
                IAws aws = new AwsProduct();                
                delete.SetDelete(aws.GetS3(), s3Result);
            }
        }
    }
}
