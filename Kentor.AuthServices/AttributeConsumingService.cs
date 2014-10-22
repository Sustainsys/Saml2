using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices
{
    public class AttributeConsumingService
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">Name of the attribute consuming service.</param>
        public AttributeConsumingService(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The name of the attribute consuming service.
        /// </summary>
        public string Name { get; set; }

        readonly ICollection<RequestedAttribute> requestedAttributes = new List<RequestedAttribute>();

        /// <summary>
        /// Requested attributes.
        /// </summary>
        public ICollection<RequestedAttribute> RequestedAttributes
        {
            get
            {
                return requestedAttributes;
            }
        }
    }
}
