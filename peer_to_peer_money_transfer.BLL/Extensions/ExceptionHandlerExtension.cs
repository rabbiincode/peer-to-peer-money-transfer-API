using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using peer_to_peer_money_transfer.BLL.Models;

namespace peer_to_peer_money_transfer.BLL.Extensions
{
    public static class ExceptionHandlerExtension
    {
        static ExceptionHandler handler = new ExceptionHandler();
        public static void ConfigureExceptionHandler(this IApplicationBuilder application)
        {
            application.UseExceptionHandler(error =>
            {
                error.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        handler.LogErrorMessage($"Something went wrong in the: {contextFeature.Error}");
                        await context.Response.WriteAsync(new ErrorHandlerModel()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "Internal Server Error...please try again"
                        }.ToString());
                    }
                });

            });
        }
    }

    public class ExceptionHandler
    {
        private readonly ILogger _logger;
        public ExceptionHandler(){}
        public ExceptionHandler(ILogger<ExceptionHandler> logger)
        {
            _logger = logger;
        }

        public void LogErrorMessage(string errorMessage)
        {
            _logger.LogError(errorMessage);
        }
    }
}
