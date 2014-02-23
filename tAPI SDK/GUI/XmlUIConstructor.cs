using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;
using Microsoft.CSharp;
using TAPI.SDK.GUI.Controls;

namespace TAPI.SDK.GUI
{
    /// <summary>
    /// A service to convert an XML document to a <see cref="TAPI.SDK.GUI.CustomUI"/>
    /// </summary>
    public class XmlUIConstructor : CustomUI
    {
        XmlUIConstructor()
            : base()
        {

        }

        static List<Assembly> assemblies = new List<Assembly>();
        static List<string> namespaces = new List<string>();
        static List<Tuple<string, object>> objects = new List<Tuple<string, object>>();

        /// <summary>
        /// Create a CustomUI from an .xml file
        /// </summary>
        /// <param name="file">The path to the .xml file</param>
        /// <returns>The <see cref="TAPI.SDK.GUI.XmlUIConstructor"/> created from the .xml file</returns>
        public static XmlUIConstructor FromFile(string file)
        {
            return FromString(File.ReadAllText(file));
        }
        /// <summary>
        /// Creates a CustomUI from XML code
        /// </summary>
        /// <param name="xml">The XML code</param>
        /// <returns>The <see cref="TAPI.SDK.GUI.XmlUIConstructor"/> created from the XML code</returns>
        public static XmlUIConstructor FromString(string xml)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(xml);

            return FromDocument(xd);
        }
        /// <summary>
        /// Creates a CustomUI from an XmlDocument
        /// </summary>
        /// <param name="document">The XmlDocument</param>
        /// <returns>The <see cref="TAPI.SDK.GUI.XmlUIConstructor"/> created from the XmlDocument</returns>
        public static XmlUIConstructor FromDocument(XmlDocument document)
        {
            if (document.FirstChild.Name.ToLower() != "sdkxmlui")
                return null;

            objects = new List<Tuple<string, object>>();
            #region assemblies = new List<Assembly>()
            assemblies = new List<Assembly>()
            {
                Assembly.LoadFrom("mscorlib.dll"),
                Assembly.LoadFrom("System.dll"),
                Assembly.LoadFrom("System.Core.dll"),
                Assembly.LoadFrom("System.Data.dll"),
                Assembly.LoadFrom("System.Numerics.dll"),
                Assembly.LoadFrom("System.Xml.dll"),

                Assembly.LoadFrom("System.Windows.Forms.dll"),
                Assembly.LoadFrom("System.Drawing.dll"),

                Assembly.LoadFrom("WindowsBase.dll"),
                Assembly.LoadFrom("PresentationCore.dll"),
                Assembly.LoadFrom("PresentationFramework.dll"),
                Assembly.LoadFrom("WindowsFormsIntegration.dll"),

                Assembly.LoadFrom("Microsoft.Xna.Framework.dll"),
                Assembly.LoadFrom("Microsoft.Xna.Framework.Game.dll"),
                Assembly.LoadFrom("Microsoft.Xna.Framework.Graphics.dll"),
                Assembly.LoadFrom("Microsoft.Xna.Framework.Xact.dll"),

                Assembly.LoadFrom("PoroCYon.XnaExtensions.dll"),

                Assembly.LoadFrom("tAPI.exe"),

                Assembly.GetExecutingAssembly()
            };
            #endregion
            #region namespaces = new List<string>()
            namespaces = new List<string>()
            {
                "System",
                "System.Collections.Generic",
                "System.Linq",
                "System.Text",

                "Microsoft.Xna.Framework",
                "Microsoft.Xna.Framework.Graphics",
                "PoroCYon.XnaExtensions",

                "TAPI",
                "TAPI.SDK",
                "TAPI.SDK.GUI",
                "TAPI.SDK.GUI.Controls"
            };
            #endregion

            for (int i = 0; i < document.FirstChild.ChildNodes.Count; i++)
            {
                XmlNode Xn = document.FirstChild.ChildNodes[i];

                #region references
                if (Xn.Name.ToLower() == "references")
                {
                    for (int j = 0; j < Xn.ChildNodes.Count; j++)
                    {
                        if (Xn.ChildNodes[j].Name.ToLower() != "reference")
                            continue;

                        for (int k = 0; k < Xn.ChildNodes[j].ChildNodes.Count; k++)
                        {
                            XmlNode XN = Xn.ChildNodes[j].ChildNodes[k];

                            #region attributes
                            for (int l = 0; l < XN.Attributes.Count; l++)
                            {
                                XmlAttribute xa = XN.Attributes[l];

                                if (xa.Name.ToLower() == "assembly")
                                {
                                    string asm = xa.Value.ToLower();

                                    Assembly a = Assembly.LoadFrom(xa.Value);

                                    if (asm == "$calling$")
                                        a = Assembly.GetCallingAssembly();
                                    if (asm == "$entry$")
                                        a = Assembly.GetEntryAssembly();
                                    if (asm == "$executing$")
                                        a = Assembly.GetExecutingAssembly();

                                    if (!assemblies.Contains(a))
                                        assemblies.Add(a);
                                }

                                if (xa.Name.ToLower() == "namespace")
                                    if (!namespaces.ConvertAll<string>((s) => { return s.ToLower(); }).Contains(xa.Value.ToLower()))
                                        namespaces.Add(xa.Value);
                            }
                            #endregion

                            #region inner
                            if (XN.Name.ToLower() == "assembly")
                            {
                                string asm = XN.InnerText.ToLower();

                                Assembly a = Assembly.LoadFrom(XN.InnerText);

                                if (asm == "$calling$")
                                    a = Assembly.GetCallingAssembly();
                                if (asm == "$entry$")
                                    a = Assembly.GetEntryAssembly();
                                if (asm == "$executing$")
                                    a = Assembly.GetExecutingAssembly();

                                if (!assemblies.Contains(a))
                                    assemblies.Add(a);
                            }

                            if (XN.Name.ToLower() == "namespace")
                                if (!namespaces.ConvertAll<string>((s) => { return s.ToLower(); }).Contains(XN.InnerText.ToLower()))
                                    namespaces.Add(XN.InnerText);
                            #endregion
                        }
                    }

                    continue;
                }
                #endregion

                objects.Add(CreateObject(Xn));
            }

            XmlUIConstructor ret = new XmlUIConstructor();

            for (int i = 0; i < objects.Count; i++)
                ret.AddControl(objects[i].Item2 as Control); // null will be filtered in the first line of AddControl

            return ret;
        }

        static Type GetTypeOf(string name)
        {
            Assembly asm = null;
            Type ret = null;

            // has assembly + namespace
            if (name.Contains('-'))
            {
                asm = assemblies.FirstOrDefault((a) =>
                {
                    return name.Split('-')[0] == a.FullName.Split(',')[0];
                });

                if (asm == default(Assembly))
                    return null;

                ret = asm.GetType(name.Split('-')[1], false, true);
            }
            else
            {
                // has namespace
                if (name.Contains('.'))
                    for (int i = 0; i < assemblies.Count; i++)
                    {
                        ret = assemblies[i].GetType(name, false, true);

                        if (ret != null)
                            break;
                    }
                else
                    // fml
                    for (int i = 0; i < assemblies.Count; i++)
                        for (int j = 0; j < namespaces.Count; j++)
                        {
                            ret = assemblies[i].GetType(namespaces[i] + "." + name, false, true);

                            if (ret != null)
                                break;
                        }

            }

            return ret;
        }

        static Tuple<string, object> CreateObject(XmlNode Xn)
        {
            if (Xn.Name.ToLower() == "ctor")
                return null;

            if (Xn.Name.ToLower() == "_node")
                return objects.FirstOrDefault((_t) =>
                {
                    return _t.Item1 == Xn.Attributes["Name"].Value.Substring(1, Xn.Attributes["Name"].Value.Length - 2);
                });

            string name = null;
            {
                XmlAttribute xa = Xn.Attributes["_Name"];
                if (xa != null)
                    name = xa.Value;
            }

            if (Xn.Name.ToLower() == "obj")
                return CreateDynamicObject(Xn);

            object[] ctorArgs = new object[0];

            if (Xn.FirstChild != null && Xn.FirstChild.Name.ToLower() == "ctor")
                ctorArgs = GetCtorArguments(Xn.FirstChild.ChildNodes);

            Type t = GetTypeOf(Xn.Name);
            object ret = Instantiate(t, ctorArgs, FromAttributes(GetTypeOf(Xn.Name), Xn.Attributes));

            for (int i = 1; i < Xn.ChildNodes.Count; i++)
            {
                if (Xn.ChildNodes[i].Name.ToLower() == "ctor")
                    continue;

                MemberInfo mi = GetFieldOrProperty(t, Xn.ChildNodes[i].Name);
                if (mi == null)
                    continue;

                if (mi.MemberType == MemberTypes.Field)
                    (mi as FieldInfo).SetValue(ret, CreateObject(Xn.ChildNodes[i]));
                if (mi.MemberType == MemberTypes.Property)
                    (mi as PropertyInfo).SetValue(ret, CreateObject(Xn.ChildNodes[i]), null);
            }

            return new Tuple<string, object>(name, ret);
        }
        static object[] GetCtorArguments(XmlNodeList nodes)
        {
            List<object> ret = new List<object>();

            for (int i = 0; i < nodes.Count; i++)
                ret.Add(CreateObject(nodes[i]).Item2);

            return ret.ToArray();
        }

        static object Instantiate(Type t, object[] args, Tuple<string, object>[] fields)
        {
            object ret = ReflectionHelper.Instantiate(t, args);

            if (ret == null)
                return null;

            SetFields(ref ret, fields);

            return ret;
        }
        static object CreateObject(XmlNode node, object[] args, Tuple<string, object>[] fields)
        {
            return Instantiate(GetTypeOf(node.Name), args, fields);
        }
        static Delegate CreateDelegate(string value)
        {
            if (value.ToLower().StartsWith("code:"))
            {
                // compile with CodeDom, return Action<UIConstructor>

                string source =
                    #region source
@"using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Xact;
using PoroCYon.XnaExtensions;
using TAPI.SDK.GUI.Controls;

namespace TAPI.SDK.GUI
{
    public static class UIConstructorScript
    {
        public static Action<UIConstructor> ScriptAction
        {
            get
            {
                return ExecuteScript;
            }
        }

        static void ExecuteScript(UIConstructor ui)
        {
            " + value + @"
        }
    }
}
";
                    #endregion
                CodeDomProvider cdp = new CSharpCodeProvider();
                CompilerParameters cp = new CompilerParameters()
                {
                    GenerateExecutable = false,
                    GenerateInMemory = true,
                    CompilerOptions = "/optimize",
                    IncludeDebugInformation = false,
                    OutputAssembly = "TAPI.SDK.GUI.ScriptAssembly"
                };
                cp.ReferencedAssemblies.AddRange(new string[]
                {
                    "mscorlib.dll", "System.dll", "System.Core.dll", "System.Numerics.dll",
                    "Microsoft.Xna.Framework.dll", "Microsoft.Xna.Framework.Game.dll", "Microsoft.Xna.Framework.Graphics.dll", "Microsoft.Xna.Framework.Xact.dll",
                    "PoroCYon.XnaExtensions.dll", "tAPI.exe", "TAPI.SDK.dll", Assembly.GetCallingAssembly().FullName
                });

                CompilerResults cr = cdp.CompileAssemblyFromSource(cp, source);

                if (cr.Errors.HasErrors)
                {
                    for (int i = 0; i < cr.Errors.Count; i++)
                        TConsole.Print(cr.Errors[i]);

                    return null;
                }

                return (Action<XmlUIConstructor>)cr.CompiledAssembly
                    .GetType("TAPI.SDK.GUI.UIConstructorScript", true, true)
                    .GetProperty("ScriptAction", BindingFlags.GetProperty | BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Static)
                    .GetValue(null, null);
            }

            string[] split = value.Split('.');
            string @class = split[0];
            for (int i = 1; i < split.Length; i++)
                @class += "." + split[i];
            string method = split[split.Length - 1];

            Type t = Type.GetType(@class, false, true);
            if (t == null)
                return null;

            MethodInfo mi = t.GetMethod(method, BindingFlags.IgnoreCase | BindingFlags.Static);
            if (mi == null)
                return null;

            return Delegate.CreateDelegate(Expression.GetDelegateType((from p in mi.GetParameters() select p.ParameterType)
                .Concat(new Type[1] { mi.ReturnType }).ToArray()), null, method, true, false);
        }
        static Tuple<string, object>[] FromAttributes(Type parent, XmlAttributeCollection attr)
        {
            List<Tuple<string, object>> ret = new List<Tuple<string, object>>();

            for (int i = 0; i < attr.Count; i++)
            {
                object fromNode = FromNode(attr[i].Value);
                if (fromNode != null)
                    ret.Add(new Tuple<string, object>(attr[i].Name, fromNode));

                MemberInfo mi = GetFieldOrProperty(parent, attr[i].Name);
                if (mi == null)
                    continue;

                ret.Add(new Tuple<string, object>(attr[i].Name, FromString(mi.ReflectedType, attr[i].Value)));
            }

            return ret.ToArray();
        }
        static object FromString(Type t, string value)
        {
            if (t.IsPointer || t.IsInterface || t.IsAbstract || t.IsArray)
                return null;

            object ret = FromNode(value);
            if (ret != null)
                return ret;

            ret = CheckForLiterals(value, t);
            if (ret != null)
                return ret;

            if (t.IsEnum)
                return Enum.Parse(t, value, true);

            if (t.IsClass || t.IsValueType)
            {
                ret = ReflectionHelper.Instantiate(t);

                List<Tuple<string, string>> fields = new List<Tuple<string, string>>();

                string[] split = value.Split(';');
                for (int i = 0; i < split.Length; i++)
                {
                    split[i] = split[i].Trim();

                    string
                        name = split[i].Split('=')[0].Trim(),
                        fieldvalue = split[i].Split('=')[1].Trim();

                    MemberInfo mi = GetFieldOrProperty(t, name);
                    if (mi == null)
                        continue;

                    if (mi.MemberType == MemberTypes.Field)
                    {
                        FieldInfo fi = mi as FieldInfo;

                        fi.SetValue(ret, FromString(mi.ReflectedType, fieldvalue));
                    }
                    if (mi.MemberType == MemberTypes.Property)
                    {
                        PropertyInfo pi = mi as PropertyInfo;

                        pi.SetValue(ret, FromString(mi.ReflectedType, fieldvalue), null);
                    }

                    fields.Add(new Tuple<string, string>(name, fieldvalue));
                }
            }

            return null;
        }
        static dynamic CreateDynamicObject(XmlNode xn)
        {
            dynamic ret = new ExpandoObject() as IDictionary<string, object>;

            for (int i = 0; i < xn.ChildNodes.Count; i++)
            {
                if (xn.ChildNodes[i].Name.ToLower() == "ctor")
                    continue;

                ret.Add(xn.ChildNodes[i].Name, CreateObject(xn.ChildNodes[i]).Item2);
            }

            return ret;
        }
        static bool InstanceOf(Type type, Type baseType)
        {
            if (type == baseType)
                return true;

            if (type == null || baseType == null)
                return false;

            if (baseType == typeof(object))
                return true;

            Type current = type;

            do
            {
                if (current.BaseType == baseType)
                    return true;
            } while (current.BaseType != typeof(object));

            return false;
        }
        static object CheckForLiterals(string value, Type t)
        {
            object ret = FromNode(value);
            if (ret != null)
                return ret;

            if (InstanceOf(t, typeof(byte)))
                return Byte.Parse(value, CultureInfo.InvariantCulture);
            if (InstanceOf(t, typeof(sbyte)))
                return SByte.Parse(value, CultureInfo.InvariantCulture);

            if (InstanceOf(t, typeof(short)))
                return Int16.Parse(value, CultureInfo.InvariantCulture);
            if (InstanceOf(t, typeof(ushort)))
                return UInt16.Parse(value, CultureInfo.InvariantCulture);

            if (InstanceOf(t, typeof(int)))
                return Int32.Parse(value, CultureInfo.InvariantCulture);
            if (InstanceOf(t, typeof(uint)))
                return UInt32.Parse(value, CultureInfo.InvariantCulture);

            if (InstanceOf(t, typeof(long)))
                return Int64.Parse(value, CultureInfo.InvariantCulture);
            if (InstanceOf(t, typeof(ulong)))
                return UInt64.Parse(value, CultureInfo.InvariantCulture);

            if (InstanceOf(t, typeof(float)))
                return Single.Parse(value, CultureInfo.InvariantCulture);
            if (InstanceOf(t, typeof(double)))
                return Double.Parse(value, CultureInfo.InvariantCulture);
            if (InstanceOf(t, typeof(decimal)))
                return Decimal.Parse(value, CultureInfo.InvariantCulture);

            if (InstanceOf(t, typeof(DateTime)))
                return DateTime.Parse(value, CultureInfo.InvariantCulture);
            if (InstanceOf(t, typeof(TimeSpan)))
                return TimeSpan.Parse(value, CultureInfo.InvariantCulture);

            if (InstanceOf(t, typeof(string)))
                return value;

            if (InstanceOf(t, typeof(Delegate)))
                return CreateDelegate(value);

            return null;
        }
        static void SetFields(ref object o, Tuple<string, object>[] fields)
        {
            Type t = o.GetType();

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo fi = t.GetField(fields[i].Item1);

                if (fi != null)
                    fi.SetValue(o, fields[i].Item2);
                else
                {
                    PropertyInfo pi = t.GetProperty(fields[i].Item1, BindingFlags.SetProperty);
                    if (pi != null)
                        pi.SetValue(o, fields[i].Item2, null);
                }
            }
        }
        static object FromNode(string nodeName)
        {
            if (!nodeName.StartsWith("{") && !nodeName.EndsWith("}"))
                return null;

            for (int i = 0; i < objects.Count; i++)
                if (objects[i].Item1.ToLower() == nodeName.Substring(1, nodeName.Length - 2).ToLower())
                    return objects[i].Item2;

            return null;
        }
        static MemberInfo GetFieldOrProperty(Type t, string name)
        {
            MemberInfo ret = t.GetField(name, BindingFlags.SetField | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (ret != null)
                return ret;

            return t.GetProperty(name, BindingFlags.SetProperty | BindingFlags.IgnoreCase | BindingFlags.Instance);
        }
    }
}
