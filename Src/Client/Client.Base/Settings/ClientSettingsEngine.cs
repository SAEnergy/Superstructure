using Core.Util;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace Client.Base
{
    public static class ClientSettingsEngine
    {
        private const string _keyname = "SimpleClient";
        private static List<ClientSettingsBase> Instances = new List<ClientSettingsBase>();

        public static void Load()
        {
            lock (Instances)
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                key = key.CreateSubKey(_keyname);
                List<KeyValuePair<string, object>> values = new List<KeyValuePair<string, object>>();
                foreach(string name in key.GetValueNames())
                {
                    values.Add(new KeyValuePair<string, object>(name, key.GetValue(name)));
                }

                List<Type> types = new List<Type>();
                types.AddRange(TypeLocator.FindTypes("*client*dll", typeof(ClientSettingsBase)));
                types.AddRange(TypeLocator.FindTypes("*plugin*dll", typeof(ClientSettingsBase)));

                foreach (Type t in types.Distinct())
                {
                    ClientSettingsBase instance = (ClientSettingsBase)Activator.CreateInstance(t);
                    instance.Unserialize(values);
                    Instances.Add(instance);
                }
            }
        }

        public static void Save()
        {
            lock (Instances)
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                key = key.CreateSubKey(_keyname);

                foreach (ClientSettingsBase instance in Instances)
                {
                    foreach (var pair in instance.Serialize())
                    {
                        key.SetValue(pair.Key, pair.Value);
                    }
                }
            }
        }



        public static T GetInstance<T>() where T : ClientSettingsBase
        {
            lock (Instances)
            {
                return (T)Instances.FirstOrDefault(t => t.GetType() == typeof(T));
            }
        }
    }
}
