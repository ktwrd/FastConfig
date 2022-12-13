using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastConfig
{
    /// <summary>
    /// The class that this attribute is assigned to must have a constructor that takes no arguments.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ConfigSerializeAttribute : Attribute
    {
    }
}
