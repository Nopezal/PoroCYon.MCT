using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CSharp;
using PoroCYon.MCT.Tools.Compiler;

namespace PoroCYon.MCT.Tools.Internal.Compiler.Compilers
{
    class CSharpCompiler(ModCompiler mc) : CodeDomCompilerHelper(mc)
    {
        readonly static string[]
            ext  = new string[] { ".cs", ".csx" },
            asm  = new string[] { "Microsoft.CSharp.dll" },
            lang = new string[] { "csharp", "c#", "cs" };

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
            return new CSharpCodeProvider();
        }
    }
}
