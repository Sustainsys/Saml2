using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sustainsys.Saml2.Metadata.Tokens
{
    public class SecurityKeyIdentifier : IEnumerable<SecurityKeyIdentifierClause>
    {
        private readonly List<SecurityKeyIdentifierClause> clauses;

        public void Add(SecurityKeyIdentifierClause clause)
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("SecurityKeyIdentifier is read only");
            }
            if (clause == null)
            {
                throw new ArgumentNullException(nameof(clause));
            }
            clauses.Add(clause);
        }

        public bool CanCreateKey
        {
            get => clauses.Exists(clause => clause.CanCreateKey);
        }

        public int Count
        {
            get => clauses.Count;
        }

        public SecurityKeyIdentifierClause this[int index]
        {
            get
            {
                if (index < 0 || index >= clauses.Count)
                {
                    throw new ArgumentOutOfRangeException("Invalid index");
                }
                return clauses[index];
            }
        }

        public IEnumerator<SecurityKeyIdentifierClause> GetEnumerator()
        {
            return clauses.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return clauses.GetEnumerator();
        }

        public bool IsReadOnly { get; private set; }

        public void MakeReadOnly()
        {
            IsReadOnly = true;
        }

        public SecurityKey CreateKey()
        {
            var clause = clauses.FirstOrDefault(x => x.CanCreateKey);
            if (clause == null)
            {
                throw new NotSupportedException("SecurityKeyIdentifier does not support key creation");
            }
            return clause.CreateKey();
        }

        public bool TryFind<TClause>(out TClause clause) where TClause : SecurityKeyIdentifierClause
        {
            clause = (TClause)clauses.FirstOrDefault(x => x is TClause);
            return clause != null;
        }

        public TClause Find<TClause>() where TClause : SecurityKeyIdentifierClause
        {
            TClause clause;
            if (!TryFind(out clause))
            {
                throw new InvalidOperationException($"A clause of type ${typeof(TClause).Name} could not be found");
            }
            return clause;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("SecurityKeyIdentifier(IsReadOnly = ");
            sb.Append(IsReadOnly);
            sb.Append(", Count = ");
            sb.Append(Count);

            sb.Append(", Clauses = [");
            for (int i = 0; i < clauses.Count; ++i)
            {
                if (i != 0)
                {
                    sb.Append(", ");
                }
                sb.Append((object)clauses[i]);
            }
            sb.Append("])");

            return sb.ToString();
        }

        public SecurityKeyIdentifier()
        {
            clauses = new List<SecurityKeyIdentifierClause>();
        }

        public SecurityKeyIdentifier(params SecurityKeyIdentifierClause[] clauses)
        {
            if (clauses == null)
            {
                throw new ArgumentNullException(nameof(clauses));
            }
            this.clauses = new List<SecurityKeyIdentifierClause>(clauses.Length);
            foreach (var clause in clauses)
            {
                Add(clause);
            }
        }
    }
}