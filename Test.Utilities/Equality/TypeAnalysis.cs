﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Test.Utilities.Equality.Rules;

namespace Test.Utilities.Equality {
    internal class TypeAnalysis<T> {
        public static TypeAnalysis<T> Analyze() {
            var analysis = new TypeAnalysis<T>();

            AnalyzeEquals(analysis);
            AnalyzeGetHashCode(analysis);
            AnalyzeEqualityOperator(analysis);
            AnalyzeInequalityOperator(analysis);
            AnalyzeEquatable(analysis);
            AnalyzeSealed(analysis);

            return analysis;
        }

        public IEnumerable<ITestRule> TypeLevelRules => Rules;
        private IList<ITestRule> Rules { get; } = new List<ITestRule>();

        private IEnumerable<MethodInfo> EqualsMethod { get; set; }
        private IEnumerable<MethodInfo> StrongEqualsMethod { get; set; }
        private IEnumerable<MethodInfo> GetHashCodeMethod { get; set; }
        private IEnumerable<MethodInfo> EqualityOperator { get; set; }
        private IEnumerable<MethodInfo> InequalityOperator { get; set; }

        public IEnumerable<ITestRule> GetEqualToRules(T instance, T other) {
            var testCase = "equal objects";
            return GetEqualsReturns(instance, other, true, testCase).Concat(GetEqualityOperatorReturns(instance, other, true, testCase)).
                                                                     Concat(GetInequalityOperatorReturns(instance, other, false, testCase)).
                                                                     Concat(GetStrongEqualsReturns(instance, other, true, testCase)).Concat(GetEqualGetHashCodeReturns(instance, other)).ToList();
        }

        public IEnumerable<ITestRule> GetNotEqualRules(T instance, T other, string testCase) =>
            GetEqualsReturns(instance, other, false, testCase).Concat(GetEqualityOperatorReturns(instance, other, false, testCase)).
                                                               Concat(GetInequalityOperatorReturns(instance, other, true, testCase)).Concat(GetStrongEqualsReturns(instance, other, false, testCase)).
                                                               ToList();

        public IEnumerable<ITestRule> GetEqualityOfTwoNulls() =>
            GetEqualityOperatorReturns(default(T), default(T), true, "equality of two nulls").Concat(GetInequalityOperatorReturns(default(T), default(T), false, "equality of two nulls")).ToList();

        private static void AnalyzeEquals(TypeAnalysis<T> analysis) {
            ImplementsMethod<T> rule = new OverridesEquals<T>();
            analysis.EqualsMethod = rule.TryGetTargetMethod();
            analysis.Rules.Add(rule);
        }

        private static void AnalyzeGetHashCode(TypeAnalysis<T> analysis) {
            ImplementsMethod<T> rule = new OverridesGetHashCode<T>();
            analysis.GetHashCodeMethod = rule.TryGetTargetMethod();
            analysis.Rules.Add(rule);
        }

        private static void AnalyzeEqualityOperator(TypeAnalysis<T> analysis) {
            ImplementsMethod<T> rule = new OverloadsEqualityOperator<T>();
            analysis.EqualityOperator = rule.TryGetTargetMethod();
            analysis.Rules.Add(rule);
        }

        private static void AnalyzeInequalityOperator(TypeAnalysis<T> analysis) {
            ImplementsMethod<T> rule = new OverloadsInequalityOperator<T>();
            analysis.InequalityOperator = rule.TryGetTargetMethod();
            analysis.Rules.Add(rule);
        }

        private static void AnalyzeEquatable(TypeAnalysis<T> analysis) {
            var rule = new ImplementsIEquatable<T>();
            analysis.StrongEqualsMethod = rule.TryGetTargetMethod();
            analysis.Rules.Add(rule);
        }

        private static void AnalyzeSealed(TypeAnalysis<T> analysis) {
            analysis.Rules.Add(new IsTypeSealed<T>());
        }

        private IEnumerable<ITestRule> GetEqualsReturns(T instance, T other, bool result, string testCase) => EqualsMethod.Select(
            method => MethodReturns<T, bool>.InstanceMethod(method, instance, result, testCase, other));

        private IEnumerable<ITestRule> GetEqualityOperatorReturns(T instance, T other, bool result, string testCase) => EqualityOperator.Select(
            method => MethodReturns<T, bool>.Operator(method, "operator ==", instance, other, result, testCase));

        private IEnumerable<ITestRule> GetInequalityOperatorReturns(T instance, T other, bool result, string testCase) => InequalityOperator.Select(
            method => MethodReturns<T, bool>.Operator(method, "operator !=", instance, other, result, testCase));

        private IEnumerable<ITestRule> GetStrongEqualsReturns(T instance, T other, bool result, string testCase) => StrongEqualsMethod.Select(
            method => MethodReturns<T, bool>.InstanceMethod(method, instance, result, testCase, other));

        private IEnumerable<ITestRule> GetEqualGetHashCodeReturns(T instance, T other) => GetHashCodeMethod.Select(method => new GetHashCodeEqualReturns<T>(method, instance, other));
    }
}