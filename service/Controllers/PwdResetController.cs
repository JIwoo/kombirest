using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kombirest.service;
using kombirest.service.PwdReset;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.Enum;
using Model.kombirest.model.PwdReset;
//using kombirest.service.Commons;
using Model.kombirest.model.Common;
using kombirest.service.Service.Common;
using kombirest.service.Interface.Commons;
using kombirest.service.Interface.PwdReset;
using kombirest.service.Service.PwdReset;

namespace kombirest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PwdResetController : ControllerBase
    {
        private readonly IPwdResetDto _pwd;

        public PwdResetController(IPwdResetDto PwdResetDao)
        {
            //_security = security;
            _pwd = PwdResetDao;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // 비밀번호 변경
        public IActionResult Post([FromForm] PwdResetContext request)
        {
            int ErrCode = 0;
            String ErrMsg = string.Empty;
            var result = new ResultContext();
            try
            {
                // 두 비밀번호가 동일한지 체크
                if (!request.Pwd.Equals(request.RePwd))
                {
                    result.Code = -1;
                    result.Msg = "비밀번호 값이 동일하지 않습니다.";
                    return StatusCode(400, result);
                }

                ISecurity _security = new Security256();

                // 더이상 사용하지 않으므로 빈값 초기화
                request.RePwd = string.Empty;
                // 새 비밀번호 암호화
                //request.Pwd = _security.GenerateSHA256String(request.Pwd);
                request.Pwd = _security.GenerateString(request.Pwd);

                // 로그인 없이 비밀번호 찾기 후 변경 
                if (request.ResetType == PwdResetType.AfterFind && !(string.IsNullOrEmpty(request.MemberId)))
                {
                    // 회원 아이디 복호화
                    //request.MemberId = _security.AESDecryptString(request.MemberId);
                    request.MemberId = _security.DecryptString(request.MemberId);
                }
                // 로그인 후 비밀번호 변경 
                else if (request.ResetType == PwdResetType.Normal && !(string.IsNullOrEmpty(request.OldPwd)))
                {
                    // 세션에서 아이디 가져오기 
                    //request.MemberId = HttpContext.Session.GetString("_userId");
                    // 기존 비밀번호 암호화
                    //request.OldPwd = _security.GenerateSHA256String(request.OldPwd);
                    request.OldPwd = _security.GenerateString(request.OldPwd);
                }
                else
                {
                    result.Code = -1;
                    result.Msg = "잘못된 접근입니다.";
                    return StatusCode(400, result);
                }

                // 비밀번호 변경

                IPwdResetUpdate update = new PwdResetUpdate();
                //var resultContext = _iPwdResetDao.SetPwdReset(request, out ErrCode, out ErrMsg);
                var resultContext = _pwd.SetPwdReset(update, request, out ErrCode, out ErrMsg);

                // 변경 실패 시 
                if (ErrCode != 0 || resultContext == false)
                {
                    result.Code = ErrCode;
                    result.Msg = ErrMsg;
                    return StatusCode(400, result);
                }

            }
            catch (Exception e)
            {
                result.Msg = "시스템 오류입니다. 잠시후 다시 시도해주세요.";
                result.Code = -500;
                return StatusCode(500, new { result });
            }

            return Ok(new { result });
        }
    }
}
