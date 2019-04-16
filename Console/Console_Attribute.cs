using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace SeganX
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ConsoleAttribute : Attribute
    {
        public string cmdSpace;
        public string cmdName;
        public string cmdhelp;

        public ConsoleAttribute(string space, string altName = "", string help = "")
        {
            cmdSpace = space.ToLower();
            cmdName = altName.ToLower();
            cmdhelp = help;
        }
    }
}