using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.kombirest.model.Activity;
//using kombirest.service.Activity;
using kombirest.service;
using Model.kombirest.model.Common;
using Model.kombirest.model.Product;
using kombirest.service.Product;
using kombirest.service.Interface.Commons;
//using kombirest.service.Member;
using Model.kombirest.model.Member;
using Microsoft.Extensions.Logging;
using kombirest.service.Interface.Member;
using kombirest.service.Service.Member;

namespace kombirest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyInfoController : ControllerBase
    {
        private int _ErrCode = 0;
        private String _ErrMsg = string.Empty;
        private readonly IAuth _auth;
        private readonly IMemberDto _member;
        private readonly ILogger<MyInfoController> _logger;
        public MyInfoController(IAuth auth, IMemberDto member,ILogger<MyInfoController> logger)
        {
            _auth = auth;
            _member = member;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
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

                var context = new MemberModel.MemberInfoContext
                {
                    MemberId = Tokens["id"].ToString(),
                    TokenGuid = Tokens["uuid"].ToString()
                };

                IMemberRead read = new MemberRead();
                result.Datas = _member.GetMemberInfo(read, context, out _ErrCode, out _ErrMsg);
                result.Msg = _ErrMsg;
                result.Code = _ErrCode;
                if(_ErrCode == -4008)
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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Post([FromForm] MemberModel.MemberInfosRequest request)
        {
            var result = new ResultContext();
            var putResilt = new MemberModel.MemberPutResult();
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

                if (_ErrCode == 0)
                {
                    request.MemberId = Tokens["id"].ToString();
                    request.TokenGuid = Tokens["uuid"].ToString();

                    IMemberUpdate update = new MemberUpdate();
                    _member.SetMemberInfo(request, out _ErrCode, out _ErrMsg);

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
                    if (_ErrCode == 0)
                    {
                        var context = new MemberModel.MemberInfoContext
                        {
                            MemberId = Tokens["id"].ToString(),
                            TokenGuid = Tokens["uuid"].ToString()
                        };

                        IMemberRead read = new MemberRead();
                        var data = _member.GetMemberInfo(read, context, out _ErrCode, out _ErrMsg);
                        putResilt.AccessToken = accessToken;
                        putResilt.Nick = data.NickNm;
                        putResilt.Profile = data.Profile;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message.ToString());
                result.Msg = "시스템 오류입니다. 잠시후 다시 시도해주세요.";
                result.Code = -500;
                return StatusCode(500, new { result });
            }
            finally{
                if(result.Code != 0)
                {
                    //TODO
                    //업로드된 이미지 삭제
                }
            }

            result.Datas = putResilt;
            return Ok(new { result });
        }
    }
}
