﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;
using System.Net;
using Quick.OwinMVC.Utils;
using Quick.OwinMVC.Routing;
using Quick.OwinMVC.Middleware;

namespace Quick.OwinMVC.Controller.Impl
{
    [Route("/:" + MvcMiddleware.QOMVC_PLUGIN_KEY + "/resource/:" + MvcMiddleware.QOMVC_PATH_KEY)]
    internal class ResourceHttpController : HttpController
    {
        public override void DoGet(IOwinContext context, string plugin, string path)
        {
            var rep = context.Response;

            Uri uri = new Uri($"resource://{plugin}/resource/{path}");
            WebResponse resourceResponse = null;
            try
            {
                resourceResponse = WebRequest.Create(uri).GetResponse();
            }
            catch { }

            if (resourceResponse == null)
            {
                rep.StatusCode = 404;
                rep.Write($"Resource '{path}' in plugin '{plugin}' not found!");
                return;
            }
            var stream = resourceResponse.GetResponseStream();
            var mime = MimeUtils.GetMime(path);
            if (mime != null)
                rep.ContentType = mime;
            rep.ContentLength = stream.Length;
            stream.CopyTo(rep.Body);
        }
    }
}
