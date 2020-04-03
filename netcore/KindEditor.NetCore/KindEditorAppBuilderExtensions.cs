using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KindEditor.AppBuilder
{
    public static partial class KindEditorAppBuilderExtensions
    {
        public static KindEditorOptions Options { get; private set; }

        internal static readonly Dictionary<string, Func<HttpContext, Task>> PageList = new Dictionary<string, Func<HttpContext, Task>> {
            { "/apilist", apiList },
            { "/upload", uploadJson },
            { "/filemanager", fileManagerJson },
        };

        public static IApplicationBuilder UseKindEditor(this IApplicationBuilder app)
        {
            return UseKindEditor(app, null);
        }
        public static IApplicationBuilder UseKindEditor(this IApplicationBuilder app, KindEditorOptions options)
        {
            Options = options ?? new KindEditorOptions();
            return app.Use(KindEditorHandler);
        }

        private static Task KindEditorHandler(HttpContext context, Func<Task> next)
        {
            var path = context.Request.Path.Value.ToLower();
            var kePath = path.Replace(Options.ApiPrefix, "");

            if (path.StartsWith(Options.ApiPrefix))
            {
                if (string.IsNullOrEmpty(kePath))
                {
                    return PageList["/"](context);
                }
                else if (kePath.StartsWith('/'))
                {
                    return PageList[kePath](context);
                }
                else
                {
                    return Task.Run(next);
                }
            }
            else
            {
                return Task.Run(next);
            }
        }

        private static Task apiList(HttpContext context)
        {
            return JsonResponse(context, Options.ApiPath);
        }

        private static Task JsonResponse(HttpContext context, object data)
        {
            context.Response.Headers["Content-Type"] = "application/json";
            context.Response.WriteAsync(JsonConvert.SerializeObject(data)).Wait();
            return Task.CompletedTask;
        }

        private static Task JsonErrorResponse(HttpContext context, string message)
        {
            return JsonResponse(context, new
            {
                error = 1,
                message,
            });
        }
    }
}