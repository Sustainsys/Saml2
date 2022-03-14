namespace Sustainsys.Saml2.Metadata
{
    public class EntityId
    {
        public string Id { get; set; }

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