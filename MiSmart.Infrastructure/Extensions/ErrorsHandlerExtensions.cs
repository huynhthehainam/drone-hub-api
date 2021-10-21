using MiSmart.Infrastructure.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MiSmart.Infrastructure.Services;
using Microsoft.AspNetCore.Diagnostics;
using System;

namespace MiSmart.Infrastructure.Extensions
{
    public static class ErrorsHandlerExtensions
    {
        public static void CustomizeErrorHandler(this IApplicationBuilder app, String appName)
        {
            app.UseStatusCodePages(async context =>
            {
                IActionResponseFactory actionResponseFactory = app.ApplicationServices.GetRequiredService<IActionResponseFactory>();
                var response = actionResponseFactory.CreateInstance();
                if (context.HttpContext.Response.StatusCode == 403)
                {
                    response.AddNotAllowedErr();
                }
                else if (context.HttpContext.Response.StatusCode == 401)
                {
                    response.AddMessageErr("Authorization", "Invalid");
                }
                await context.HttpContext.Response.WriteAsJsonAsync(response);
            });
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    var exceptionHandlerPathFeature =
                        context.Features.Get<IExceptionHandlerPathFeature>();
                    EmailService emailService = context.RequestServices.GetService<EmailService>();
                    await emailService.SendMailAsync(new String[] { "huynhthehainam@gmail.com" }, new String[] { }, new String[] { }, $"Bugggggg {appName}", $"{exceptionHandlerPathFeature?.Error.ToString()}");
                });
            });
        }
    }
}