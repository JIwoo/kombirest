using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace kombirest.Filter
{
    public class IsLogin : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {            
            if (context.HttpContext.Session.GetString("_isLogin") == null)
            {
                int Errcode = -8888;
                string Url = "Login";
                string Errmsg = $"넌이미실패해있다";
                //var Controller = context.ActionDescriptor.RouteValues["controller"];
                //var Action = context.ActionDescriptor.RouteValues["action"];                

                var result = new JsonResult(new { Errcode, Url, Errmsg });
                context.Result = new ObjectResult(result)
                {
                    //StatusCode = (int)HttpStatusCode.InternalServerError,
                    StatusCode = 200,
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
