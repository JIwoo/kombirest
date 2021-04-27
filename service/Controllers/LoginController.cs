using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kombirest.service;
using kombirest.service.Interface.Commons;
using kombirest.service.Interface.Login;
using kombirest.service.Service.Login;
//using kombirest.service.Login;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model.Enum;
using Model.kombirest.model.Common;
using Model.kombirest.model.Login;

namespace kombirest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]  
    public class LoginController : ControllerBase
    {
        private int _ErrCode = 0;
        private String _ErrMsg = string.Empty;
        //private readonly ILoginDao _login;
        private readonly ILoginDto _login;
        private readonly IAuth _auth;
        private readonly IHttpContextAccessor _accessor;
        private readonly ILogger<LoginController> _logger;
        public LoginController(ILoginDto login, IAuth auth, IHttpContextAccessor accessor, ILogger<LoginController> logger)
        {
            _login = login;
            _auth = auth;
            _accessor = accessor;
            _logger = logger;
        }

        /// <summary>
        /// 로그인 / 로그아웃 
        /// 로그아웃 일 경우에만 TokenGuid 사용
        /// </summary>
        /// <param name="MemberId">회원아이디</param>
        /// <param name="state">로그인, 로그아웃 상태값</param>
        /// <param name="TokenGuid">UUID</param>
        public void SetLoginLog (string MemberId, LoginStateType state)
        {
            try
            {
                var loginLogContext = new LoginLogModel.LoginLogContext();
                var os = Environment.OSVersion;
                loginLogContext.Agent = Request.Headers["User-Agent"].ToString();
                loginLogContext.LoginIP = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
                loginLogContext.Platform = os.Platform.ToString();
                loginLogContext.MemberId = MemberId;
                loginLogContext.State = state;

                // [1] 로그인 로그 쌓기
                ILoginCreate create = new LoginCreate();
                var logResult = _login.SetLoginLogDetail(create, loginLogContext, out _ErrCode, out _ErrMsg);

                if (_ErrCode != 0)
                {
                    return;
                }
            }
            catch (Exception e)
            {
                _ErrCode = -500;
                _ErrMsg = e.Message.ToString();
            }

            return;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // 로그인 
        public IActionResult Post([FromForm] LoginContext request)
        {
            var result = new Model.kombirest.model.Common.LoginResultContext();
            //string accessToken, refreshToken;
            Task<string> accessToken;
            string refreshToken;
            string nick, profile;
            
            try
            {
                // 로그인 처리 
                ILoginRead read = new LoginRead();
                var loginData = _login.LoginResultDetail(read, request, out _ErrCode, out _ErrMsg);
                // 로그인 성공
                if (_ErrCode == 0)
                {
                    nick = loginData.NickNm;
                    profile = loginData.Profile;
                    //var uuid = _ErrMsg;
                    //_ErrMsg = string.Empty;

                    //로그인 로그 쌓기
                    SetLoginLog(request.MemberId, LoginStateType.Login);

                    // 결과
                    if (_ErrCode != 0)
                    {
                        result.Msg = _ErrMsg;
                        result.Code = _ErrCode;
                        return StatusCode(400, result);
                    }
                    else
                    {
                        //accessToken = _auth.SetJwt(request.MemberId, null);
                        accessToken = _auth.SetFireBaseToken(request.MemberId);
                        Task.WaitAll(accessToken);
                        refreshToken = _auth.SetJwt(30, request.MemberId);
                        // test 
                        //var test = _auth.GetAccessToken(accessToken, out _ErrCode, out _ErrMsg);
                    }
                }
                // 로그인 실패
                else
                {
                    result.Msg = _ErrMsg;
                    result.Code = _ErrCode;
                    return StatusCode(400, result);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message.ToString());
                result.Msg = "시스템 오류입니다. 잠시후 다시 시도해주세요.";
                result.Code = -500;
                return StatusCode(500, result);
            }

            var cookieOptions = new CookieOptions()
            {
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddDays(14),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            };
            Response.Cookies.Append("x-ref-token", refreshToken, cookieOptions);

            result.Code = _ErrCode;
            result.Msg = _ErrMsg;
            result.AccessToken = accessToken.Result.ToString();
            //result.RefreshToken = refreshToken;
            result.NickName = nick;
            result.Profile = profile;
            return Ok(new { result });
        }        

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        // 로그아웃  
        public IActionResult Logout()
        {
            var result = new ResultContext();
            string accessToken;
            string outToken = "토큰만료";
            try
            {
                string authorization = Request.Headers["Authorization"].ToString() != null ? Request.Headers["Authorization"].ToString().Substring("Bearer ".Length).Trim()
                    : null;
                if (!string.IsNullOrEmpty(authorization) || authorization != "undefined")
                {
                    var Tokens = _auth.GetJwtPayload(authorization, out _ErrCode, out _ErrMsg, out accessToken);

                    if (Tokens != null && Tokens.Count() > 0 && _ErrCode == 0 && !string.IsNullOrEmpty(Tokens["id"].ToString()) && !string.IsNullOrEmpty(Tokens["uuid"].ToString()))
                    {
                        outToken = Tokens["id"].ToString();
                    }
                }


                // 로그아웃 로그 쌓기
                SetLoginLog(outToken, LoginStateType.Logout);
                Response.Cookies.Append("x-ref-token", "", new CookieOptions()
                {                    
                    Path = "/",
                    Expires = DateTime.Now.AddDays(-1),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                });

                // 결과
                if (_ErrCode != 0)
                {
                    result.Code = _ErrCode;
                    result.Msg = _ErrMsg;
                    return StatusCode(400, result);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message.ToString());
                result.Code = -500;
                result.Msg = "시스템 오류입니다. 잠시후 다시 시도해주세요."; ;
                return StatusCode(500, result);
            }

            return Ok(new { result });
        }

        [Route("[action]")]
        [HttpPost]

        // 로그인체크  
        public IActionResult LogCheck()
        {
            var result = new ResultContext();
            var token = new ResultPostContext();
            string accessToken;
            try
            {
                string authorization = Request.Headers["Authorization"].ToString().Substring("Bearer ".Length).Trim();
                string authCookie = Request.Cookies["x-ref-token"].ToString();

                if (string.IsNullOrEmpty(authorization) || string.IsNullOrWhiteSpace(authorization) || string.IsNullOrEmpty(authCookie) || string.IsNullOrWhiteSpace(authCookie))
                {
                    Response.Cookies.Append("x-ref-token", "", new CookieOptions()
                    {
                        Path = "/",
                        Expires = DateTime.Now.AddDays(-1),
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None
                    });
                    result.Msg = "로그인 후 이용 가능합니다.";
                    result.Code = -4001;
                    return StatusCode(401, result);
                }

                var Tokens = _auth.GetJwtPayload(authorization, authCookie, out _ErrCode, out _ErrMsg, out accessToken);

                if (Tokens == null || Tokens.Count() <= 0 || _ErrCode != 0
                    || string.IsNullOrEmpty(Tokens["id"].ToString())
                    || string.IsNullOrEmpty(Tokens["uuid"].ToString()))
                {
                    result.Msg = _ErrMsg;
                    result.Code = _ErrCode;
                    Response.Cookies.Append("x-ref-token", "", new CookieOptions()
                    {
                        Path = "/",
                        Expires = DateTime.Now.AddDays(-1),
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None
                    });
                    return StatusCode(401, result);
                }
                result.Msg = "갱신완료";
                result.Code = 0;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message.ToString());
                result.Code = -500;
                result.Msg = "시스템 오류입니다. 잠시후 다시 시도해주세요."; ;
                return StatusCode(500, result);
            }

            token.AccessToken = accessToken;
            result.Datas = token;
            return Ok(new { result });
        }
    }
}
