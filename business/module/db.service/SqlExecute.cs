using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.service
{
    public class SqlExecute : SqlFactory, ISqlExecute
    {
        private static readonly Lazy<SqlExecute> _sql = new Lazy<SqlExecute>(() => new SqlExecute());
        public static SqlExecute _Instance { get { return _sql.Value; } }

        #region Get

        /// <summary>
        /// 저장프로시저를 호출하여 하나의 결과셋을 리턴
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procName">프로시저</param>
        /// <param name="param">파라미터</param>
        /// <param name="dbConn">연결 DB 정보</param>
        /// <returns>한 로우의 데이터</returns>
        public T ExecuteProcWithParamsSingle<T>(string procName, dynamic param, Dictionary<ConnectionType, string> dbConn, out Int32 ErrCode, out String ErrMsg)
        {
            object mRow = new object();

            using (var connection = new SqlConnection(dbConn[0].ToString()))
            {
                var dPara = new DynamicParameters();

                dPara.AddDynamicParams(param);
                dPara.Add("ErrCode", 0, DbType.Int32, ParameterDirection.InputOutput);
                dPara.Add("ErrMsg", "", DbType.String, ParameterDirection.InputOutput);

                mRow = connection.Query<T>(procName, dPara, commandType: CommandType.StoredProcedure).FirstOrDefault();

                ErrCode = dPara.Get<Int32>("@ErrCode");
                ErrMsg = dPara.Get<String>("@ErrMsg");
            }
            return (T)mRow;
        }

        /// <summary>
        /// 스토어 프로시저 호출
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procname">프로시저 이름</param>
        /// <param name="param">파라미터</param>
        /// <param name="dbConn">연결 DB 정보</param>
        /// <returns>리스트 셋</returns>
        public List<T> ExecuteProcWithParams<T>(string procname, dynamic param, Dictionary<ConnectionType, string> dbConn, out Int32 ErrCode, out string ErrMsg)
        {
            List<T> result = new List<T>();

            using (var connection = new SqlConnection(dbConn[0].ToString()))
            {
                var dPara = new DynamicParameters();

                dPara.AddDynamicParams(param);
                dPara.Add("ErrCode", 0, DbType.Int32, ParameterDirection.InputOutput);
                dPara.Add("ErrMsg", "", DbType.String, ParameterDirection.InputOutput);

                result = connection.Query<T>(procname, dPara, commandType: CommandType.StoredProcedure).ToList();

                ErrCode = dPara.Get<Int32>("@ErrCode");
                ErrMsg = dPara.Get<String>("@ErrMsg");
            }

            return result;
        }

        /// <summary>
        /// 저장프로시저를 호출하여 여러개의 조회 데이터 리턴
        /// JSON List 형식으로 데이터를 넘겨준다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procName">프로시저</param>
        /// <param name="param">파라미터</param>
        /// <param name="dbConn">연결 DB 정보</param>
        /// <returns>한 로우의 데이터</returns>
        public List<dynamic> ExecuteProcWithMultiple(string procName, dynamic param, Dictionary<ConnectionType, string> dbConn, out Int32 ErrCode, out string ErrMsg)
        {
            var result = new List<dynamic>();
            ErrCode = 0;
            ErrMsg = string.Empty;

            using (var connection = new SqlConnection(dbConn[0].ToString()))
            {
                var dPara = new DynamicParameters();

                dPara.AddDynamicParams(param);
                dPara.Add("ErrCode", 0, DbType.Int32, ParameterDirection.InputOutput);
                dPara.Add("ErrMsg", "", DbType.String, ParameterDirection.InputOutput);

                var obj = connection.QueryMultiple(procName, dPara, commandType: CommandType.StoredProcedure);

                while (!obj.IsConsumed)
                {
                    var firstRow = JsonConvert.SerializeObject(obj.Read().ToList());
                    if (firstRow == "[]")
                    {
                        firstRow = null;
                    }
                    result.Add(firstRow);

                    ErrCode = dPara.Get<Int32>("@ErrCode");
                    ErrMsg = dPara.Get<String>("@ErrMsg");
                }
            }
            return result;
        }

        #endregion

        #region Set

        /// <summary>
        /// 추가, 삭제, 업데이트 프로시저(공통 리턴 값 리턴 out파라미터 추가)
        /// </summary>
        /// <param name="procName">프로시져명</param>
        /// <param name="param">파라미터</param>
        /// <param name="dbConn">연결 DB 정보</param>
        /// <param name="outParams">결과값 이외의 OUT 파라미터 값</param>
        /// <returns>프로시져 실행 결과</returns>
        public bool ExecuteInsertUpdateOrDeleteProcOutParams(string procName, dynamic param, Dictionary<ConnectionType, string> dbConn, out Int32 ErrCode, out string ErrMsg)
        {
            var result = false;
            var dPara = new DynamicParameters();

            dPara.AddDynamicParams(param);
            dPara.Add("ErrCode", 0, DbType.Int32, ParameterDirection.InputOutput);
            dPara.Add("ErrMsg", "", DbType.String, ParameterDirection.InputOutput);

            using (var connection = new SqlConnection(dbConn[0].ToString()))
            {
                connection.Execute(procName, dPara, commandType: CommandType.StoredProcedure);

                ErrCode = dPara.Get<Int32>("@ErrCode");
                ErrMsg = dPara.Get<String>("@ErrMsg");

                if (ErrCode == 0)
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// 추가, 삭제, 업데이트 프로시저(공통 리턴 값 리턴 out파라미터 추가)
        /// </summary>
        /// <param name="procName">프로시져명</param>
        /// <param name="param">파라미터</param>
        /// <param name="dbConn">연결 DB 정보</param>
        /// <param name="outParams">결과값 이외의 OUT 파라미터 값</param>
        /// <returns>프로시져 실행 결과</returns>
        public bool ExecuteInsertUpdateOrDeleteProcOutIdxParams(string procName, dynamic param, Dictionary<ConnectionType, string> dbConn, out Int32 ErrCode, out string ErrMsg, out Int64 RowIdx)
        {
            var result = false;
            var dPara = new DynamicParameters();

            dPara.AddDynamicParams(param);
            dPara.Add("ErrCode", 0, DbType.Int32, ParameterDirection.InputOutput);
            dPara.Add("ErrMsg", "", DbType.String, ParameterDirection.InputOutput);
            dPara.Add("RowIdx", 0, DbType.Int64, ParameterDirection.InputOutput);

            using (var connection = new SqlConnection(dbConn[0].ToString()))
            {
                connection.Execute(procName, dPara, commandType: CommandType.StoredProcedure);

                ErrCode = dPara.Get<Int32>("@ErrCode");
                ErrMsg = dPara.Get<String>("@ErrMsg");
                RowIdx = dPara.Get<Int64>("@RowIdx");

                if (ErrCode == 0)
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// 추가, 삭제, 업데이트 프로시저(공통 리턴 값 리턴 out파라미터 추가)
        /// AuthCode out파라미터 추가를 위해 오버로딩
        /// </summary>
        public bool ExecuteInsertUpdateOrDeleteProcOutParams(string procName, dynamic param, Dictionary<ConnectionType, string> dbConn,
                                                                            out Int32 ErrCode, out string ErrMsg, out string AuthCode)
        {
            var result = false;
            var dPara = new DynamicParameters();

            dPara.AddDynamicParams(param);
            dPara.Add("ErrCode", 0, DbType.Int32, ParameterDirection.InputOutput);
            dPara.Add("ErrMsg", "", DbType.String, ParameterDirection.InputOutput);
            dPara.Add("AuthCode", "", DbType.String, ParameterDirection.InputOutput);

            using (var connection = new SqlConnection(dbConn[0].ToString()))
            {
                connection.Execute(procName, dPara, commandType: CommandType.StoredProcedure);

                ErrCode = dPara.Get<Int32>("@ErrCode");
                ErrMsg = dPara.Get<String>("@ErrMsg");
                AuthCode = dPara.Get<String>("@AuthCode");

                if (ErrCode == 0)
                {
                    result = true;
                }
            }

            return result;
        }

        #endregion

        #region Custom

        public void ExecuteActivity(string procName, dynamic param, Dictionary<ConnectionType, string> dbConn, out Int32 ErrCode, out string ErrMsg, out Int64 RowIdx)
        {
            var dPara = new DynamicParameters();

            dPara.AddDynamicParams(param);
            dPara.Add("ErrCode", 0, DbType.Int32, ParameterDirection.InputOutput);
            dPara.Add("ErrMsg", "", DbType.String, ParameterDirection.InputOutput);
            dPara.Add("RowIdx", 0, DbType.Int64, ParameterDirection.InputOutput);

            using (var connection = new SqlConnection(dbConn[0].ToString()))
            {
                connection.Execute(procName, dPara, commandType: CommandType.StoredProcedure);

                ErrCode = dPara.Get<Int32>("@ErrCode");
                ErrMsg = dPara.Get<String>("@ErrMsg");
                RowIdx = dPara.Get<Int64>("@RowIdx");

                if (ErrCode == 0 && param.Items.Count > 0 && RowIdx > 0)
                {
                    dPara = new DynamicParameters();
                    foreach (var data in param.Items)
                    {
                        dPara.Add("@ActIdx", RowIdx);
                        dPara.Add("@ItemNm", data.ItemNm);
                        dPara.Add("@PageUrl", data.PageUrl);
                        dPara.Add("ErrCode", 0, DbType.Int32, ParameterDirection.InputOutput);
                        dPara.Add("ErrMsg", "", DbType.String, ParameterDirection.InputOutput);

                        connection.Execute("P_ItemInsert_SET", dPara, commandType: CommandType.StoredProcedure);

                        ErrCode = dPara.Get<Int32>("@ErrCode");
                        ErrMsg = dPara.Get<String>("@ErrMsg");

                        if (ErrCode != 0)
                        {
                            break;
                        }                        
                    }
                }
            }
        }

        public void ExecuteProduct(string procName, dynamic param, Dictionary<ConnectionType, string> dbConn, string member, string Domain, string Path, string fileDate,
            out Int32 ErrCode, out string ErrMsg)
        {
            ErrCode = 0;
            ErrMsg = string.Empty;

            using (var connection = new SqlConnection(Instance(ConnectionType.DBCon)[0].ToString()))
            {
                var dPara = new DynamicParameters();
                int count = 0, idx = 0;

                var s3Path = new StringBuilder();

                foreach (var tmp in param.files)
                {
                    s3Path.Append(Domain);
                    s3Path.Append(Path);
                    s3Path.Append(fileDate);
                    s3Path.Append("_");
                    s3Path.Append(tmp.FileName);

                    dPara.Add("@PrdIdx", idx);
                    dPara.Add("@PrdImg", s3Path.ToString());
                    dPara.Add("@PrdType", tmp.ContentType);
                    dPara.Add("@ActIdx", param.key);
                    dPara.Add("@PrdContent", param.contents);
                    dPara.Add("@PrdNm", param.title);
                    dPara.Add("@MemberId", member);
                    dPara.Add("@FilePath", s3Path.ToString());
                    dPara.Add("@FileRealName", tmp.FileName);
                    dPara.Add("@FileSaveName", tmp.FileName);
                    dPara.Add("@FileSize", tmp.Length);
                    dPara.Add("@FileType", tmp.ContentType);
                    dPara.Add("ErrCode", 0, DbType.Int32, ParameterDirection.InputOutput);
                    dPara.Add("ErrMsg", "", DbType.String, ParameterDirection.InputOutput);

                    connection.Execute("P_Product_SET", dPara, commandType: CommandType.StoredProcedure);

                    ErrCode = dPara.Get<Int32>("@ErrCode");
                    ErrMsg = dPara.Get<String>("@ErrMsg");

                    if (ErrCode < 0)
                    {
                        break;
                    }

                    if (count == 0 && ErrCode > 0)
                    {
                        idx = ErrCode;
                    }
                    count++;
                    dPara = new DynamicParameters();
                    s3Path.Length = 0;
                }                
            }
        }

        #endregion
    }
}
