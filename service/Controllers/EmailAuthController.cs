using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using kombirest.service.Commons;
//using kombirest.service.EmailAuth;
using kombirest.service.Interface.EmailAuth;
using kombirest.service.Interface.Member;
using kombirest.service.Member;
using kombirest.service.Service.Common;
using kombirest.service.Service.EmailAuth;
using kombirest.service.Service.Member;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model.Enum;
using Model.kombirest.model.Common;
using Model.kombirest.model.EmailAuth;

namespace kombirest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailAuthController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IEmailAuthDto _emailAuthDao;
        private readonly IMemberDto _member;

        private readonly ILogger<EmailAuthController> _logger;

        public EmailAuthController(IMemberDto member, IWebHostEnvironment env, IEmailAuthDto emailAuthDao, ILogger<EmailAuthController> logger)
        {
            _member = member;
            _env = env;
            _emailAuthDao = emailAuthDao;
            _logger = logger;
        }

        /// <summary>
        /// 비밀번호 변경 > 이메일 인증
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Post([FromForm] EmailAuthModel.EmailAuthContext request)
        {
            var result = new ResultContext();

            int ErrCode = 0;
            String ErrMsg = string.Empty;
            String AuthCode = string.Empty;
            try
            {
                // [1] 이메일 인증코드 생성
                IMemberCreate create = new MemberCreate();
                var resultContext = _member.SetAuthCode(create, request, out ErrCode, out ErrMsg, out AuthCode);

                // 생성 실패 시 
                if (AuthCode.Equals(string.Empty) || ErrCode != 0 || resultContext == false)
                {
                    result.Msg = ErrMsg;
                    result.Code = ErrCode;
                    return BadRequest(new { result });
                }

                // [2] 이메일 전송 
                var context = new SendEmailContext();

                context.MemberId = request.MemberId;
                context.AuthCode = AuthCode;
                context.ServerMapPath = _env.ContentRootPath;
                context.AuthType = request.AuthType;

                kombirest.service.Interface.Commons.IInfo _info = new Info();
                _info.SendEmailOfAuthCode(context, out ErrCode, out ErrMsg);

                // 전송 실패 시 
                if (ErrCode != 0)
                {
                    result.Msg = ErrMsg;
                    result.Code = ErrCode;
                    return BadRequest(new { result });
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message.ToString());
                result.Msg = "시스템 오류입니다. 잠시후 다시 시도해주세요.";
                result.Code = -500;
                return StatusCode(500, new { result });
            }

            result.Code = ErrCode;
            result.Msg = ErrMsg;
            return Ok(new { result });
        }

        [Route("[action]")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // 이메일 인증 검사 
        public IActionResult EmailAuthCheck([FromQuery] EmailAuthModel.EmailAuthCheckContext request)
        {
            var result = new ResultContext();
            int ErrCode = 0;
            String ErrMsg = string.Empty;
            String AuthCode = string.Empty;
            kombirest.service.Interface.Commons.ISecurity _security = new Security256();
            try
            {
                // 복호화 작업
                //request.AuthCode = _security.AESDecryptString(request.AuthCode);
                //request.MemberId = _security.AESDecryptString(request.MemberId);
                request.AuthCode = _security.DecryptString(request.AuthCode);
                request.MemberId = _security.DecryptString(request.MemberId);

                IEmailAuthRead emailread = new EmailAuthRead();
                var resultContext = _emailAuthDao.CheckAuthCode(emailread, request, out ErrCode, out ErrMsg);
                // 전송 실패 시 
                if (ErrCode != 0 || resultContext == false)
                {
                    result.Msg = ErrMsg;
                    result.Code = ErrCode;
                    return BadRequest(new { result });
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message.ToString());
                result.Msg = "시스템 오류입니다. 잠시후 다시 시도해주세요.";
                result.Code = -500;
                return StatusCode(500, new { result });
            }

            result.Code = ErrCode;
            result.Msg = ErrMsg;
            //return Ok(new { Value = result });
            if (AuthType.FindPwd.Equals(request.AuthType))
            {
#if !DEBUG
                return Redirect($"https://Kombirest.com/ResetPwd?MemberId={_security.EncryptString(request.MemberId)}");
#endif
                //return LocalRedirect($"/ResetPwd?MemberId={_security.AESEncryptString(request.MemberId)}");
                return LocalRedirect($"/ResetPwd?MemberId={_security.EncryptString(request.MemberId)}");
            }
            else
            {
#if !DEBUG
                return Redirect($"https://Kombirest.com/login");
#endif
                return LocalRedirect($"/login");
            }

        }
    }
}
