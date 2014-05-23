using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualBasic;

namespace PoroCYon.MCT.Tools.Compiler.Internal.Compilation
{
    class VBCompiler : CodeDomCompilerHelper
    {
        readonly static string[]
            ext  = new string[] { ".vb", ".vbs" },
            asm  = new string[] { "Microsoft.VisualBasic.dll" },
            lang = new string[] { "visual basic", "visualbasic", "vb", "visual basic .net", "visualbasic.net" };

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
            return new VBCodeProvider();
        }
    }
}
