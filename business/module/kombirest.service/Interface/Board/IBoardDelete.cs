using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Interface.Board
{
    public interface IBoardDelete
    {
        void SetBoardDelete<T>(T context, out int ErrCode, out string ErrMsg);
    }
}
