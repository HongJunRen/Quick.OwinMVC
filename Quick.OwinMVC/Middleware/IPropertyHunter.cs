﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quick.OwinMVC.Middleware
{
    public interface IPropertyHunter
    {
        void Hunt(String key, String value);
    }
}
