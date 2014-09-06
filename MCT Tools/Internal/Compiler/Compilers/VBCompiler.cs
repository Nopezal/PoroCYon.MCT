using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic;
using PoroCYon.MCT.Tools.Compiler;

namespace PoroCYon.MCT.Tools.Internal.Compiler.Compilers
{
    class VBCompiler(ModCompiler mc) : CodeDomCompilerHelper(mc)
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
        protected override void ModifyCompilerParameters(CompilerParameters cp)
        {
            base.ModifyCompilerParameters(cp);

            Path.ChangeExtension(cp.OutputAssembly, null);
        }
    }
}
