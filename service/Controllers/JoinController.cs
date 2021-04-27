using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kombirest.service;
//using kombirest.service.Commons;
using kombirest.service.Interface.Commons;
using kombirest.service.Interface.Login;
using kombirest.service.Join;
using kombirest.service.Login;
using kombirest.service.Service.Common;
using kombirest.service.Service.Login;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.kombirest.model.Common;
using Model.kombirest.model.Join;

namespace kombirest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JoinController : ControllerBase
    {
        private readonly ILoginDto _login;
        private int _ErrCode = 0;
        private String _ErrMsg = string.Empty;

        public JoinController(ILoginDto login)
        {
            //_security = security;
            _login = login;
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // 회원가입
        public IActionResult Post([FromForm] JoinContext request)
        {
            var result = new ResultContext();
            try
            {
                //git test
                // 패스워드 암호화
                ISecurity _security = new Security256();
                request.MemberPwd = _security.GenerateString(request.MemberPwd);
                //request.MemberPwd = _security.GenerateSHA256String(request.MemberPwd);

                // 회원가입 처리 
                ILoginCreate create = new LoginCreate();
                var resultContext = _login.SetJoin(create,request, out _ErrCode, out _ErrMsg);

                // 회원가입 결과
                if (_ErrCode != 0 || resultContext == false)
                {
                    result.Msg = _ErrMsg;
                    result.Code = _ErrCode;
                    return StatusCode(400, result);
                }
            }
            catch 
            {
                result.Msg = "시스템 오류입니다. 잠시후 다시 시도해주세요.";
                result.Code = -500;
                return StatusCode(500, result);
            }

            result.Code = _ErrCode;
            result.Msg = _ErrMsg;
            return Ok(new { result });
        }
    }
}
