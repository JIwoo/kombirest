using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Interface.Board
{
    public interface IBoardCreate
    {
        void SetBoard<T>(T context, out int ErrCode, out string ErrMsg);
    }
}
