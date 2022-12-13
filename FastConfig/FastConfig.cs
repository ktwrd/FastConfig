using Nini.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FastConfig
{
    public class FastConfigSource<T> where T : new()
    {
        internal IniConfigSource Source;
        public FastConfigSource(string location)
            : this(Encoding.UTF8.GetBytes(File.ReadAllText(location)))
        {}
        public FastConfigSource(byte[] byteArray)
            : this(new MemoryStream(byteArray))
        {}
        public FastConfigSource(Stream content)
        {
            Source = new IniConfigSource(content);
            var gotten = GetDict();
        }
        public T Parse()
        {
            T instance = new();
            var type = typeof(T);
            ParseChildren(type, instance, null);
            return instance;
        }
        internal void ParseChildren(Type type, object instance, string defaultGroup)
        {
            foreach (var attr in Attribute.GetCustomAttributes(instance.GetType()))
                if (attr is GroupAttribute)
                    defaultGroup = ((GroupAttribute)attr).Group;
            foreach (var field in type.GetFields())
            {
                if (Attribute.GetCustomAttribute(field.FieldType, typeof(InnerAttribute)) != null)
                    ParseChildren(field.FieldType, field.GetValue(instance) ?? Activator.CreateInstance(field.FieldType), defaultGroup);
                else
                    field.SetValue(instance, GetConfigMemberValue(field, field.FieldType, instance, defaultGroup, field.Name, field.GetValue(instance)));
            }
            foreach (var prop in type.GetProperties())
            {
                if (Attribute.GetCustomAttribute(prop.PropertyType, typeof(InnerAttribute)) != null)
                    ParseChildren(prop.PropertyType, prop.GetValue(instance) ?? Activator.CreateInstance(prop.PropertyType), defaultGroup);
                else
                {
                    prop.SetValue(instance, GetConfigMemberValue(prop, prop.PropertyType, instance, defaultGroup, prop.Name, prop.GetValue(instance)));
                }
            }

        }
        private Dictionary<string, Dictionary<string, object>> ToDictionary_Logic(Dictionary<string, Dictionary<string, object>> dict, string group, string key, object value, object workingInstance)
        {
            object currentInstance = workingInstance;
            foreach (var attr in Attribute.GetCustomAttributes(workingInstance.GetType()))
                if (attr is GroupAttribute)
                    group = ((GroupAttribute)attr).Group;
            foreach (var field in workingInstance.GetType().GetFields())
            {
                foreach (var attr in Attribute.GetCustomAttributes(field))
                {
                    workingInstance = currentInstance;
                    if (attr is GroupAttribute)
                    {
                        group = ((GroupAttribute)attr).Group;
                    }
                    if (attr is EntryAttribute)
                    {
                        var entryAttr = (EntryAttribute)attr;
                        group = entryAttr.Group ?? group;
                        key = entryAttr.Key ?? field.Name;
                    }
                    key = field.Name ?? key;
                    if (!dict.ContainsKey(group))
                        dict.Add(group, new Dictionary<string, object>());
                    if (Attribute.GetCustomAttribute(field.FieldType, typeof(InnerAttribute)) == null)
                    {
                        dict[group][key] = field.GetValue(currentInstance);
                    }
                }
                if (Attribute.GetCustomAttribute(field.FieldType, typeof(InnerAttribute)) != null)
                {
                    var res = ToDictionary_Logic(new Dictionary<string, Dictionary<string, object>>(), group, key, value, field.GetValue(currentInstance) ?? Activator.CreateInstance(field.FieldType));
                    dict = dict.Concat(res.AsEnumerable()).ToDictionary(v => v.Key, v => v.Value);
                }
                else
                {
                    key = field.Name ?? key;
                    if (!dict.ContainsKey(group))
                        dict.Add(group, new Dictionary<string, object>());
                    if (Attribute.GetCustomAttribute(field.FieldType, typeof(InnerAttribute)) == null)
                    {
                        dict[group][key] = field.GetValue(currentInstance);
                    }
                }
            }
            foreach (var prop in workingInstance.GetType().GetProperties())
            {
                foreach (var attr in Attribute.GetCustomAttributes(prop))
                {
                    workingInstance = currentInstance;
                    if (attr is GroupAttribute)
                    {
                        group = ((GroupAttribute)attr).Group;
                    }
                    if (attr is EntryAttribute)
                    {
                        var entryAttr = (EntryAttribute)attr;
                        group = entryAttr.Group ?? group;
                    }
                    key = prop.Name ?? key;
                    if (!dict.ContainsKey(group))
                        dict.Add(group, new Dictionary<string, object>());
                    if (Attribute.GetCustomAttribute(prop.PropertyType, typeof(InnerAttribute)) == null)
                    {
                        dict[group][key] = prop.GetValue(currentInstance);
                    }
                }
                if (Attribute.GetCustomAttribute(prop.PropertyType, typeof(InnerAttribute)) != null)
                {
                    var res = ToDictionary_Logic(new Dictionary<string, Dictionary<string, object>>(), group, key, value, prop.GetValue(currentInstance) ?? Activator.CreateInstance(prop.PropertyType));
                    dict = dict.Concat(res.AsEnumerable()).ToDictionary(v => v.Key, v => v.Value);
                }
                else
                {
                    key = prop.Name ?? key;
                    if (!dict.ContainsKey(group))
                        dict.Add(group, new Dictionary<string, object>());
                    if (Attribute.GetCustomAttribute(prop.PropertyType, typeof(InnerAttribute)) == null)
                    {
                        dict[group][key] = prop.GetValue(currentInstance);
                    }
                }
            }
            return dict;
        }
        public Dictionary<string, Dictionary<string, object>> ToDictionary(T instance)
        {
            var dict = new Dictionary<string, Dictionary<string, object>>();
            return ToDictionary_Logic(dict, "", "", null, instance);
        }
        private class EntryInfo
        {
            internal Type EntryType { get; set; }
            internal object EntryParent { get; set; }
        }
        private Dictionary<string, Dictionary<string, EntryInfo>> InternalTree = new Dictionary<string, Dictionary<string, EntryInfo>>();
        public Dictionary<Type, Func<string, object>> Parser = new Dictionary<Type, Func<string, object>>();
        internal object GetConfigMemberValue(MemberInfo member, Type memberType, object parent, string defaultGroup = null, string defaultKey = null, object defaultValue = null)
        {
            string group = defaultGroup;
            string key = defaultKey;
            object value = defaultValue;
            foreach (var attr in Attribute.GetCustomAttributes(member))
            {
                if (attr is EntryAttribute)
                {
                    var entryAttr = (EntryAttribute)attr;
                    group = entryAttr.Group ?? defaultGroup;
                    key = entryAttr.Key ?? defaultKey;
                    value = entryAttr.DefaultValue ?? defaultValue;
                    defaultValue = entryAttr.DefaultValue ?? defaultValue;
                }
                if (attr is GroupAttribute)
                {
                    group = ((GroupAttribute)attr).Group;
                }
            }
            if (memberType == typeof(string))
                value = GetDictString(group, key, defaultValue?.ToString() ?? defaultValue?.ToString() ?? "");
            else if (memberType == typeof(int))
                value = GetDictInt(group, key, int.Parse(defaultValue?.ToString() ?? defaultValue?.ToString() ?? ""));
            else if (memberType == typeof(long))
                value = GetDictLong(group, key, long.Parse(defaultValue?.ToString() ?? defaultValue?.ToString() ?? ""));
            else if (memberType == typeof(bool))
                value = GetDictBoolean(group, key, (defaultValue?.ToString() ?? defaultValue?.ToString() ?? "") == "true");
            else if (memberType == typeof(float))
                value = GetDictFloat(group, key, float.Parse(defaultValue?.ToString() ?? defaultValue?.ToString() ?? ""));
            else if (Parser.ContainsKey(memberType))
                value = Parser[memberType](GetDictString(group, key));
            if (!InternalTree.ContainsKey(group))
                InternalTree.Add(group, new Dictionary<string, EntryInfo>());
            InternalTree[group][key] = new EntryInfo()
            {
                EntryType = memberType,
                EntryParent = parent
            };
            return value;
        }

        public Dictionary<string, Dictionary<string, string>> GetDict()
        {
            var dict = new Dictionary<string, Dictionary<string, string>>();
            foreach (IConfig cfg in Source.Configs)
            {
                if (cfg == null) continue;
                dict.Add(cfg.Name, new Dictionary<string, string>());
                foreach (var key in cfg.GetKeys())
                {
                    var value = cfg.Get(key);
                    dict[cfg.Name].Add(key, value);
                }
            }
            return dict;
        }
        internal IConfig GetDict(string group)
        {
            var cfg = Source.Configs[group];
            if (cfg == null)
                cfg = Source.Configs.Add(group);
            return cfg;
        }
        internal void SetDict(string group, string key, object value)
        {
            var cfg = GetDict(group);
            cfg.Set(key, value);
        }

        public string GetDict(string group, string key) => GetDict(group).Get(key);
        public string GetDict(string group, string key, string fallback) => GetDict(group).Get(key, fallback);
        public string GetDictExpanded(string group, string key) => GetDict(group).GetExpanded(key);
        public string GetDictString(string group, string key) => GetDict(group).GetString(key);
        public string GetDictString(string group, string key, string fallback) => GetDict(group).GetString(key, fallback);
        public int GetDictInt(string group, string key) => GetDict(group).GetInt(key);
        public int GetDictInt(string group, string key, int fallback) => GetDict(group).GetInt(key, fallback);
        public int GetDictInt(string group, string key, int fallback, bool fromAlias) => GetDict(group).GetInt(key, fallback, fromAlias);
        public long GetDictLong(string group, string key) => GetDict(group).GetLong(key);
        public long GetDictLong(string group, string key, long fallback) => GetDict(group).GetLong(key, fallback);
        public bool GetDictBoolean(string group, string key) => GetDict(group).GetBoolean(key);
        public bool GetDictBoolean(string group, string key, bool fallback) => GetDict(group).GetBoolean(key, fallback);
        public float GetDictFloat(string group, string key) => GetDict(group).GetFloat(key);
        public float GetDictFloat(string group, string key, float fallback) => GetDict(group).GetFloat(key, fallback);
        public string[] GetDictKeys(string group) => GetDict(group).GetKeys();
        public string[] GetDictValues(string group) => GetDict(group).GetValues();
        public void DictRemove(string group, string key) => GetDict(group).Remove(key);
    }
}
