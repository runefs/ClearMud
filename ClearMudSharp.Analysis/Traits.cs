using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.FxCop.Sdk;
using System.Diagnostics;
using Microsoft.VisualStudio.CodeAnalysis.Extensibility;

public class InterfaceMethodsMustBeImplemented : BaseRule
{
    public InterfaceMethodsMustBeImplemented() : base("InterfaceMethodsMustBeImplemented") { }
    public override Microsoft.FxCop.Sdk.ProblemCollection Check(Microsoft.FxCop.Sdk.TypeNode type)
    {
        var inters = type.Interfaces ?? Enumerable.Empty<TypeNode>();
        var interfaces = from i in type.Interfaces
                         let attributes = i.Attributes
                         where attributes.Where(a => a.Type.FullName == "ClearMudSharp.Traits.TraitAttribute").Any()
                         select i;

        foreach (var inter in interfaces)
        {
            if (inter.TemplateArguments.Count < 1)
            {
                AddProblem(inter.Name.Name, "<No type argument>", string.Empty,type);
            }
            else
            {
                var staticClass = inter.TemplateArguments.First();
                var methodImplementations = staticClass.Members.Where(m => m.NodeType == NodeType.Method).ToArray();
                var methodDeclarations = inter.Members.Where(m => m.NodeType == NodeType.Method).ToArray();

                var implementedMethods = from method in methodImplementations.Cast<Method>()
                                         let args = method.Parameters.Skip(1).Select(p => p.Type).ToArray()
                                         where inter.GetMethod(Identifier.For(method.Name.Name), args) != null
                                         select method.Name.Name;
                if (implementedMethods.Count() < methodDeclarations.Length)
                {
                    var missingMethods = methodDeclarations.Select(m => m.Name.Name).Except(implementedMethods);
                    foreach (var missingMethod in missingMethods)
                    {
                        AddProblem(inter.FullName, staticClass.FullName, missingMethod + " not implemented",type);
                    }
                }
            }
        }
        return Problems;
    }

    private void AddProblem(string interfaceName, string typeName, string message, Microsoft.FxCop.Sdk.TypeNode type)
    {
        var resolutionx = GetResolution(interfaceName,typeName,message);
        var problemx = new Problem(resolutionx, type)
        {
            Certainty = 100,
            FixCategory = FixCategories.Breaking,
            MessageLevel = MessageLevel.Warning
        };
        Problems.Add(problemx);
    }
}

