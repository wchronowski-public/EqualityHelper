using Test.Utilities;
using Xunit;

namespace Sample.Core.Tests {
    public class SampleObjectTest {
        [Fact]
        public void Equalalty_Tests() {
            var someSample1 = BuildSomeSampleObject();
            var someSample2 = BuildSomeSampleObject();
            var thisSample = BuildThisSampleObject();
            var thatSample = BuildThatSampleObject();
            var noSample = BuildNoSampleObject();

            EqualityTests
                .For(someSample1)
                .EqualTo(someSample1)
                .EqualTo(someSample2)
                .NotEqualTo(thisSample, "different sample (some to this)")
                .NotEqualTo(thatSample, "different sample (some to that)")
                .NotEqualTo(noSample, "different sample (some to none)")
                .Assert();
        }

        private static SampleObject BuildNoSampleObject() => SampleObject.As().NoSample();
        private static SampleObject BuildSomeSampleObject() => SampleObject.As().SomeSample();
        private static SampleObject BuildThisSampleObject() => SampleObject.As().ThisSample();
        private static SampleObject BuildThatSampleObject() => SampleObject.As().ThatSample();
    }
}