using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.JScript;
using PoroCYon.MCT.Tools.Compiler;

namespace PoroCYon.MCT.Tools.Internal.Compiler.Compilers
{
    class JScriptCompiler : CodeDomCompilerHelper
    {
        readonly static string[]
            ext  = new string[] { ".js" },
            asm  = new string[] { "Microsoft.JScript.dll" },
            lang = new string[] { "jscript", "jscript.net", "js", "javascript", "javascript.net", "ecmascript" };

        public override string[] FileExtensions
        {
            get
            {
                return ext;
            }
        }
        public override string[] LanguageDependancyAssemblies
        {
            get
            {
                return asm;
            }
        }
        public override string[] LanguageNames
        {
            get
            {
                return lang;
            }
        }

        protected override CodeDomProvider CreateCompiler()
        {
            return new JScriptCodeProvider();
        }
    }
}
