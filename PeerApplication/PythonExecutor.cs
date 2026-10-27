using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PeerApplication
{
    internal class PythonExecutor
    {
        public string Execute(string encodedCode, string expectedHash,
            string functionName, List<object> arguments)
        {       // calcualte the hash that is supplied in the code that is endcoded
            byte[] hashBytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(encodedCode));
            // hast converted into string that is hexdecimal
            string actualHash = BitConverter.ToString(hashBytes).Replace("-", "");

            // hash matches the hash that is expected is checked
            if (!string.Equals(actualHash, expectedHash, StringComparison.OrdinalIgnoreCase))
            { throw new InvalidOperationException("Python code hash verification has failed!!!"); }

            //base 64 code into bytes (Python code)
            byte[] codeBytes = Convert.FromBase64String(encodedCode);

            // bytes into string ( Python code )
            string code = Encoding.UTF8.GetString(codeBytes);
             //scripting engine (python)
            ScriptEngine engine = Python.CreateEngine();



            // scope is created
            ScriptScope scope = engine.CreateScope();
            // python code is ec=xceuted
            engine.Execute(code, scope);
            // func from python code is taken
             dynamic function = scope.GetVariable(functionName);
            object result = engine.Operations.Invoke(function, arguments.ToArray());

            // empty string for a null reslut
            return result == null ? "" : result.ToString();


        }  } }
    


