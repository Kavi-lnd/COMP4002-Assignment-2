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
        public string Execute(
            string encodedCode,
            string expectedHash,
            string functionName,
            List<object> arguments)
        {
            byte[] hashBytes = SHA256.Create().ComputeHash(
                Encoding.UTF8.GetBytes(encodedCode));

            string actualHash =
                BitConverter.ToString(hashBytes).Replace("-", "");

            if (!string.Equals(
                actualHash, expectedHash,
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Python code hash verification failed.");
            }

            byte[] codeBytes = Convert.FromBase64String(encodedCode);
            string code = Encoding.UTF8.GetString(codeBytes);

            ScriptEngine engine = Python.CreateEngine();
            ScriptScope scope = engine.CreateScope();

            engine.Execute(code, scope);
            dynamic function = scope.GetVariable(functionName);

            object result = engine.Operations.Invoke(function, arguments.ToArray());

            return result == null ? "" : result.ToString();
        }
    }
}


