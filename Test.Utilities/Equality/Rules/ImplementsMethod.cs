using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Test.Utilities.Equality.Rules {
    internal abstract class ImplementsMethod<T> : ITestRule {
        private string MethodName { get; }
        private string MethodLabel { get; }
        private Type[] ArgumentTypes { get; }
        private Action DiscoverTargetMehtod { get; set; }

        private string OverrideLabel {
            get {
                if (TryGetTargetMethod().Any(m => m.GetBaseDefinition() != null))
                    return "override";
                return "overload";
            }
        }

        private string MethodSignature => $"{MethodLabel}({string.Join(", ", ArgumentTypes.Select(type => type.Name))}";

        private IEnumerable<MethodInfo> targetMethod;

        protected ImplementsMethod(string methodName, params Type[] argumentTypes) : this(methodName, methodName, argumentTypes) { }

        protected ImplementsMethod(string methodName, string methodLabel, params Type[] argumentTypes) {
            MethodName = methodName;
            MethodLabel = methodLabel;
            ArgumentTypes = argumentTypes;

            DiscoverTargetMehtod = () => {
                var method = typeof(T).GetMethod(MethodName, ArgumentTypes);
                if (method == null)
                    targetMethod = Enumerable.Empty<MethodInfo>();
                else
                    targetMethod = new[] {
                        method
                    };

                DiscoverTargetMehtod = () => { };
            };
        }

        public IEnumerable<string> GetErrorMessages() {
            if (TryGetTargetMethod().All(method => method.DeclaringType != typeof(T)))
                yield return $"{typeof(T).Name} should {OverrideLabel} {MethodSignature}).";
        }

        public IEnumerable<MethodInfo> TryGetTargetMethod() {
            DiscoverTargetMehtod();
            return targetMethod;
        }
    }
}