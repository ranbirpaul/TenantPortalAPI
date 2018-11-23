using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace TenantWebAPI
{
	public class ApiExceptionFilter : ExceptionFilterAttribute
	{
		public override void OnException(ExceptionContext context)
		{
			// Handle API errors
			var ex = context.Exception;
			Log.Logger.Error(context.Exception, "An unhandled error occurred. Error has been logged.");
			context.Exception = null;
			context.Result = new JsonResult(ex.Message);
			context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
			context.ExceptionHandled = true;
			base.OnException(context);
		}
	}

	public class ErrorHandlingMiddleware
	{
		private readonly RequestDelegate next;

		public ErrorHandlingMiddleware(RequestDelegate next)
		{
			this.next = next;
		}

		public async Task Invoke(HttpContext context /* other dependencies */)
		{
			try
			{
				await next(context);
			}
			catch (Exception ex)
			{
				await HandleExceptionAsync(context, ex);
			}
		}

		private static Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			var code = HttpStatusCode.InternalServerError;
			if (exception is NullReferenceException)
				code = HttpStatusCode.NotFound;
			else if (exception is UnauthorizedAccessException)
				code = HttpStatusCode.Unauthorized;
			else if (exception is Exception)
				code = HttpStatusCode.BadRequest;

			Log.Logger.Error(exception, "An unhandled error occurred. Error has been logged.");
			var result = JsonConvert.SerializeObject(new { error = exception.Message });
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)code;
			return context.Response.WriteAsync(result);
		}
	}
}