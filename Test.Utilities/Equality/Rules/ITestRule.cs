using System.Collections.Generic;

namespace Test.Utilities.Equality.Rules {
    internal interface ITestRule {
        IEnumerable<string> GetErrorMessages();
    }
}