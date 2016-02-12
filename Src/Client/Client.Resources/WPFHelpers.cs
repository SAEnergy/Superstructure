using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Client.Resources
{
    public static class WPFHelpers
    {
        public static ImageSource GetImage(string path)
        {
            string packString;
            ImageSourceConverter converter = new ImageSourceConverter();

            // first look in calling assembly
            packString = string.Format(
                "pack://application:,,,/{0};component/{1}"
                , Assembly.GetCallingAssembly().GetName().Name
                , path
            );

            //todo: this throws a null reference, need to find a way to see if path is valid without exception
            if (converter.IsValid(packString))
            {
                return (ImageSource)converter.ConvertFromString(packString);
            }

            // now look in this assembly
            packString = string.Format(
                "pack://application:,,,/{0};component/{1}"
                , Assembly.GetExecutingAssembly().GetName().Name
                , path
            );

            if (converter.IsValid(packString))
            {
                return (ImageSource)converter.ConvertFromString(packString);
            }

            // give up ¯\_(ツ)_/¯
            return null;
        }
    }
}
