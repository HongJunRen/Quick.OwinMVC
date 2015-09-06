﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Quick.OwinMVC.Controller;
using Quick.OwinMVC.View;
using Quick.OwinMVC.Utils;

namespace Quick.OwinMVC.Middleware
{
    public class ViewMiddleware : AbstractControllerMiddleware<IViewController>
    {
        private IViewRender viewRender;

        public ViewMiddleware(OwinMiddleware next) : base(next)
        {
            var properties = Server.Instance.properties;
            //创建视图渲染器
            String viewRenderClassName = properties[VIEWRENDER_CLASS];
            this.viewRender = (IViewRender)AssemblyUtils.CreateObject(viewRenderClassName);
            this.viewRender.Init(properties);
        }

        public override string GetRouteMiddle()
        {
            return "view";
        }

        public override void ExecuteController(IViewController controller, IOwinContext context, string plugin, string path)
        {
            //执行MVC控制器，返回视图名称
            String viewName = controller.Service(context, context.Environment);
            if (viewName == null)
                return;
            viewName = $"{plugin}:{viewName}";
            //根据视图名称与数据，渲染输出页面
            var content = viewRender.Render(viewName, context.Environment);
            var bytes = encoding.GetBytes(content);
            //将页面写到响应中
            var rep = context.Response;
            rep.ContentType = "text/html; charset=UTF-8";
            rep.ContentLength = bytes.Length;
            rep.Write(content);
        }
    }
}
