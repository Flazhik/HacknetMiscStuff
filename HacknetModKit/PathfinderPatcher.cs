using System;
using System.IO;
using System.Linq;
using System.Security;
using Mono.Cecil;
using Mono.Cecil.Cil;
using CustomAttributeNamedArgument = Mono.Cecil.CustomAttributeNamedArgument;
using MethodAttributes = Mono.Cecil.MethodAttributes;
using SR = System.Reflection;

namespace HacknetModKit
{
    public static class PathfinderPatcher
    {
        public static void Patch(string srcExecutable, string dstExecutable)
        {
            using (var hn = AssemblyDefinition.ReadAssembly(srcExecutable))
            {
                void MakePublic(TypeDefinition type, bool nested = false)
                {
                    if (nested)
                    {
                        type.IsNestedPublic = true;
                    }
                    else
                    {
                        type.IsPublic = true;
                    }

                    foreach (var method in type.Methods)
                        method.IsPublic = true;
                    foreach (var field in type.Fields)
                        field.IsPublic = true;
                    foreach (var property in type.Properties)
                    {
                        if (property.GetMethod != null)
                            property.GetMethod.IsPublic = true;
                        if (property.SetMethod != null)
                            property.SetMethod.IsPublic = true;
                    }

                    foreach (var nestedType in type.NestedTypes)
                    {
                        MakePublic(nestedType, true);
                    }
                }

                // Make everything public
                foreach (var type in hn.MainModule.Types)
                {
                    MakePublic(type);
                }
                
                // We want to run before everything else, including the Main program function, so we make a static module constructor
                var hnEntryType = hn.MainModule.Types.First(x => x.Name == "<Module>");

                var ctor = new MethodDefinition(".cctor",
                    MethodAttributes.Public
                    | MethodAttributes.Static
                    | MethodAttributes.HideBySig
                    | MethodAttributes.SpecialName
                    | MethodAttributes.RTSpecialName,
                    hn.MainModule.TypeSystem.Void
                );

                // Inject the call
                var processor = ctor.Body.GetILProcessor();

                // Get absolute path of BepInEx.Hacknet.dll
                processor.Emit(OpCodes.Ldstr, "./BepInEx/core/BepInEx.EntryPoint.dll");
                processor.Emit(OpCodes.Call,
                    hn.MainModule.ImportReference(
                        typeof(Path).GetMethod("GetFullPath", new[] { typeof(string) })));
                // Load BepInEx.Hacknet.dll
                processor.Emit(OpCodes.Call,
                    hn.MainModule.ImportReference(typeof(SR.Assembly).GetMethod("LoadFile",
                        new[] { typeof(string) })));
                // Get Entrypoint type
                processor.Emit(OpCodes.Ldstr, "Entrypoint.Entrypoint");
                processor.Emit(OpCodes.Call,
                    hn.MainModule.ImportReference(typeof(SR.Assembly).GetMethod("GetType",
                        new[] { typeof(string) })));
                // Get bootstrap method
                processor.Emit(OpCodes.Ldstr, "Bootstrap");
                processor.Emit(OpCodes.Call,
                    hn.MainModule.ImportReference(typeof(Type).GetMethod("GetMethod", new[] { typeof(string) })));
                // Call bootstrap method
                processor.Emit(OpCodes.Ldnull);
                processor.Emit(OpCodes.Ldnull);
                processor.Emit(OpCodes.Callvirt,
                    hn.MainModule.ImportReference(typeof(SR.MethodBase).GetMethod("Invoke",
                        new[] { typeof(object), typeof(object[]) })));
                processor.Emit(OpCodes.Pop);
                // Return
                processor.Emit(OpCodes.Ret);

                hnEntryType.Methods.Add(ctor);

                // Hacknet needs to be able to access *everything*, this ensures that
                var unverifiableType = hn.MainModule.ImportReference(typeof(UnverifiableCodeAttribute)).Resolve();
                var unverifiableCtor =
                    hn.MainModule.ImportReference(
                        unverifiableType.Methods.First(x => x.IsConstructor && x.Parameters.Count == 0));
                hn.MainModule.CustomAttributes.Add(new CustomAttribute(unverifiableCtor));

                var corlibRef = hn.MainModule.AssemblyReferences.FirstOrDefault(x => x.Name == "mscorlib");
                corlibRef.PublicKey = null;
                corlibRef.PublicKeyToken = null;
                corlibRef.HasPublicKey = false;

                var targetRuntime =
                    hn.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == "TargetFrameworkAttribute");
                targetRuntime.ConstructorArguments.Clear();
                targetRuntime.ConstructorArguments.Add(new CustomAttributeArgument(hn.MainModule.TypeSystem.String,
                    ".NETFramework,Version=v4.7.2"));
                targetRuntime.Properties.Clear();
                targetRuntime.Properties.Add(new CustomAttributeNamedArgument("FrameworkDisplayName",
                    new CustomAttributeArgument(hn.MainModule.TypeSystem.String, ".NET Framework 4.7.2")));

                // Write modified assembly to disk
                hn.Write(dstExecutable);
                hn.Dispose();
            }
        }
    }
}