namespace Test.Utilities.Equality.Rules {
    internal class OverridesGetHashCode<T> : ImplementsMethod<T> {
        public OverridesGetHashCode() : base("GetHashCode") { }
    }
}