using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastConfig
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class EntryAttribute : Attribute
    {
        public object DefaultValue;
        public string Group;
        public string Key;
        public EntryAttribute(string group = null, string key = null, object defaultValue=null)
        {
            DefaultValue = defaultValue;
            Group = group;
            Key = key;
        }
    }
}
