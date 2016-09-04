using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.CSharp;

namespace CodeSnippetDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //the code template with placehoder for the code snippet.
            var sourceTemplate =
                @"using System; 
  using System.Windows.Forms; 

  namespace Foo { 
     public static class Bar { 
       public static void Execute() {
         @Placeholder
       }
     }
   }";

            var snippet = "int i = 2; int j = 3; int result = i + j; MessageBox.Show(result.ToString());";
            var sourceCode = sourceTemplate.Replace("@Placeholder", snippet);
            var snippetCompileUnit = new CodeSnippetCompileUnit(sourceCode);

            using (var provider =
                new CSharpCodeProvider(new Dictionary<String, String> {{"CompilerVersion", "v3.5"}}))
            {
                var parameters = new CompilerParameters();
                parameters.ReferencedAssemblies.Add("System.dll");
                parameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
                parameters.GenerateExecutable = false;
                parameters.GenerateInMemory = true;
                parameters.IncludeDebugInformation = false;

                var results = provider.CompileAssemblyFromDom(parameters, snippetCompileUnit);

                //from the compile result and reflection to run the code
                if (!results.Errors.HasErrors)
                {
                    var type = results.CompiledAssembly.GetType("Foo.Bar");
                    var method = type.GetMethod("Execute");
                    method.Invoke(null, new object[] {});
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (CompilerError compilerError in results.Errors)
                        sb.AppendFormat("Error in line {0}:\n\n{1}", compilerError.Line, compilerError.ErrorText);
                    MessageBox.Show(sb.ToString(), "Compiler Error");
                }
            }
        }
    }
}
