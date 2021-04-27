using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Interface.Product
{
    public interface IProductUpdate
    {
        void SetPrdUpdate<T>(T context, out int ErrCode, out string ErrMsg);

        void SetProductChoice<T>(T context, out int ErrCode, out string ErrMsg);

        void SetProductReport<T>(T context, out int ErrCode, out string ErrMsg);
    }
}
