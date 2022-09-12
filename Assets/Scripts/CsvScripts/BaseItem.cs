using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;

namespace SolarGames.Scripts
{
    public class BaseItem
    {
        [System.AttributeUsage(System.AttributeTargets.Property)]
        public class DataName : System.Attribute
        {
            public string Name { get; set; }
            public DataName(string name)
            {
                this.Name = name;
            }
        }

        public virtual void ReadDictionary(Dictionary<string, string> valuesDict)
        {
            var allProps = this.GetType().GetProperties();
            var actualFields = allProps.Select(o => new { key = o.GetCustomAttributes(typeof(DataName), true).Cast<DataName>().FirstOrDefault(), prop = o });
            Dictionary<string, PropertyInfo> propsDict = actualFields.Where(o => o.key != null).ToDictionary(k => k.key.Name.ToLower(), v => v.prop);
            foreach (var pair in propsDict)
            {
                if (valuesDict.ContainsKey(pair.Key))
                {
                    try
                    {
                        string valueStr = valuesDict[pair.Key];
                        ParseValue(pair, valueStr);
                    }
                    catch
                    {
                    //    Debug.LogWarning(valuesDict.ToStringCustomDict());
                        Debug.LogErrorFormat("[BaseStuffItem.ReadCsv] fail fo set value `{1}` for key `{0}`", pair.Key, valuesDict[pair.Key]);
                        throw;
                    }
                }
                else
                {
                   // Debug.LogWarning(valuesDict.ToStringCustomDict());
                    Debug.LogErrorFormat("[BaseStuffItem.ReadCsv] fail fo get value for key {0}", pair.Key);
                }
            }
        }

        private void ParseValue(KeyValuePair<string, PropertyInfo> propPair, string valueStr)
        {
            if (propPair.Value.PropertyType.IsEnum)
            {
                var value = Enum.Parse(propPair.Value.PropertyType, valueStr);
                propPair.Value.SetValue(this, value, null);
            }
            else if (propPair.Value.PropertyType == typeof(bool))
            {
                bool value = false;
                if (Boolean.TryParse(valueStr, out value))
                {
                    propPair.Value.SetValue(this, value, null);
                }
                else
                {
                    propPair.Value.SetValue(this, valueStr == "1", null);
                }
            }
            else
            {

                if (string.IsNullOrEmpty(valueStr) && (propPair.Value.PropertyType == typeof(int) || propPair.Value.PropertyType == typeof(float)))
                    valueStr = "0";
                if (string.IsNullOrEmpty(valueStr) && propPair.Value.PropertyType == typeof(string))
                    valueStr = "";

                object value = Convert.ChangeType(valueStr, propPair.Value.PropertyType);
                propPair.Value.SetValue(this, value, null);
            }
        }

        public void ReadTsv(List<string> keys, List<string> values)
        {
            var allProps = this.GetType().GetProperties();
            var actualFields = allProps.Select(o => new { key = o.GetCustomAttributes(typeof(DataName), true).Cast<DataName>().FirstOrDefault(), prop = o });
            Dictionary<string, PropertyInfo> dict = actualFields.Where(o => o.key != null).ToDictionary(k => k.key.Name, v => v.prop);
            foreach (var pair in dict)
            {
                int index = keys.IndexOf(pair.Key);
                string valueStr = values.ElementAtOrDefault(index);
                if (index >= 0 && !string.IsNullOrEmpty(valueStr))
                {
                    try
                    {
                        ParseValue(pair, valueStr);
                    }
                    catch
                    {
                        Debug.LogErrorFormat("[BaseStuffItem.ReadCsv] fail fo set value `{1}` for key `{0}`", pair.Key, valueStr);
                        throw;
                    }
                }
                else
                {
                    Debug.LogErrorFormat("[BaseStuffItem.ReadCsv] fail fo get value for key {0}", pair.Key);
                }
            }
        }

        public void ReadXml(XmlNode node)
        {
            //string text = node.OuterXml;
            var allProps = this.GetType().GetProperties();
            var actualFields = allProps.Select(o => new { key = o.GetCustomAttributes(typeof(DataName), true).Cast<DataName>().FirstOrDefault(), prop = o });
            Dictionary<string, PropertyInfo> dict = actualFields.Where(o => o.key != null).ToDictionary(k => k.key.Name, v => v.prop);
            foreach (var pair in dict)
            {
                XmlAttribute attr = node.Attributes[pair.Key];
                if (attr != null)
                {
                    try
                    {
                        if (pair.Value.PropertyType.IsEnum)
                        {
                            var value = Enum.Parse(pair.Value.PropertyType, attr.Value);
                            pair.Value.SetValue(this, value, null);
                        }
                        else
                        {
                            object value = Convert.ChangeType(attr.Value, pair.Value.PropertyType);
                            pair.Value.SetValue(this, value, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarningFormat("[StuffEquip.ReadXml] attribute `{0}` failed to set value {1}", pair.Key, node.OuterXml);
                        Debug.LogException(ex);
                    }
                }
                else
                {
                    //Debug.LogFormat("[StuffEquip.ReadXml] attribute `{0}` not found in xml node {1}", pair.Key, node.OuterXml);
                }
            }
        }

        protected T Parse<T>(string key, Dictionary<string, string> valuesDict)
        {
            if (valuesDict.ContainsKey(key))
            {
                try
                {
                    return (T)Convert.ChangeType(valuesDict[key], typeof(T));
                }
                catch
                {
                    Debug.LogErrorFormat("[BaseStuffItem.Parse] fail to parse `{0}` to `{1}`", valuesDict[key], typeof(T));
                    throw;
                }
            }
            else
            {
                throw new Exception("[BaseStuffItem.Parse] dictionary doesnt contain key " + key);
            }
        }
    }
}
