using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using db.service;
using kombirest.service.Commons;
using kombirest.service.Interface.Product;
using Microsoft.AspNetCore.Http;
using Model.kombirest.model.Activity;
using Model.kombirest.model.Product;
using static Model.kombirest.model.Product.ProductModel;

namespace kombirest.service.Service.Product
{
    public class ProductCreate: SqlExecute, IProductCreate
    {


        public void SetProduct<T>(T context, out int ErrCode, out string ErrMsg)
        {
            _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_Product_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
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
                        url.Append("§");
                        model.Append("§");
                        name.Append("§");
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

        public async Task<PrdFileUpdateContext> GetOptions(List<IFormFile> options)
        {            
            //--------------------------------
            // 게시물이미지들 필드별로 하나의문자열로 파싱 "," 로 구분
            //--------------------------------

            var filename = new StringBuilder();
            var type = new StringBuilder();
            var length = new StringBuilder();
            string regexName = string.Empty;

            await Task.Run(() =>
            {
                if (options != null && options.Count > 0)
                {
                    int count = 1;
                    int optionCount = options.Count;

                    options.ForEach(tmp =>
                    {
                        regexName = GetParseFileName(tmp.FileName);
                        //filename.Append(tmp.FileName);
                        filename.Append(regexName);
                        type.Append(tmp.ContentType);
                        length.Append(tmp.Length);
                        if (optionCount > count)
                        {
                            filename.Append("§");
                            type.Append("§");
                            length.Append("§");
                        }
                        count++;
                    });
                }
            });
            return new PrdFileUpdateContext
            {
                OptionFileName = filename.ToString(),
                OptionLength = length.ToString(),
                OptionType = type.ToString()
            };
        }

        public string GetParseFileName(string value)
        {
            return Regex.Replace(value, @"[^\w\.@-]", "", RegexOptions.Singleline).ToString();
        }
    }
}
