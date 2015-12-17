using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Converts between string and IPendingAuthStorageContainer, used by the configuration system to
    /// allow configuration properties of type IPendingAuthStorageContainer.
    /// </summary>
    public class PendingAuthStorageContainerConverter : ConfigurationConverterBase
    {
        /// <summary>
        /// Converts a string to an instance of a IPendingAuthStorageContainer
        /// </summary>
        /// <param name="context">Ignored</param>
        /// <param name="culture">Ignored</param>
        /// <param name="value">String to convert</param>
        /// <returns>IPendingAuthStorageContainer</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var typeAndAssembly = (string)value;
            if (!string.IsNullOrWhiteSpace(typeAndAssembly)) { 
                var parts = typeAndAssembly.Split(',');
                var typeName = parts[0];
                var assemblyName = parts[1];
                return (IPendingAuthStorageContainer)Activator.CreateInstance(assemblyName, typeName).Unwrap();
            }
            return null;
        }
    }
}
