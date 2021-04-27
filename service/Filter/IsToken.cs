using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace kombirest.Filter
{
    public class IsToken : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string auth = context.HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(auth) || auth == "undefined")
            {
                int Code = -4444;
                string Msg = $"토큰 서명을 찾을수 없습니다.";
                var result = new JsonResult(new { Code, Msg });
                context.Result = new ObjectResult(result)
                {
                    StatusCode = -401,
                    Value = result
                };
            }
        }

        public class ReturnUnauthorized : ActionResult
        {
            public override void ExecuteResult(ActionContext context)
            {
                context.HttpContext.Response.StatusCode = 5000;
                base.ExecuteResult(context);
            }
        }
    }
}
