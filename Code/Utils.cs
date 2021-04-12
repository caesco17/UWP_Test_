using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using UWP_Test.Models;
using System.Text.RegularExpressions;
using System.Xml;

namespace UWP_Test.Code
{
    public class Utils
    {
        //Message Dialog
        public async void _MessageDialog(string _msg)
        {
            MessageDialog showDialog = new MessageDialog(_msg);

            showDialog.Commands.Add(new UICommand("Ok")
            {
                Id = 0
            });

            showDialog.CancelCommandIndex = 0;

            var result = await showDialog.ShowAsync();
        } 

        public bool ValidPhone(PhoneNumber phoneNumber)
        {
            var regex = @"^\+?[1-9]\d{1,14}$";
            var match = Regex.Match(phoneNumber.Phone, regex, RegexOptions.IgnoreCase);

            return match.Success;
        }

        public string GetConfigFromXML(string tag)
        {
            string result = string.Empty;

            var _location = "Config.xml";

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(_location);
            XmlNodeList Base = xDoc.GetElementsByTagName("Parametro");
            XmlNodeList Value = ((XmlElement)Base[0]).GetElementsByTagName(tag);

            result = Value[0].InnerText;

            return result;
        }
    }
}
