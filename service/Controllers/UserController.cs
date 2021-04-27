using kombirest.service.Interface.Commons;
using kombirest.service.Interface.User;
using kombirest.service.Service.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model.kombirest.model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Model.kombirest.model.User.UserModel;

namespace kombirest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private int _ErrCode = 0;
        private String _ErrMsg = string.Empty;


        private readonly IAuth _auth;
        private readonly ILogger<UserController> _logger;
        private readonly IUserDto _user;

        public UserController(ILogger<UserController> logger, IUserDto user, IAuth auth)
        {
            _logger = logger;
            _user = user;
            _auth = auth;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] UserSearchRequest request)
        {
            var result = new ResultContext();
            try
            {
                IUserRead read = new UserRead();

                var resultContext = _user.GetUserList(read, request, out _ErrCode, out _ErrMsg);
                result.Code = _ErrCode;
                result.Msg = _ErrMsg;
                result.Datas = resultContext;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message.ToString());
                result.Msg = "시스템 오류입니다. 잠시후 다시 시도해주세요.";
                result.Code = -500;
                return StatusCode(500, new { result });
            }
            return Ok(new { result });
        }

        [Route("[action]")]
        [HttpGet]
        public IActionResult MyFriend()
        {
            var result = new ResultContext();
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

                var context = new UserFriendListContext
                {
                    MemberId = Tokens["id"].ToString(),
                    TokenGuid = Tokens["uuid"].ToString(),
                };

                IUserRead read = new UserRead();
                result.Datas = _user.GetMyFriendList(read, context, out _ErrCode, out _ErrMsg);
                result.Msg = _ErrMsg;
                result.Code = _ErrCode;
                if (_ErrCode == -4008)
                {
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
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message.ToString());
                result.Msg = "시스템 오류입니다. 잠시후 다시 시도해주세요.";
                result.Code = -500;
                return StatusCode(500, new { result });
            }
            return Ok(new { result, accessToken });
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult UserActivityFollow([FromForm] UserSearchRequest request)
        {
            var result = new ResultContext();
            var token = new ResultPostContext();

            string accessToken;
            try
            {
                string authorization = Request.Headers["Authorization"].ToString().Substring("Bearer ".Length).Trim();
                var authCookie = Request.Cookies["x-ref-token"].ToString();

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

                var context = new UserFriendAddContext
                {
                    MemberId = Tokens["id"].ToString(),
                    TokenGuid = Tokens["uuid"].ToString(),
                    Nick = request.Nick
                };

                IUserCreate create = new UserCreate();
                _user.SetUserActivityFriend(create, context, out _ErrCode, out _ErrMsg);

                if (_ErrCode == -4008)
                {
                    Response.Cookies.Append("x-ref-token", "", new CookieOptions()
                    {
                        Path = "/",
                        Expires = DateTime.Now.AddDays(-1),
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None
                    });
                    result.Msg = _ErrMsg;
                    result.Code = _ErrCode;
                    return StatusCode(401, result);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message.ToString());
                result.Msg = "시스템 오류입니다. 잠시후 다시 시도해주세요.";
                result.Code = -500;
                return StatusCode(500, result);
            }

            token.AccessToken = accessToken;

            result.Code = _ErrCode;
            result.Msg = _ErrMsg;
            result.Datas = token;
            return Ok(new { result });
        }
    }
}
