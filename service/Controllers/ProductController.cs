using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.kombirest.model.Product;
//using kombirest.service.Product;
using kombirest.Filter;
using Model.kombirest.model.Common;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.OpenApi.Writers;
using kombirest.service.Interface.Commons;
using System.Web.Http;
using Model.kombirest.model.Activity;
using Microsoft.Extensions.Logging;
using kombirest.service.Interface.Product;
using kombirest.service.Service.Product;
//using System.Runtime.Caching;

namespace kombirest.Controllers
{
    

    [Route("api/[controller]")]    
    [ApiController]
    public class ProductController : ControllerBase
    {
        private int _ErrCode = 0;
        private String _ErrMsg = string.Empty;

        private readonly IMemoryCache _cache;
        private readonly IAuth _auth;
        private readonly ILogger<ProductController> _logger;
        private readonly IProductDto _product;

        public ProductController(IProductDto product, IMemoryCache memoryCache, IAuth auth, ILogger<ProductController> logger)
        {
            _cache = memoryCache;
            _product = product;
            _auth = auth;       
            _logger = logger;
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[RequestSizeLimit()]
        //[IsLogin]
        //[IsToken]
        public IActionResult Post([FromForm] ProductModel.PrdAddRequest request)
        {
            var result = new ResultContext();
            var token = new ResultPostContext();
            string accessToken = string.Empty;
            string fileDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            string filePath = DateTime.Now.ToString("yyyyMMdd");

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

                if (request.files != null && request.files.Count() > 3)
                {
                    result.Msg = "사진은 3장까지 업로드 가능합니다.";
                    result.Code = -1000;
                    return StatusCode(403, result);
                }
                if (request.items != null && request.items.Count() > 20)
                {
                    result.Msg = "아이템은 20개까지만 선택 가능합니다.";
                    result.Code = -1001;
                    return StatusCode(403, result);
                }

                if (_ErrCode == 0)
                {
                    request.memberId = Tokens["id"].ToString();
                    request.Guid = Tokens["uuid"].ToString();         
                    
                    _product.SetProduct(request, fileDate, filePath, out _ErrCode, out _ErrMsg);

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
            finally{
                if(_ErrCode != 0 && _ErrCode > -4000)
                {
                    //_delete.SetDelete(request.files, fileDate);
                }
            }

            token.AccessToken = accessToken;
            result.Datas = token;
            return Ok(new { result });
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[RequestSizeLimit()]
        //[IsLogin]
        //[IsToken]
        public IActionResult Report([FromForm] ProductModel.PrdReportRequest request)
        {
            var result = new ResultContext();
            var token = new ResultPostContext();

            string accessToken = string.Empty;
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

                    IProductUpdate update = new ProductUpdate();
                    update.SetProductReport(request, out _ErrCode, out _ErrMsg);

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

        [HttpGet]
        public IActionResult Get([FromQuery] ProductModel.PrdListContext request)
        {
            var result = new ResultContext();
            string accessToken;
            try
            {
                IProductRead read = new ProductRead();
                if (!request.Type.Equals("search"))
                {
                    //DateTime cacheEntry;
                    //if (!_cache.TryGetValue(result, out cacheEntry))
                    //{
                    //    var resultContext = _product.GetProductList(read, request, out _ErrCode, out _ErrMsg);
                    //    result.Code = _ErrCode;
                    //    result.Msg = _ErrMsg;
                    //    result.Datas = resultContext;

                    //    cacheEntry = DateTime.Now;
                    //    var cacheEntryOptions = new MemoryCacheEntryOptions()
                    //        .SetSlidingExpiration(TimeSpan.FromMinutes(15));
                    //    _cache.Set(result, cacheEntry, cacheEntryOptions);
                    //}
                    var resultContext = _product.GetProductList(read, request, out _ErrCode, out _ErrMsg);
                    result.Code = _ErrCode;
                    result.Msg = _ErrMsg;
                    result.Datas = resultContext;
                }
                else
                {
                    if (request.Type.Equals("search"))
                    {
                        string authorization = Request.Headers["Authorization"].ToString() != null ? Request.Headers["Authorization"].ToString().Substring("Bearer ".Length).Trim()
                            : null;

                        if (!string.IsNullOrEmpty(authorization))
                        {
                            var Tokens = _auth.GetJwtPayload(authorization, out _ErrCode, out _ErrMsg, out accessToken);

                            if (Tokens != null && Tokens.Count() > 0 && _ErrCode == 0
                                && !string.IsNullOrEmpty(Tokens["id"].ToString()))
                            {
                                request.MemberId = Tokens["id"].ToString();
                            }
                        }
                    }
                    var resultContext = _product.GetProductList(read,request, out _ErrCode, out _ErrMsg);
                    result.Code = _ErrCode;
                    result.Msg = _ErrMsg;
                    result.Datas = resultContext;
                }
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Detail([FromQuery] ProductModel.PrdDetailSingleRequest request)
        {
            var result = new ResultContext();
            string accessToken;
            try
            {
                if (request.Key > 0)
                {
                    //string authorization = Request.Headers["Authorization"].ToString() != null ? Request.Headers["Authorization"].ToString().Substring("Bearer ".Length).Trim()
                    //    : null;
                    //if (!string.IsNullOrEmpty(authorization) || authorization != "undefined")
                    //{
                    //    var Tokens = _auth.GetJwtPayload(authorization, out _ErrCode, out _ErrMsg, out accessToken);

                    //    if (Tokens != null && Tokens.Count() > 0 && _ErrCode == 0
                    //        && !string.IsNullOrEmpty(Tokens["id"].ToString())
                    //        && !string.IsNullOrEmpty(Tokens["uuid"].ToString()))
                    //    {
                    //        request.MemberId = Tokens["id"].ToString();
                    //        request.TokenGuid = Tokens["uuid"].ToString();
                    //    }
                    //}

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

                    IProductRead read = new ProductRead();
                    var resultContext = _product.GetProductDetail(read, request, out _ErrCode, out _ErrMsg);
                    result.Code = _ErrCode;
                    result.Msg = _ErrMsg;
                    result.Datas = resultContext;
                }
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult PutDetail([FromQuery] ProductModel.PrdDetailSingleRequest request)
        {
            var result = new ResultContext();
            string accessToken;
            try
            {
                if (request.Key > 0)
                {
                    //string authorization = Request.Headers["Authorization"].ToString() != null ? Request.Headers["Authorization"].ToString().Substring("Bearer ".Length).Trim()
                    //    : null;
                    //if (!string.IsNullOrEmpty(authorization) || authorization != "undefined")
                    //{
                    //    var Tokens = _auth.GetJwtPayload(authorization, out _ErrCode, out _ErrMsg, out accessToken);

                    //    if (Tokens != null && Tokens.Count() > 0 && _ErrCode == 0
                    //        && !string.IsNullOrEmpty(Tokens["id"].ToString())
                    //        && !string.IsNullOrEmpty(Tokens["uuid"].ToString()))
                    //    {
                    //        request.MemberId = Tokens["id"].ToString();
                    //        request.TokenGuid = Tokens["uuid"].ToString();
                    //    }
                    //}

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

                    IProductRead read = new ProductRead();
                    var resultContext = _product.GetProductDetail(read,request, out _ErrCode, out _ErrMsg);
                    result.Code = _ErrCode;
                    result.Msg = _ErrMsg;
                    result.Datas = resultContext;
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

        [Route("[action]")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdatePostDetail([FromQuery] ProductModel.PrdDetailSingleRequest request)
        {
            var result = new ResultContext();
            string accessToken = string.Empty;
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

                if (request.Key > 0)
                {
                    IProductRead read = new ProductRead();
                    var resultContext = _product.GetProductDetail(read, request, out _ErrCode, out _ErrMsg);
                    result.Code = _ErrCode;
                    result.Msg = _ErrMsg;
                    result.Datas = resultContext;
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
                return StatusCode(500, new { result });
            }
            return Ok(new { result });
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Put([FromForm] ProductModel.PrdUpdateRequest request)
        {
            var result = new ResultContext();
            var token = new ResultPostContext();


            string accessToken = string.Empty;
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

                if (request.Files != null && request.Files.Count() > 3)
                {
                    result.Msg = "사진은 3장까지 업로드 가능합니다.";
                    result.Code = -1000;
                    return StatusCode(403, result);
                }
                if (request.Items.Count() > 20)
                {
                    result.Msg = "아이템은 20개까지만 선택 가능합니다.";
                    result.Code = -1001;
                    return StatusCode(403, result);
                }
                if (request.Deletes != null && request.Deletes.Count() > 3)
                {
                    result.Msg = "시스템오류입니다.";
                    result.Code = -1002;
                    return StatusCode(403, result);
                }

                if (request.Key > 0)
                {
                    request.memberId = Tokens["id"].ToString();
                    request.Guid = Tokens["uuid"].ToString();

                    _product.SetPrdUpdate(request, out _ErrCode, out _ErrMsg);

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
                return StatusCode(500, new { result });
            }

            token.AccessToken = accessToken;
            result.Datas = token;
            return Ok(new { result });
        }

        [Route("[action]")]
        [HttpPut]
        public IActionResult Choice([FromBody] ProductModel.PrdChoiceRequest request)
        {
            var result = new ResultContext();
            var token = new ResultPostContext();

            string accessToken = string.Empty;
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

                    IProductUpdate update = new ProductUpdate();
                    _product.SetProductChoice(update, request, out _ErrCode, out _ErrMsg);

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
        public IActionResult Delete([FromQuery] ProductModel.PrdDetailRequest request)
        {
            var result = new ResultContext();
            var token = new ResultPostContext();
            string accessToken = string.Empty;
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

                if (request.Key > 0)
                {
                    request.MemberId = Tokens["id"].ToString();
                    request.TokenGuid = Tokens["uuid"].ToString();


                    IProductRead read = new ProductRead();
                    var optionsContext = new ProductModel.PrdDetailRequest
                    {
                        Key = request.Key
                    };
                    var s3Result = read.GetDeleteImage(optionsContext, out _ErrCode, out _ErrMsg);

                    IProductDelete delete = new ProductDelete();
                    _product.SetPrdDelete(delete, request, s3Result, out _ErrCode, out _ErrMsg);

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
                return StatusCode(500, new { result });
            }

            token.AccessToken = accessToken;
            result.Datas = token;
            return Ok(new { result });
        }
    }
}
