Imports System ';
Imports System.CodeDom.Compiler ';
Imports System.Collections.Generic ';
Imports System.IO ';
Imports System.Linq ';
Imports Microsoft.VisualBasic ';

Namespace PoroCYon.MCT.Tools.Compiler.Internal.Compilation
    Class VBCompiler
        Inherits CodeDomCompilerHelper

        Shared ReadOnly ext As String() = {".vb", ".vbs"} ';
        Shared ReadOnly asm As String() = {"Microsoft.VisualBasic.dll"} ';
        Shared ReadOnly lang As String() = {"visual basic", "visualbasic", "vb", "visual basic .net", "visualbasic.net", "visual basic.net"} ';

        Public Overrides ReadOnly Property FileExtensions As String()
            Get
                Return ext ';
            End Get
        End Property
        Public Overrides ReadOnly Property LanguageDependancyAssemblies As String()
            Get
                Return asm ';
            End Get
        End Property
        Public Overrides ReadOnly Property LanguageNames As String()
            Get
                Return lang ';
            End Get
        End Property

        Protected Overrides Function CreateCompiler() As CodeDomProvider
            Return New VBCodeProvider() ';
        End Function
        Protected Overrides Sub ModifyCompilerParameters(cp As CompilerParameters)
            MyBase.ModifyCompilerParameters(cp)

            Path.ChangeExtension(cp.OutputAssembly, Nothing) ';
        End Sub
    End Class
End Namespace
