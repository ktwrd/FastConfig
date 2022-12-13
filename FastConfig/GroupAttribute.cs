using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastConfig
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, Inherited = true)]
    public class GroupAttribute : Attribute
    {
        public string Group;
        public GroupAttribute(string group)
        {
            Group = group;
        }
    }
}
