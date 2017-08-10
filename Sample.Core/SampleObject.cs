using System;

namespace Sample.Core {
    public sealed class SampleObject : ISampleObject, IEquatable<SampleObject> {
        private enum StatusRepresentation {
            None,
            Some,
            This,
            That
        }

        public static ISampleObject As() => new SampleObject(StatusRepresentation.None);

        public static bool operator ==(SampleObject a, SampleObject b) {
            if (ReferenceEquals(a, null))
                return ReferenceEquals(b, null);
            return a.Equals(b);
        }

        public static bool operator !=(SampleObject a, SampleObject b) => !(a == b);

        private StatusRepresentation Representation { get; }

        private SampleObject(StatusRepresentation represenation) => Representation = represenation;

        public bool Equals(SampleObject other) {
            if (ReferenceEquals(null, other))
                return false;
            return ReferenceEquals(this, other) || Representation.Equals(other.Representation);
        }

        public SampleObject NoSample() => new SampleObject(StatusRepresentation.None);
        public SampleObject SomeSample() => new SampleObject(StatusRepresentation.Some);
        public SampleObject ThisSample() => new SampleObject(StatusRepresentation.This);
        public SampleObject ThatSample() => new SampleObject(StatusRepresentation.That);

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;
            return ReferenceEquals(this, obj) || Equals((SampleObject) obj);
        }

        public override int GetHashCode() => Representation.GetHashCode();
    }
}