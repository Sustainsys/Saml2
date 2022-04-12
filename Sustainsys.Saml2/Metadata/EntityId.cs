using System;

namespace Sustainsys.Saml2.Metadata
{
    public class EntityId
    {
		public string Id { get; set; }
        public string NameQualifier { get; set; }
        public string Format { get; set; }

        public EntityId(string id)
		{
			Id = id;
		}

		public EntityId() :
			this(null)
		{
		}
	}
}
