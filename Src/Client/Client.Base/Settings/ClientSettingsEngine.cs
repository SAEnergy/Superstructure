using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Base
{
    public static class ClientSettingsEngine
    {
        private static List<Type> SettingTypes = new List<Type>();
        private static List<ClientSettingsBase> Instances = new List<ClientSettingsBase>();

        public static void Load()
        {
            // todo: list of plugins should be populated by plugin system
            SettingTypes.Add(typeof(WindowPositionSettings));

            foreach (Type t in SettingTypes)
            {
                Instances.Add((ClientSettingsBase)Activator.CreateInstance(t));
            }

            // todo: Unserialize from settings engine

        }

        public static void Save()
        {
            // todo: serialize to settings engine
        }

        public static T GetInstance<T>() where T : ClientSettingsBase
        {
            return (T) Instances.FirstOrDefault(t => t.GetType() == typeof(T));
        }
    }
}
