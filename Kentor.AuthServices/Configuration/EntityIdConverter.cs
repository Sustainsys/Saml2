using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Converts between string and EntityId, used by the configuration system to
    /// allow configuration properties of type EntityId.
    /// </summary>
    public class EntityIdConverter : ConfigurationConverterBase
    {
        /// <summary>
        /// Converts a string to an EntityId
        /// </summary>
        /// <param name="context">Ignored</param>
        /// <param name="culture">Ignored</param>
        /// <param name="value">String to convert</param>
        /// <returns>EntityID</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return new EntityId((string)value);
        }
    }
}
