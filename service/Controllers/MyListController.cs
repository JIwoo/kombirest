using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.kombirest.model.Member;
//using kombirest.service.Member;
using Model.kombirest.model.Common;
using kombirest.service.Interface.Commons;
using Microsoft.Extensions.Logging;
using kombirest.service.Interface.Member;
using kombirest.service.Service.Member;

namespace kombirest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyListController : ControllerBase
    {
        private int _ErrCode = 0;
        private String _ErrMsg = string.Empty;
        private readonly IAuth _auth;
        private readonly IMemberDto _member;
        private readonly ILogger<MyListController> _logger;

        public MyListController(IMemberDto member, IAuth auth, ILogger<MyListController> logger)
        {
            _member = member;
            _auth = auth;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Get([FromQuery] MemberModel.MyListRequest request)
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

                if (Tokens == null || Tokens.Count() <= 0 || _ErrCode != 0 || string.IsNullOrEmpty(Tokens["id"].ToString()) || string.IsNullOrEmpty(Tokens["uuid"].ToString()))
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

                request.MemberId = Tokens["id"].ToString();
                request.TokenGuid = Tokens["uuid"].ToString();
                _logger.LogInformation(request.MemberId);
                IMemberRead read = new MemberRead();
                result.Datas = _member.GetMyList(read, request, out _ErrCode, out _ErrMsg);
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
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[IsLogin]
        public IActionResult Product([FromQuery] MemberModel.MyListRequest request)
        {
            var result = new ResultContext();
            string accessToken;
            try
            {
                //string authorization = Request.Headers["Authorization"].ToString() != null ? Request.Headers["Authorization"].ToString().Substring("Bearer ".Length).Trim() : null;
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

                if (Tokens == null || Tokens.Count() <= 0 || _ErrCode != 0 || string.IsNullOrEmpty(Tokens["id"].ToString()) || string.IsNullOrEmpty(Tokens["uuid"].ToString()))
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

                request.MemberId = Tokens["id"].ToString();
                request.TokenGuid = Tokens["uuid"].ToString();

                IMemberRead read = new MemberRead();
                result.Datas = _member.GetMyProduct(read, request, out _ErrCode, out _ErrMsg);
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
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[IsLogin]
        public IActionResult Follow([FromQuery] MemberModel.MyFollowRequest request)
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

                request.MemberId = Tokens["id"].ToString();
                request.TokenGuid = Tokens["uuid"].ToString();

                IMemberRead read = new MemberRead();
                result.Datas = _member.GetFollow(read, request, out _ErrCode, out _ErrMsg);
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
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[IsLogin]
        public IActionResult UserFollow([FromQuery] MemberModel.UserFollowRequest request)
        {
            var result = new ResultContext();
            string accessToken = string.Empty;
            try
            {
                string authorization = Request.Headers["Authorization"].ToString().Substring("Bearer ".Length).Trim();
                string authCookie = string.Empty;

                if (Request.Cookies["x-ref-token"] != null)
                {
                    authCookie = Request.Cookies["x-ref-token"].ToString();
                }

                if (!string.IsNullOrEmpty(authorization) && !string.IsNullOrWhiteSpace(authorization) && !string.IsNullOrEmpty(authCookie) && !string.IsNullOrWhiteSpace(authCookie))
                {
                    var Tokens = _auth.GetJwtPayload(authorization, out _ErrCode, out _ErrMsg, out accessToken);

                    if (Tokens != null || Tokens.Count() > 0 || _ErrCode == 0 || !string.IsNullOrEmpty(Tokens["id"].ToString()) || !string.IsNullOrEmpty(Tokens["uuid"].ToString()))
                    {
                        request.MemberId = Tokens["id"].ToString();
                        request.TokenGuid = Tokens["uuid"].ToString();
                    }
                }

                IMemberRead read = new MemberRead();
                result.Datas = _member.GetUserFollow(read, request, out _ErrCode, out _ErrMsg);
                result.Msg = _ErrMsg;
                result.Code = _ErrCode;
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
        public IActionResult AddMember(MemberModel.MyFollowSetRequest request)
        {
            var result = new ResultContext();
            var token = new ResultPostContext();
            string accessToken = string.Empty;
            string fileDate = DateTime.Now.ToString("yyyyMMddHHmmss");
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

                if (_ErrCode == 0)
                {
                    request.MemberId = Tokens["id"].ToString();
                    request.TokenGuid = Tokens["uuid"].ToString();


                    IMemberCreate create = new MemberCreate();
                    _member.SetFollow(create, request, out _ErrCode, out _ErrMsg);


                    result.Code = _ErrCode;
                    result.Msg = _ErrMsg;
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
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message.ToString());
                result.Msg = "시스템 오류입니다. 잠시후 다시 시도해주세요.";
                result.Code = -500;
                return StatusCode(500, result);
            }

            token.AccessToken = accessToken;
            result.Datas = token;
            return Ok(new { result });
        }

        [Route("[action]")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[IsLogin]
        public IActionResult Disconnect([FromQuery] MemberModel.MyFollowDisconnectRequest request)
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

                request.MemberId = Tokens["id"].ToString();
                request.TokenGuid = Tokens["uuid"].ToString();

                IMemberDelete delete = new MemberDelete();
                _member.SetFollowDisconnect(delete, request, out _ErrCode, out _ErrMsg);

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

            token.AccessToken = accessToken;
            result.Datas = token;
            return Ok(new { result });
        }

        [Route("[action]")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[IsLogin]
        public new IActionResult User([FromQuery] MemberModel.UserListRequest request)
        {
            var result = new ResultContext();
            string accessToken;

            try
            {
                string authorization = Request.Headers["Authorization"].ToString().Substring("Bearer ".Length).Trim();
                string authCookie = string.Empty;

                if (Request.Cookies["x-ref-token"] != null)
                {
                    authCookie = Request.Cookies["x-ref-token"].ToString();
                }

                if (!string.IsNullOrEmpty(authorization) && !string.IsNullOrWhiteSpace(authorization) && !string.IsNullOrEmpty(authCookie) && !string.IsNullOrWhiteSpace(authCookie))
                {
                    var Tokens = _auth.GetJwtPayload(authorization, out _ErrCode, out _ErrMsg, out accessToken);

                    if (Tokens != null || Tokens.Count() > 0 || _ErrCode == 0 || !string.IsNullOrEmpty(Tokens["id"].ToString()) || !string.IsNullOrEmpty(Tokens["uuid"].ToString()))
                    {
                        request.MemberId = Tokens["id"].ToString();
                        request.TokenGuid = Tokens["uuid"].ToString();
                    }
                }

                IMemberRead read = new MemberRead();
                result.Datas = _member.GetUserList(read, request, out _ErrCode, out _ErrMsg);
                result.Msg = _ErrMsg;
                result.Code = _ErrCode;
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
    }
}
