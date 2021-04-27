using System;
using System.Collections.Generic;
using System.Text;

namespace db.service
{
    interface ISqlExecute
    {
        /// <summary>
        /// 스토어 프로시저 호출 단일
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procname">프로시저 이름</param>
        /// <param name="param">파라미터</param>
        /// <param name="dbConn">접속 정보</param>
        /// <param name="ErrCode">에러코드</param>
        /// <param name="ErrMsg">에러메세지</param>
        /// <returns>리스트 셋</returns>
        T ExecuteProcWithParamsSingle<T>(string procName, dynamic param, Dictionary<ConnectionType, string> dbConn, out Int32 ErrCode, out String ErrMsg);

        /// <summary>
        /// 스토어 프로시저 호출 리스트
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procname">프로시저 이름</param>
        /// <param name="param">파라미터</param>
        /// <param name="dbConn">접속 정보</param>
        /// <param name="ErrCode">에러코드</param>
        /// <param name="ErrMsg">에러메세지</param>
        /// <returns>리스트 셋</returns>
        List<T> ExecuteProcWithParams<T>(string procname, dynamic param, Dictionary<ConnectionType, string> dbConn, out Int32 ErrCode, out string ErrMsg);
    }
}
