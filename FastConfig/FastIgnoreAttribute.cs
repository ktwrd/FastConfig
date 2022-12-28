using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kate.FastConfig
{
    /// <summary>
    /// Ignore serialization/deserialization of fields/properties
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class FastIgnoreAttribute : Attribute
    {
    }
}
