using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastConfig
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class InnerAttribute : Attribute
    {
    }
}
