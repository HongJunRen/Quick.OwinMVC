﻿using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Middleware
{
    public class Error404Middleware : OwinMiddleware, IPropertyHunter
    {
        private static readonly String INVOKE_TIMES = $"{typeof(Error404Middleware).FullName}.{nameof(INVOKE_TIMES)}";
        private Server server;
        private String RewritePath;

        public Error404Middleware(OwinMiddleware next) : base(next)
        {
            this.server = Server.Instance;
        }

        void IPropertyHunter.Hunt(string key, string value)
        {
            if (key == nameof(RewritePath))
                RewritePath = value;
        }

        public override Task Invoke(IOwinContext context)
        {
            var rep = context.Response;
            rep.StatusCode = 404;

            //判断调用次数，避免无限递归
            var invokeTimes = context.Get<Int32>(INVOKE_TIMES);
            invokeTimes++;
            if (invokeTimes > 1 || String.IsNullOrEmpty(RewritePath))
            {
                String msg = "404 Not Found";
                rep.ContentLength = msg.Length;
                return rep.WriteAsync(msg);
            }
            context.Set(INVOKE_TIMES, invokeTimes);

            context.Set<String>("owin.RequestPath", RewritePath);
            OwinMiddleware first = server.GetFirstMiddlewareInstance();
            if (first == null)
                throw new ArgumentException($"Middleware '{this.GetType().FullName};{this.GetType().Assembly.GetName().Name}' must not be the first middleware,and recommand to be set to the last one.");

            //清理OwinContext
            foreach (var cleaner in server.GetMiddlewares<IOwinContextCleaner>())
                cleaner.Clean(context);

            return first.Invoke(context);
        }
    }
}
