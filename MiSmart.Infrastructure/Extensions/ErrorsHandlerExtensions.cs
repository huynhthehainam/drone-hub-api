using MiSmart.Infrastructure.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MiSmart.Infrastructure.Services;
using Microsoft.AspNetCore.Diagnostics;
using System;
using System.Text.Json;
using System.Collections.Generic;

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
                    response.AddNotAllowedErr(false);
                }
                else if (context.HttpContext.Response.StatusCode == 401)
                {
                    response.AddAuthorizationErr(false);
                }
                else if (context.HttpContext.Response.StatusCode == 404)
                {
                    response.AddNotFoundErr("Url", false);

                }
                await context.HttpContext.Response.WriteAsJsonAsync(response);
            });
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionHandlerPathFeature =
                        context.Features.Get<IExceptionHandlerPathFeature>();
                    // EmailService emailService = context.RequestServices.GetService<EmailService>();
                    // await emailService.SendMailAsync(new String[] { "huynhthehainam@gmail.com" }, new String[] { }, new String[] { }, $"Bugggggg {appName}", $"{exceptionHandlerPathFeature?.Error.ToString()}");
                    var message = exceptionHandlerPathFeature?.Error.Message;

                    try
                    {
                        ErrorException exception = JsonSerializer.Deserialize<ErrorException>(message);
                        context.Response.StatusCode = exception.StatusCode;
                        await context.Response.WriteAsJsonAsync(exception);
                    }
                    catch (Exception)
                    {
                        await context.Response.WriteAsJsonAsync(new ErrorException
                        {
                            StatusCode = context.Response.StatusCode,
                            Errors = new
                            {
                                Unknown = new List<String>() { message }
                            }
                        });
                    }
                });
            });
        }
    }
}