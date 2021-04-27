using System;
using System.Linq;
using kombirest.service.Interface.Commons;
using kombirest.service.Interface.Board;
using kombirest.service.Service.Board;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.kombirest.model.Common;
using Model.kombirest.model.Product;
using Microsoft.Extensions.Logging;

namespace kombirest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardController : ControllerBase
    {
        private int _ErrCode = 0;
        private String _ErrMsg = string.Empty;

        private readonly IAuth _auth;
        private readonly ILogger<BoardController> _logger;
        private readonly IBoardDto _board;
        public BoardController(IAuth auth, IBoardDto board, ILogger<BoardController> logger)
        {
            _auth = auth;
            _board = board;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[IsToken]
        public IActionResult Post([FromForm] ProductModel.PrdBoardRequest request)
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

                var context = new ProductModel.PrdBoardContext
                {
                    MemberId = Tokens["id"].ToString(),
                    PrdIdx = request.PrdIdx,
                    ReplyNick = request.NickName,
                    Contents = request.Contents,
                    Depth = 0,
                    Guid = Tokens["uuid"].ToString()
                };

                IBoardCreate create = new BoardCreate();
                _board.SetBoard(create, context, out _ErrCode, out _ErrMsg);

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
            catch(Exception e)
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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Get([FromQuery] ProductModel.PrdBoardScrollRequest request)
        {
            var result = new ResultContext();
            try
            {
                IBoardRead read = new BoardRead();
                var data = _board.GetBoard(read, request, out _ErrCode, out _ErrMsg);
                result.Code = _ErrCode;
                result.Msg = _ErrMsg;
                result.Datas = data;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message.ToString());
                result.Msg = "시스템 오류입니다. 잠시후 다시 시도해주세요.";
                result.Code = -500;
                return StatusCode(500, result);
            }
            return Ok(new { result });
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Put([FromBody] ProductModel.PrdBoardUpdateRequest request)
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

                if (request.BoardIdx > 0)
                {
                    request.MemberId = Tokens["id"].ToString();
                    request.TokenGuid = Tokens["uuid"].ToString();

                    IBoardUpdate update = new BoardUpdate();
                    _board.SetBoardUpdate(update, request, out _ErrCode, out _ErrMsg);

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

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Delete([FromQuery] ProductModel.PrdBoardDeleteRequest request)
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

                result.Code = -1;
                result.Msg = "시스템 오류입니다.";

                if (request.BoardIdx > 0 && request.PrdIdx > 0)
                {
                    request.MemberId = Tokens["id"].ToString();
                    request.TokenGuid = Tokens["uuid"].ToString();

                    IBoardDelete delete = new BoardDelete();
                    _board.SetBoardDelete(delete, request, out _ErrCode, out _ErrMsg);

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
    }
}
