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

        public static readonly DependencyProperty ColorRedProperty = DependencyProperty.Register("ColorRed", typeof(byte), typeof(ScrumPokerSettings));
        public byte ColorRed
        {
            get { return (byte)GetValue(ColorRedProperty); }
            set { SetValue(ColorRedProperty, value); }
        }

        public static readonly DependencyProperty ColorGreenProperty = DependencyProperty.Register("ColorGreen", typeof(byte), typeof(ScrumPokerSettings));
        public byte ColorGreen
        {
            get { return (byte)GetValue(ColorGreenProperty); }
            set { SetValue(ColorGreenProperty, value); }
        }

        public static readonly DependencyProperty ColorBlueProperty = DependencyProperty.Register("ColorBlue", typeof(byte), typeof(ScrumPokerSettings));
        public byte ColorBlue
        {
            get { return (byte)GetValue(ColorBlueProperty); }
            set { SetValue(ColorBlueProperty, value); }
        }

        //public static readonly DependencyProperty CardColorProperty = DependencyProperty.Register("CardColor", typeof(Color), typeof(ScrumPokerSettings));
        //public Color CardColor
        //{
        //    get { return (Color)GetValue(CardColorProperty); }
        //    set { SetValue(CardColorProperty, value); }
        //}

        public ScrumPokerSettings()
        {
            PlayerName = "Player Name";
            ColorRed = 0;
            ColorGreen = 0;
            ColorBlue = 200;
            //CardColor = Color.FromRgb(0, 0, 200);
        }

        //protected override void UnserializeOneProperty(PropertyInfo prop, object value)
        //{
        //    if (prop.Name == CardColorProperty.Name)
        //    {
        //        CardColor = (Color)ColorConverter.ConvertFromString((string)value);
        //        return;
        //    }
        //    base.UnserializeOneProperty(prop, value);
        //}
    }
}
