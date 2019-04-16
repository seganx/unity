using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace SeganX.Console
{
    public class Console_Command : MonoBehaviour
    {
        public class MethodObject
        {
            public string space;
            public string name;
            public string help;
            public MethodInfo info;
        }

        public InputField userInput = null;

        private List<MethodObject> methods = new List<MethodObject>();

        void Awake()
        {
            var assembly = System.AppDomain.CurrentDomain.Load("Assembly-CSharp");
            var allMethods = assembly.GetTypes().SelectMany(x => x.GetMethods()).Where(y => y.GetCustomAttributes(true).OfType<ConsoleAttribute>().Any()).ToList();
            foreach (var method in allMethods)
            {
                var attribs = method.GetCustomAttributes(true);
                foreach (var attrib in attribs)
                {
                    if (attrib.IsTypeOf<ConsoleAttribute>())
                    {
                        var cattrib = attrib.As<ConsoleAttribute>();

                        if (cattrib.cmdName.IsNullOrEmpty())
                            cattrib.cmdName = method.Name.ToLower();

                        if (cattrib.cmdhelp.IsNullOrEmpty())
                            cattrib.cmdhelp = GenerateMethodHelp(method);

                        methods.Add(new MethodObject() { space = cattrib.cmdSpace, name = cattrib.cmdName, help = cattrib.cmdhelp, info = method });
                    }
                }
            }
        }

        public void RunCommand()
        {
            var str = userInput.text;

            //  handle help command
            if (str.ToLower() == "help")
            {
                string helpStr = methods.Count > 0 ? "List of commands:\n" : "No command founded!";
                foreach (var item in methods)
                {
                    helpStr += item.space + " " + item.name + " " + item.help + "\n";
                }
                Debug.Log(helpStr);
                return;
            }


            Debug.Log("Execute: " + str);

            string[] cmd = str.Split(' ');
            if (cmd.Length < 2)
            {
                Debug.Log("Nothing to execute!");
                return;
            }

            var space = cmd[0].ToLower();
            var name = cmd[1].ToLower();
            var method = methods.Find(x => x.space == space && x.name == name);
            if (method == null)
            {
                Debug.Log("Command not found!");
                return;
            }
            else if (!method.info.IsStatic)
            {
                Debug.Log("Function is not static!");
                return;
            }

            var methodParams = method.info.GetParameters();
            if (methodParams.Length == 0)
            {
                method.info.Invoke(null, null);
                return;
            }
            else if (methodParams.Length != cmd.Length - 2)
            {
                Debug.Log("Mismatched parameters!\n" + method.space + " " + method.name + " " + method.help);
                return;
            }

            var arglist = new object[methodParams.Length];
            for (int i = 0; i < arglist.Length; i++)
            {
                var methodParam = methodParams[i];

                if (methodParam.ParameterType == typeof(bool))
                    arglist[i] = cmd[i + 2].ToBoolean();
                else if (methodParam.ParameterType == typeof(int))
                    arglist[i] = cmd[i + 2].ToInt();
                else if (methodParam.ParameterType == typeof(string))
                    arglist[i] = cmd[i + 2];
                else if (methodParam.ParameterType == typeof(float))
                    arglist[i] = cmd[i + 2].ToFloat();
                else
                {
                    Debug.Log("Not a type value!");
                    return;
                }
            }
            method.info.Invoke(null, arglist);
        }

        private static string GetTypeName(System.Type type)
        {
            if (type == typeof(bool))
                return "bool";
            else if (type == typeof(int))
                return "int";
            else if (type == typeof(string))
                return "string";
            else if (type == typeof(float))
                return "float";
            else
                return type.Name;
        }

        private static string GenerateMethodHelp(MethodInfo info)
        {
            var methodParams = info.GetParameters();
            if (methodParams.Length < 1) return string.Empty;
            var stringParams = new string[methodParams.Length];
            for (int i = 0; i < methodParams.Length; i++)
                stringParams[i] = methodParams[i].Name + ":" + GetTypeName(methodParams[i].ParameterType);
            return string.Join(" ", stringParams);
        }
    }
}