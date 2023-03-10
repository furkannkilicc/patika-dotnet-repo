using System;
using System.Diagnostics;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace  HelloWebapi.Middlewares{


    public class CustomExceptionMiddleware{
            private readonly RequestDelegate _next;
            public CustomExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async  Task Invoke (HttpContext context)
        {
                 var watch= Stopwatch.StartNew();
            try
            {
            string message = "[Request ] HTTP "+ context.Request.Method + "- =>  "+ context.Request.Path;
            Console.WriteLine(message);
            await _next(context);   //Bir sonraki mw çağırıyoruz  
            watch.Stop();
               message = "[Request ] HTTP "+ context.Request.Method + "- =>  "+ context.Request.Path + "in" + watch.Elapsed.TotalMilliseconds + "ms";
            Console.WriteLine(message);
            }
            catch (Exception ex)
            {
                    watch.Stop();
                    await HandleException(context,ex.watch );

                }
           

        }

        private Task HandleException(HttpContext context, Exception ex, Stopwatch watch)
        {
                string message = "[Error] HTTP " +context.Request.Method+"-"+ context.Response.StatusCode+"Error Message"+ex.Message+"in" + watch.Elapsed.TotalMilliseconds+ "ms" ;
                Console.WriteLine(message);
                context.Response.ContentType="application/json";
                context.Response.StatusCode= (int)HttpStatusCode.InternalServerError;
                var result = JsonConverter.SerializeObject(new {error = ex.Message}, Formatting.None);
                return context.Response.WriteAsync(result);
        }
    }
        public static class CustomExceptionMiddlewareExtension {
            public static IApplicationBuilder  UseCustomExceptionMiddle(this IApplicationBuilder builder)
            {
                return builder.UseMiddleware<CustomExceptionMiddleware>();
            } 
        }

 }
