using System;
using System.Collections.Generic;
using System.Text;
using db.service;
using kombirest.service.Interface.Product;

namespace kombirest.service.Service.Product
{
    public class ProductUpdate: SqlExecute, IProductUpdate
    {
        public void SetPrdUpdate<T>(T context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 게시물 수정
            //--------------------------------
            _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_ProductUpdate_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }

        public void SetProductChoice<T>(T context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 좋아요 등록  
            //--------------------------------
            _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_ProductChoice_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }

        public void SetProductReport<T>(T context, out int ErrCode, out string ErrMsg)
        {
            //--------------------------------
            // 신고하기  
            //--------------------------------
            _Instance.ExecuteInsertUpdateOrDeleteProcOutParams("P_ProductReport_SET", context, Instance(ConnectionType.DBCon), out ErrCode, out ErrMsg);
        }
    }
}
