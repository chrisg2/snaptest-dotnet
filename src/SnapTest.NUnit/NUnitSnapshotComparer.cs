using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SnapTest.NUnit
{
    internal class NUnitSnapshotComparer : SnapshotComparer
    {
        public ConstraintResult ConstraintResult;
        private IConstraint _constraint;

        public NUnitSnapshotComparer(IConstraint constraint)
        {
            _constraint = constraint;
        }

        public override bool Compare(JToken actualValue, JToken snapshottedValue)
        {
            bool result;
            if (snapshottedValue == null) {
                ConstraintResult = new ConstraintResult(_constraint, $"No snapshotted value available", false);
                result = false;
            } else {
                result = base.Compare(actualValue, snapshottedValue);

                if (result)
                    ConstraintResult = new ConstraintResult(_constraint, actualValue, true);
                else {
                    // Check result using an NUnit Is.EqualTo constraint.
                    // The constraint result is recorded in the context to be returned later by ApplyTo.

                    // Serialize object properties in sorted order to ensure equivalence
                    var jsonSettings = new JsonSerializerSettings() {
                        ContractResolver = new OrderedContractResolver(),
                        Formatting = Formatting.None
                    };

                    var expectedJson = JsonConvert.SerializeObject(snapshottedValue, Formatting.None, jsonSettings);
                    var actualJson = JsonConvert.SerializeObject(actualValue, Formatting.None, jsonSettings);

                    ConstraintResult = Is.EqualTo(expectedJson).ApplyTo(actualJson);
                }
            }

            return ConstraintResult.Status == ConstraintStatus.Success;
        }

        private class OrderedContractResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
                => base.CreateProperties(type, memberSerialization).OrderBy(p => p.PropertyName).ToList();
        }
    }
}
