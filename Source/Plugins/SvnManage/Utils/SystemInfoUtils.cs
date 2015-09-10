﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SvnManage.Utils
{
    public class SystemInfoUtils
    {
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        public class ShellCmdAttribute : Attribute
        {
            public PlatformID PlatformID { get; set; }
            /// <summary>
            /// 程序名称
            /// </summary>
            public String Program { get; set; }
            /// <summary>
            /// 参数
            /// </summary>
            public String Arguments { get; set; }
            public Regex Regex { get; set; }
            public ShellCmdAttribute(PlatformID platformID, String program, String arguments, String regex)
            {
                this.PlatformID = platformID;
                this.Program = program;
                this.Arguments = arguments;
                this.Regex = new Regex(regex);
            }
        }

        private static String executeShell()
        {
            PlatformID currentPlatform = Environment.OSVersion.Platform;
            var stackFrame = new StackTrace(1).GetFrame(0);
            var method = stackFrame.GetMethod();
            var shellCmdAttr = method.GetCustomAttributes(typeof(ShellCmdAttribute), false)
                .Cast<ShellCmdAttribute>()
                .Where(t => t.PlatformID == currentPlatform).FirstOrDefault();
            if (shellCmdAttr == null)
                return null;

            String programName = shellCmdAttr.Program;
            String arguments = shellCmdAttr.Arguments;

            var process = new Process();
            var psi = process.StartInfo;
            psi.FileName = programName;
            psi.Arguments = arguments;

            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;

            process.Start();
            var content = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            var match = shellCmdAttr.Regex.Match(content);
            if (match == null || !match.Success)
                return null;
            var value = match.Groups["value"].Value;
            return value;
        }


        /// <summary>
        /// 获取计算机名称
        /// </summary>
        /// <returns></returns>
        [ShellCmd(PlatformID.Win32NT, "cmd", "/c wmic computersystem get Caption", @"^Caption\s*(?'value'.*?)\s*$")]
        [ShellCmd(PlatformID.Unix, "uname", "-n", @"^\s*(?'value'.*?)\s*$")]
        public static String GetComputerName()
        {
            return executeShell();
        }

        /// <summary>
        /// 获取操作系统名称
        /// </summary>
        /// <returns></returns>
        [ShellCmd(PlatformID.Win32NT, "cmd", "/c wmic OS get Caption", @"^Caption\s*(?'value'.*?)\s*$")]
        [ShellCmd(PlatformID.Unix, "uname", "-s -r -v -m -o", @"^\s*(?'value'.*?)\s*$")]
        public static String GetOsName()
        {
            return executeShell();
        }

        /// <summary>
        /// 获取CPU使用率
        /// </summary>
        /// <returns></returns>
        [ShellCmd(PlatformID.Win32NT, "cmd", "/c wmic cpu get LoadPercentage", @"^LoadPercentage\s*(?'value'.*?)\s*$")]
        [ShellCmd(PlatformID.Unix, "grep", "'cpu ' /proc/stat", @"^\s*(?'value'.*?)\s*$")]
        public static int GetCpuUsage()
        {
            var value = executeShell();
            if (String.IsNullOrEmpty(value))
                return 0;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                return Int32.Parse(value);
            //| awk '{usage=($2+$4)*100/($2+$4+$5)} END {print usage}'
            var numbers = value.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Skip(1).Take(4).Select(t => Int32.Parse(t)).ToArray();
            return (numbers[0] + numbers[2]) * 100 / (numbers[0] + numbers[2] + numbers[3]);
        }

        /// <summary>
        /// 获取总内存数
        /// </summary>
        /// <returns></returns>
        [ShellCmd(PlatformID.Win32NT, "cmd", "/c wmic OS get TotalVisibleMemorySize", @"^TotalVisibleMemorySize\s*(?'value'.*?)\s*$")]
        public static long GetTotalMemory()
        {
            var value = executeShell();
            if (String.IsNullOrEmpty(value))
                return 0;
            return Int64.Parse(value) * 1024;
        }

        /// <summary>
        /// 获取空闲内存数
        /// </summary>
        /// <returns></returns>
        [ShellCmd(PlatformID.Win32NT, "cmd", "/c wmic OS get FreePhysicalMemory", @"^FreePhysicalMemory\s*(?'value'.*?)\s*$")]
        public static long GetFreeMemory()
        {
            var value = executeShell();
            if (String.IsNullOrEmpty(value))
                return 0;
            return Int64.Parse(value) * 1024;
        }
    }
}
