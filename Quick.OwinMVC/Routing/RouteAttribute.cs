﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Routing
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true)]
    public class RouteAttribute : Attribute
    {
        public String Path { get; private set; }

        public RouteAttribute(String path)
        {
            this.Path = path;
        }
    }
}
