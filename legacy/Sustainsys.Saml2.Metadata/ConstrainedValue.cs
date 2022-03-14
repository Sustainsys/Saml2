using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;

namespace Sustainsys.Saml2.Metadata
{
    public class ClaimValue
    {
        public string Value { get; set; }
        public ICollection<XmlElement> StructuredValue { get; set; }
    }

    public class ConstrainedValue
    {
        public abstract class Constraint
        {
        }

        public class CompareConstraint : Constraint
        {
            public enum CompareOperator
            {
                Lt,
                Lte,
                Gt,
                Gte,
            }

            public CompareOperator CompareOp { get; private set; }
            public ClaimValue Value { get; set; } = new ClaimValue();

            public CompareConstraint(CompareOperator compareOp)
            {
                CompareOp = compareOp;
            }
        }

        public class RangeConstraint : Constraint
        {
            public ClaimValue LowerBound { get; set; } = new ClaimValue();
            public ClaimValue UpperBound { get; set; } = new ClaimValue();
        }

        public class ListConstraint : Constraint
        {
            public ICollection<ClaimValue> Values { get; private set; } =
                new Collection<ClaimValue>();
        }

        public bool AssertConstraint { get; set; }

        public ICollection<Constraint> Constraints { get; private set; } =
            new Collection<Constraint>();
    }
}