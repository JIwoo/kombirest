using System;
using System.Collections.Generic;
using System.Text;
using db.service;
using kombirest.service.Interface.Product;
using Model.kombirest.model.Product;
using Newtonsoft.Json;
using static Model.kombirest.model.Product.ProductModel;

namespace kombirest.service.Service.Product
{
    public class ProductRead: SqlExecute, IProductRead
    {
        public List<PrdResult> GetProductList<T>(T context, out int ErrCode, out string ErrMsg)
        {
            return _Instance.ExecuteProcWithParams<PrdResult>("P_ProductList_GET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }

        public PrdDetailResult GetProductDetail<T>(T context, out int ErrCode, out string ErrMsg)
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

        public List<PrdOptionResult> GetDeleteImage<T>(T context, out int ErrCode, out string ErrMsg)
        {
            return _Instance.ExecuteProcWithParams<PrdOptionResult>("P_ProductImage_GET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }
    }
}
