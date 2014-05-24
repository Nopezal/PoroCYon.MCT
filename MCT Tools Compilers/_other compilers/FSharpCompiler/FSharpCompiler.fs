namespace PoroCYon.MCT.Tools.Compiler.Compilers

open System
open System.CodeDom.Compiler
open System.Collections.Generic
open System.Linq
open PoroCYon.MCT.Tools.Compiler
open Microsoft.FSharp.Compiler.CodeDom

type FSharpCompiler() = 
    inherit CodeDomCompilerHelper()

    let ext  = [| ".fs"; ".fsx"        |]
    let asm  = [| "FSharp.Core.dll"    |]
    let name = [| "fsharp"; "f#"; "fs" |]

    override this.FileExtensions               with get() = ext
    override this.LanguageDependancyAssemblies with get() = asm
    override this.LanguageNames                with get() = name

    override this.CreateCompiler () = (new FSharpCodeProvider() :> CodeDomProvider)
