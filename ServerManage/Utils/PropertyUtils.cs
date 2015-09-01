﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServerManage.Utils
{
    public class PropertyUtils
    {
        public static IDictionary<String, String> Load(String content)
        {
            IDictionary<String, String> dict = new Dictionary<String, String>();
            Regex regex = new Regex(@"\s*(?'key'[^#][^\s]*)\s*=\s*(?'value'.*)\s*");
            foreach (Match match in regex.Matches(content))
            {
                String key = match.Groups["key"].Value;
                String value = match.Groups["value"].Value.Trim();

                dict[key] = value;
            }
            return dict;
        }

        public static IDictionary<String, String> LoadFile(String fileName)
        {
            return Load(File.ReadAllText(fileName));
        }
    }
}
