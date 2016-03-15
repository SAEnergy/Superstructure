using Client.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Reflection;

namespace Example.ScrumPoker.Plugin
{
    public class ScrumPokerSettings : ClientSettingsBase
    {
        public static readonly DependencyProperty PlayerNameProperty = DependencyProperty.Register("PlayerName", typeof(string), typeof(ScrumPokerSettings));
        public string PlayerName
        {
            get { return (string)GetValue(PlayerNameProperty); }
            set { SetValue(PlayerNameProperty, value); }
        }

        public static readonly DependencyProperty CardColorProperty = DependencyProperty.Register("CardColor", typeof(Color), typeof(ScrumPokerSettings));
        public Color CardColor
        {
            get { return (Color)GetValue(CardColorProperty); }
            set { SetValue(CardColorProperty, value); }
        }

        public ScrumPokerSettings()
        {
            PlayerName = "Player Name";
            CardColor = Color.FromRgb(0, 0, 200);
        }

        protected override void UnserializeOneProperty(PropertyInfo prop, object value)
        {
            if (prop.Name == CardColorProperty.Name)
            {
                CardColor = (Color)ColorConverter.ConvertFromString((string)value);
                return;
            }
            base.UnserializeOneProperty(prop, value);

        }
    }
}
