using Model.kombirest.model.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Product
{
    public interface IProductUpdate
    {
        void SetPrdUpdate(ProductModel.PrdUpdateRequest request, out int ErrCode, out string ErrMsg);

        void SetProductChoice(ProductModel.PrdChoiceRequest context, out int ErrCode, out string ErrMsg);

        void SetProductReport(ProductModel.PrdReportRequest context, out int ErrCode, out string ErrMsg);
    }
}
