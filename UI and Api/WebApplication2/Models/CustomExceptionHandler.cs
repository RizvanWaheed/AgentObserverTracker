using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace AgentObserverWeb.Models
{
    //public class GlobalExceptionMiddleware : OwinMiddleware
    //{
    //    public GlobalExceptionMiddleware(OwinMiddleware next) : base(next)
    //    { }

    //    public override async Task Invoke(IOwinContext context)
    //    {
    //        try
    //        {
    //            await Next.Invoke(context);
    //        }
    //        catch (Exception ex)
    //        {
    //            // your handling logic
    //        }
    //    }
    //}
    public class CustomExceptionHandler : IExceptionHandler
    {
        public Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            // Perform some form of logging

            context.Result = new ResponseMessageResult(new HttpResponseMessage
            {
                Content = new StringContent("An unexpected error occurred"),
                StatusCode = HttpStatusCode.InternalServerError
            });

            return Task.FromResult(0);
        }
    }
}