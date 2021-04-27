using Model.kombirest.model;
using Model.kombirest.model.Join;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service.Join
{
    interface IJoinDao
    {
        /// <summary>
        /// 회원 회원가입 
        /// </summary>
        bool SetJoin(JoinContext context, out Int32 ErrCode, out String ErrMsg);
    }
}
