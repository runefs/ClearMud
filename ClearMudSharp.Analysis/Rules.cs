using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClearMudSharp.Analysis
{
    public class Rules : BaseRule
    {
        public Rules()
            : base("Rules")
        {
        }

        public override ProblemCollection Check(TypeNode type)
        {
            if (!type.Name.Name.EndsWith("Foo", StringComparison.Ordinal))
            {
                var resolution = GetResolution(type.Name.Name);
                var problem = new Problem(resolution, type)
                {
                    Certainty = 100,
                    FixCategory = FixCategories.Breaking,
                    MessageLevel = MessageLevel.Warning
                };
                Problems.Add(problem);
            }

            return Problems;
        }

        public ProblemCollection CheckX(TypeNode type)
        {
            if (type.Name.Name.EndsWith("Context"))
            {
                var roleContracts = from t in type.NestedTypes
                                    let roleAttributes = t.Attributes.Where(a => a.Type.Name.Name == "RoleAttribute")
                                    where roleAttributes.Any()
                                    from roleContract in roleAttributes
                                    select t;

                var roleMethods = type.NestedTypes.Where(t => t.Name.Name == "RoleMethod").Single();
                foreach (Method method in roleMethods.Members.Where(m => m.NodeType == NodeType.Method))
                {
                    var parameterTypes =
                        method.Parameters.SkipWhile(p => p.Attributes.Any(a => a.Type.Name.Name == "RoleAttribute")).Select(
                            p => p.Type).ToArray();
                    var actors = method.Parameters.TakeWhile(p => p.Attributes.Any(a => a.Type.Name.Name == "RoleAttribute")).Select(
                            p => p.Name.Name);
                    var contract = roleContracts.SingleOrDefault(c => actors.Contains("I" + c.Name.Name));
                    if (contract == null)
                    {
                        var resolution = GetResolution("No contract defined");
                        var problem = new Problem(resolution, type)
                        {
                            Certainty = 100,
                            FixCategory = FixCategories.Breaking,
                            MessageLevel = MessageLevel.Warning
                        };
                        Problems.Add(problem);
                    }

                    var contractMethod = contract.Members.Where(m => m.NodeType == NodeType.Property && m.Name.Name == method.Name.Name);
                }

            }

            if (!type.Name.Name.EndsWith("Foo", StringComparison.Ordinal))
            {

            }

            return Problems;
        }
    }
}
