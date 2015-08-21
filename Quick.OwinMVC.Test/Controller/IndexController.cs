﻿using Quick.OwinMVC.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Quick.OwinMVC.Routing;

namespace Quick.OwinMVC.Test.Controller
{
    [Route("/")]
    [Route("index")]
    public class IndexController : IMvcController
    {
        public string Service(IOwinContext context, dynamic data)
        {
            return "index";
        }
    }
}
