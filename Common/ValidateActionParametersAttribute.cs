using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace PracticeWeb.Common
{
    public class ValidateActionParametersAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            //no code as of now.
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                StringBuilder errorMessagesBuilder = new StringBuilder();
                try
                {
                    foreach (var modelstate in context.ModelState.Values)
                    {
                        foreach (var error in modelstate.Errors)
                        {
                            errorMessagesBuilder.AppendLine(error.ErrorMessage);
                        }
                    }
                }
                catch
                {
                    //no need to catch the exception
                    //because any exception would still be in INVALID model state only
                }
                finally
                {
                    var errorResponse = new ApiErrorResponse
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = errorMessagesBuilder.ToString()
                    };

                    base.OnActionExecuting(context);

                    //set the result so that execution gets short-circuited and error response is returned.
                    context.HttpContext.Response.ContentType = "application/json";
                    context.Result = new BadRequestObjectResult(errorResponse);
                }



            }
        }

    }
}
