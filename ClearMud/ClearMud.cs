using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ClaySharp;

namespace ClearMudSharp
{
    public abstract class ClearMud<TContext> where TContext : ClearMud<TContext>
    {
        private static readonly dynamic New = new ClayFactory();

        #region GetFunc

        private static dynamic GetFunc(MethodInfo m)
        {
            bool isFunc = IsFunc(m);
            Type[] args =
                (isFunc
                     ? m.GetParameters().Select(p => p.ParameterType).Concat(new[] {m.ReturnType})
                     : m.GetParameters().Select(p => p.ParameterType)).ToArray();

            Type funcType = GetFuncType(args, isFunc);
            return Delegate.CreateDelegate(funcType, m, true);
        }

        private static Func<TReturn> GetFunc<TRole, TReturn>(Func<TRole, TReturn> staticFunc, TRole role1)
        {
            return () => staticFunc(role1);
        }

        private static Func<TReturn> GetFunc<TRole1, TRole2, TReturn>(Func<TRole1, TRole2, TReturn> staticFunc,
                                                                      TRole1 role1, TRole2 role2)
        {
            return () => staticFunc(role1, role2);
        }

        private static Func<TArg, TReturn> GetFunc<TRole1, TArg, TReturn>(Func<TRole1, TArg, TReturn> staticFunc,
                                                                          TRole1 role1)
        {
            return (arg) => staticFunc(role1, arg);
        }

        private static Func<TReturn> GetFunc<TRole1, TRole2, TRole3, TReturn>(
            Func<TRole1, TRole2, TRole3, TReturn> staticFunc, TRole1 role1, TRole2 role2, TRole3 role3)
        {
            return () => staticFunc(role1, role2, role3);
        }

        private static Func<TArg, TReturn> GetFunc<TRole1, TRole2, TArg, TReturn>(
            Func<TRole1, TRole2, TArg, TReturn> staticFunc, TRole1 role1, TRole2 role2)
        {
            return (arg) => staticFunc(role1, role2, arg);
        }

        private static Func<TArg1, TArg2, TReturn> GetFunc<TRole1, TArg1, TArg2, TReturn>(
            Func<TRole1, TArg1, TArg2, TReturn> staticFunc, TRole1 role1)
        {
            return (arg1, arg2) => staticFunc(role1, arg1, arg2);
        }


        private static Func<TReturn> GetFunc<TRole1, TRole2, TRole3, TRole4, TReturn>(
            Func<TRole1, TRole2, TRole3, TRole4, TReturn> staticFunc, TRole1 role1, TRole2 role2, TRole3 role3,
            TRole4 role4)
        {
            return () => staticFunc(role1, role2, role3, role4);
        }

        private static Func<TArg, TReturn> GetFunc<TRole1, TRole2, TRole3, TArg, TReturn>(
            Func<TRole1, TRole2, TRole3, TArg, TReturn> staticFunc, TRole1 role1, TRole2 role2, TRole3 role3)
        {
            return (arg) => staticFunc(role1, role2, role3, arg);
        }

        private static Func<TArg1, TArg2, TReturn> GetFunc<TRole1, TRole2, TArg1, TArg2, TReturn>(
            Func<TRole1, TRole2, TArg1, TArg2, TReturn> staticFunc, TRole1 role1, TRole2 role2)
        {
            return (arg1, arg2) => staticFunc(role1, role2, arg1, arg2);
        }

        private static Func<TArg1, TArg2, TArg3, TReturn> GetFunc<TRole1, TArg1, TArg2, TArg3, TReturn>(
            Func<TRole1, TArg1, TArg2, TArg3, TReturn> staticFunc, TRole1 role1)
        {
            return (arg1, arg2, arg3) => staticFunc(role1, arg1, arg2, arg3);
        }


        private static Func<TReturn> GetFunc<TRole1, TRole2, TRole3, TRole4, TRole5, TReturn>(
            Func<TRole1, TRole2, TRole3, TRole4, TRole5, TReturn> staticFunc, TRole1 role1, TRole2 role2, TRole3 role3,
            TRole4 role4, TRole5 role5)
        {
            return () => staticFunc(role1, role2, role3, role4, role5);
        }

        private static Func<TArg, TReturn> GetFunc<TRole1, TRole2, TRole3, TRole4, TArg, TReturn>(
            Func<TRole1, TRole2, TRole3, TRole4, TArg, TReturn> staticFunc, TRole1 role1, TRole2 role2, TRole3 role3,
            TRole4 role4)
        {
            return (arg) => staticFunc(role1, role2, role3, role4, arg);
        }

        private static Func<TArg1, TArg2, TReturn> GetFunc<TRole1, TRole2, TRole3, TArg1, TArg2, TReturn>(
            Func<TRole1, TRole2, TRole3, TArg1, TArg2, TReturn> staticFunc, TRole1 role1, TRole2 role2, TRole3 role3)
        {
            return (arg1, arg2) => staticFunc(role1, role2, role3, arg1, arg2);
        }

        private static Func<TArg1, TArg2, TArg3, TReturn> GetFunc<TRole1, TRole2, TArg1, TArg2, TArg3, TReturn>(
            Func<TRole1, TRole2, TArg1, TArg2, TArg3, TReturn> staticFunc, TRole1 role1, TRole2 role2)
        {
            return (arg1, arg2, arg3) => staticFunc(role1, role2, arg1, arg2, arg3);
        }

        private static Func<TArg1, TArg2, TArg3, TArg4, TReturn> GetFunc<TRole1, TArg1, TArg2, TArg3, TArg4, TReturn>(
            Func<TRole1, TArg1, TArg2, TArg3, TArg4, TReturn> staticFunc, TRole1 role1)
        {
            return (arg1, arg2, arg3, arg4) => staticFunc(role1, arg1, arg2, arg3, arg4);
        }


        private static Type GetFuncType(Type[] args, bool isFunc)
        {
            int count = args.Length + (isFunc ? 0 : 1);
            Type funcType;
            switch (count)
            {
                case 1:
                    funcType = isFunc
                                   ? typeof (Func<>).MakeGenericType(args)
                                   : typeof (Action);
                    break;
                case 2:
                    funcType = (isFunc
                                    ? typeof (Func<,>)
                                    : typeof (Action<>)).MakeGenericType(args);
                    break;
                case 3:
                    funcType = (isFunc
                                    ? typeof (Func<,,>)
                                    : typeof (Action<,>)).MakeGenericType(args);
                    break;
                case 4:
                    funcType = (isFunc
                                    ? typeof (Func<,,,>)
                                    : typeof (Action<,,>)).MakeGenericType(args);
                    break;
                case 5:
                    funcType = (isFunc
                                    ? typeof (Func<,,,,>)
                                    : typeof (Action<,,,>)).MakeGenericType(args);
                    break;
                case 6:
                    funcType = (isFunc
                                    ? typeof (Func<,,,,,>)
                                    : typeof (Action<,,,,>)).MakeGenericType(args);
                    break;
                case 7:
                    funcType = (isFunc
                                    ? typeof (Func<,,,,,,>)
                                    : typeof (Action<,,,,,>)).MakeGenericType(args);
                    break;
                case 8:
                    funcType = (isFunc
                                    ? typeof (Func<,,,,,,,>)
                                    : typeof (Action<,,,,,,>)).MakeGenericType(args);
                    break;
                case 9:
                    funcType = (isFunc
                                    ? typeof (Func<,,,,,,,,>)
                                    : typeof (Action<,,,,,,,>)).MakeGenericType(args);
                    break;
                case 10:
                    funcType = (isFunc
                                    ? typeof (Func<,,,,,,,,,>)
                                    : typeof (Action<,,,,,,,,>)).MakeGenericType(args);
                    break;
                default:
                    throw new InvalidOperationException("Number of arguments not supported");
            }
            return funcType;
        }

        private static bool IsFunc(MethodInfo m)
        {
            return m.ReturnType != typeof (void);
        }


        private static dynamic GetRealFunc(dynamic functor, IEnumerable<ParameterInfo> parameters,
                                           IDictionary<string, dynamic> self)
        {
            string[] actors = parameters.TakeWhile(IsActor).Select(p => p.Name).ToArray();
            int actorCount = actors.Length;
            switch (actorCount)
            {
                case 0:
                    return functor;
                case 1:
                    return GetFunc(functor, self[actors[0]]);
                case 2:
                    return GetFunc(functor, self[actors[0]], self[actors[1]]);
                case 3:
                    return GetFunc(functor, self[actors[0]], self[actors[1]], self[actors[2]]);
                case 4:
                    return GetFunc(functor, self[actors[0]], self[actors[1]], self[actors[2]], self[actors[3]]);
                case 5:
                    return GetFunc(functor, self[actors[0]], self[actors[1]], self[actors[2]], self[actors[3]],
                                   self[actors[4]]);
                default:
                    throw new NotSupportedException("Only uptil 2 roles supported");
            }
        }

        private static bool IsActor(ParameterInfo p)
        {
            return p.GetCustomAttributes(typeof (RoleAttribute), false).SingleOrDefault() != null;
        }

        private static T Cast<T>(dynamic obj)
        {
            return (T) obj;
        }

        #endregion

        protected static void Bind(TContext context, object roleMap)
        {
            Type definition = typeof (TContext).GetNestedType("RoleMethods", BindingFlags.NonPublic);
            Dictionary<string, Type> roleTypes =
                typeof (TContext).GetNestedTypes().Select(
                    t =>
                    new
                        {
                            Type = t,
                            Attribute =
                        (RoleAttribute) t.GetCustomAttributes(typeof (RoleAttribute), false).SingleOrDefault()
                        }).Where(
                            t => t.Attribute != null).ToDictionary(t => t.Attribute.Name, t => t.Type);
            if (definition == null)
            {
                throw new InvalidOperationException("Must have an inner type called RoleMethods");
            }

            MethodInfo[] methods = definition.GetMethods(BindingFlags.Static | BindingFlags.Public);

            var roles = new HashSet<string>(from p in methods.SelectMany(m => m.GetParameters())
                                            where
                                                IsActor(p)
                                            select p.Name);
            IEnumerable<PropertyInfo> properties = roleMap == null
                                                       ? Enumerable.Empty<PropertyInfo>()
                                                       : roleMap.GetType().GetProperties();
            IDictionary<string, dynamic> self = new Dictionary<string, dynamic>();
            foreach (PropertyInfo role in properties)
            {
                var roleName = role.Name;
                roles.Remove(roleName);
                self.Add(role.Name, role.GetValue(roleMap, null));
            }
            if (roles.Count != 0)
            {
                throw new InvalidOperationException("Not all roles were bound");
            }

            foreach (MethodInfo m in methods)
            {
                ParameterInfo[] parameters = m.GetParameters();
                ParameterInfo injectee = parameters[0];
                dynamic role = New.Injectee();
                role[m.Name] = GetRealFunc(GetFunc(m), m.GetParameters(), self);
                var fieldName = injectee.Name;
                var cast = ((MethodCallExpression) ((Expression<Func<object>>) (() => Cast<object>(null))).Body).Method.GetGenericMethodDefinition();
                cast = cast.MakeGenericMethod(roleTypes[fieldName]);
                object value = cast.Invoke(null,new object[]{role});
                SetRoleField(fieldName, context, value);
            }
            var props = self.ToArray();
            foreach (var kp in props)
            {
                if (!roleTypes.ContainsKey(kp.Key))
                {
                    SetRoleField(kp.Key, context, kp.Value);
                }
                self.Remove(kp.Key);
            }
            Contract.Assert(self.Count == 0);
        }

        private static void SetRoleField(string fieldName, TContext context, object value)
        {
            FieldInfo roleField = context.GetType().GetField(fieldName,
                                                             BindingFlags.NonPublic | BindingFlags.Instance);
            if (roleField != null)
            {
                roleField.SetValue(context, value);
            }
        }
    }
}