using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Base
{
    [ClientSettingsMetadata(Hidden = true)]
    public class WindowPositionSettings : ClientSettingsBase
    {
        public Dictionary<string, string> WindowPositions { get; protected set; }

        public WindowPositionSettings()
        {
            WindowPositions = new Dictionary<string, string>();
        }


        protected override object SerializeOneProperty(PropertyInfo prop)
        {
            if (prop.Name == "WindowPositions")
            {
                List<string> vals = new List<string>();
                foreach (var pair in WindowPositions)
                {
                    vals.Add(pair.Key + "=" + pair.Value);
                }
                return vals.ToArray();
            }
            return base.SerializeOneProperty(prop);
        }

        protected override void UnserializeOneProperty(PropertyInfo prop, object value)
        {
            if (prop.Name == "WindowPositions")
            {
                foreach (string s in (string[])value)
                {
                    WindowPositions.Add(s.Substring(0, s.IndexOf("=")), s.Substring(s.IndexOf("=") + 1));
                }
                return;
            }
            base.UnserializeOneProperty(prop, value);
        }

        public void Serialize(DialogBase sourceDialog)
        {
            string className = sourceDialog.GetType().Name;
            string windowPos = sourceDialog.WindowState.ToString() + "," + sourceDialog.Top + "," + sourceDialog.Left + "," + sourceDialog.Width + "," + sourceDialog.Height;
            WindowPositions[className] = windowPos;
        }

        public void Unserialize(DialogBase targetDialog)
        {
            string className = targetDialog.GetType().Name;
            string windowPos = null;
            if (WindowPositions.TryGetValue(className, out windowPos))
            {
                string[] values = windowPos.Split(',');

                targetDialog.WindowState = (WindowState)Enum.Parse(typeof(WindowState), values[0]);
                targetDialog.WindowStartupLocation = WindowStartupLocation.Manual;
                targetDialog.Top = double.Parse(values[1]);
                targetDialog.Left = double.Parse(values[2]);
                targetDialog.Width = double.Parse(values[3]);
                targetDialog.Height = double.Parse(values[4]);
            }
        }
    }
}
