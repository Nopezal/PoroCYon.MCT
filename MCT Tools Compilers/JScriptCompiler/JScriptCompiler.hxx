#pragma once

using namespace System;
using namespace System::CodeDom::Compiler;
using namespace System::Collections::Generic;
using namespace System::Linq;
using namespace Microsoft::JScript;

namespace PoroCYon { namespace MCT { namespace Tools { namespace Compiler { namespace Compilers {

    public ref class JScriptCompier : public CodeDomCompilerHelper {
        static array<String^>^ ext = { ".js" };
        static array<String^>^ refs = { "Microsoft.JScript.dll" };
        static array<String^>^ lang = { "jscript", "jscript.net", "js", "javascript", "javascript.net", "ecmascript" };

    public:
        property array<String^>^ FileExtensions {
            array<String^>^ get() override {
                return ext;
            }
        }
        property array<String^>^ LanguageDependancyAssemblies {
            array<String^>^ get() override {
                return refs;
            }
        }
        property array<String^>^ LanguageNames {
            array<String^>^ get() override {
                return lang;
            }
        }

    protected:
        CodeDomProvider^ CreateCompiler() override {
            return gcnew JScriptCodeProvider();
        }
    };

} } } } }
