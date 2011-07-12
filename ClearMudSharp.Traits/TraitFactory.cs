using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;

namespace ClearMudSharp.Traits
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public class TraitAttribute : Attribute
    {
        public TraitAttribute()
        {
        }
    }



    public class TraitFactory
    {
        private static readonly Dictionary<string, Type> _types = new Dictionary<string, Type>();
        private static readonly AssemblyBuilder _assembly;
        private static readonly ModuleBuilder _module;
        private static readonly object _syncTypes = new object();
        static TraitFactory()
        {
            AssemblyName aName = new AssemblyName("Traits___Module");
            /*_assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);
            _module = _assembly.DefineDynamicModule(aName.Name);*/
            _assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.RunAndSave);
            _module = _assembly.DefineDynamicModule(aName.Name, aName.Name + ".dll");
        }

        private AssemblyBuilder Assembly
        {
            get
            {
                return _assembly;
            }
        }

        private ModuleBuilder Module
        {
            get
            {
                return _module;
            }
        }
        private static readonly TypeComparer _typeComparer = new TypeComparer();
        private class TypeComparer : IEqualityComparer<Type>
        {
            public bool Equals(Type x, Type y)
            {
                return x.AssemblyQualifiedName == y.AssemblyQualifiedName;
            }

            public int GetHashCode(Type obj)
            {
                return obj.AssemblyQualifiedName.GetHashCode();
            }
        }

        public T Create<T>()
        {
            var returnType = typeof(T);
            var key = returnType.AssemblyQualifiedName;
            Type type;
            lock (_syncTypes)
            {
                if (!_types.ContainsKey(key))
                {
                    var interfaces = GetInterfaces(returnType);

                    interfaces = returnType.IsInterface ? new[] { returnType }.Concat(interfaces) : interfaces.Except(GetInterfaces(returnType.BaseType),_typeComparer);
                    var methodGroups = from i in interfaces
                                       let attribute = (TraitAttribute)i.GetCustomAttributes(typeof(TraitAttribute), false).SingleOrDefault()
                                       where attribute != null
                                  let trait = i.GetGenericArguments().First()
                                  from m in trait.GetMethods()
                                  let args = m.GetParameters().Skip(1).Select(p => p.ParameterType).ToArray()
                                  let im = i.GetMethod(m.Name, args, null)
                                  where im != null
                                  group new
                                  {
                                      StaticMethodDefinition = m,
                                      InterfaceMethodDeclaration = im,
                                      Args = args
                                  } by im.Name + string.Join("#", args.Select(a => a.AssemblyQualifiedName));

                    var tb = _module.DefineType("<>" + returnType.FullName, TypeAttributes.Public, returnType.IsInterface ? typeof(object) : returnType, interfaces.ToArray());

                    CreateDefaultConstructor(tb);

                    foreach (var methods in methodGroups)
                    {
                        var first = methods.First();
                        //Make the first an implicit implementation and the rest explicit
                        var explicitImplementation = !(returnType.IsInterface || returnType.BaseType.GetMethod(first.InterfaceMethodDeclaration.Name, first.Args) == null); 
                        foreach (var md in methods)
                        {
                            var interfaceMethod = md.InterfaceMethodDeclaration;
                            var args = md.Args;
                            var methodDefinition = md.StaticMethodDefinition;
                            var methodAttributes = explicitImplementation ? MethodAttributes.Private
                                                    | MethodAttributes.HideBySig
                                                    | MethodAttributes.NewSlot
                                                    | MethodAttributes.Virtual
                                                    | MethodAttributes.Final :
                                                    MethodAttributes.Public
                                                    | MethodAttributes.Virtual
                                                    | MethodAttributes.Final;

                            var method = tb.DefineMethod(methodDefinition.Name, methodAttributes, methodDefinition.ReturnType, args);
                            var generator = method.GetILGenerator();
                            generator.Emit(OpCodes.Ldarg_0);
                            for (var i = 0; i < args.Length; i++)
                            {
                                generator.EmitArg(i + 1);
                            }
                            generator.Emit(OpCodes.Tailcall);
                            generator.Emit(OpCodes.Call, methodDefinition);
                            generator.Return();
                            if (explicitImplementation)
                            {
                                tb.DefineMethodOverride(method, interfaceMethod);
                            }
                            explicitImplementation = true;
                        }
                    }
                    _types.Add(key, tb.CreateType());
                    Assembly.Save(Assembly.GetName().Name + ".dll");
                }
                type = _types[key];
            }
            return (T)type.GetConstructor(Type.EmptyTypes).Invoke(null);
        }

        private static IEnumerable<Type> GetInterfaces(Type type)
        {

            IEnumerable<Type> interfaces = type.GetInterfaces();
            if(interfaces == null || !interfaces.Any())
                return Enumerable.Empty<Type>();

            var filteredInterfaces = (from i in interfaces
                              where i.GetCustomAttributes(typeof(TraitAttribute), false).Any()
                              select i).ToArray();
            var collectedInterfaces = filteredInterfaces.Concat(interfaces.SelectMany(i => GetInterfaces(i)));
            return collectedInterfaces;
        }

        private static void CreateDefaultConstructor(TypeBuilder tb)
        {
            var ctor = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            var generator = ctor.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Call,
                typeof(object).GetConstructor(Type.EmptyTypes));
            generator.Return();
        }
    }

    internal static class GeneratorExtensions
    {
        public static void EmitArg(this ILGenerator generator, int i)
        {
            switch (i)
            {
                case 0:
                    generator.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    generator.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    generator.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    generator.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    generator.Emit(OpCodes.Ldarg, i);
                    break;
            }
        }

        public static void Return(this ILGenerator generator)
        {
            generator.Emit(OpCodes.Ret);
        }
    }
}
