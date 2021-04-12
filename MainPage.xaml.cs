using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using UWP_Test.Models;
using UWP_Test.Code;
using Microsoft.Toolkit.Uwp.UI.Controls;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace UWP_Test
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ObservableCollection<PhoneNumber> Items;
        private ObservableCollection<Message> Messages;

        public MainPage()
        {
            this.InitializeComponent();

            //Phone Number List Init
            Items = new ObservableCollection<PhoneNumber>();
            MyListView.ItemsSource = Items;

            //Grid Messages Sent
            Messages = GetMessages();
        }

        //Button click
        //Send Message
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Utils _Utils = new Utils();
            Message msg = new Message();
            ApiCalls api = new ApiCalls();

            var _To = string.Empty;
            var _Message = string.Empty;
            var _GeneratedId = 0;

            //Getting Values           
            List<PhoneNumber> phoneNumbers = new List<PhoneNumber>();

            foreach (PhoneNumber item in MyListView.Items)
            {
                phoneNumbers.Add(item);
            }

            if (phoneNumbers.Count > 0)
            {
                SMS_MESSAGE.Document.GetText(Windows.UI.Text.TextGetOptions.AdjustCrlf, out _Message);

                bool isEmpty = string.IsNullOrEmpty(_Message);

                if (!isEmpty)
                {

                    foreach (PhoneNumber x in phoneNumbers)
                    {

                        //Register values on database
                        msg.Created_Date = DateTime.Now;
                        msg.MessageValue = _Message;
                        msg.SendTo = x.Phone;

                        try
                        {
                            api._PostMessage(msg);
                            _GeneratedId = api.Response.MessageId;

                            //Processing values to API
                            string result_Twilio = api.Twilio_MSG(msg, x);

                            api._PostSendingMessage(_GeneratedId, result_Twilio);

                            //Clear after send
                            ClearAll();

                            //Showing Dialog 
                            _Utils._MessageDialog("Message created. Id:" + _GeneratedId.ToString());

                        }
                        catch (Exception ex)
                        {
                            _Utils._MessageDialog(ex.Message.ToString());
                        }
                    }
                }
                else
                {
                    _Utils._MessageDialog("Message is Null or Empty.");
                }
            }
            else
            {
                _Utils._MessageDialog("You should add phonenumbers first.");
            }
        }

        private void ClearAll()
        {
            SMS_TO.Text = SMS_TO.PlaceholderText;
            SMS_MESSAGE.Document.SetText(Windows.UI.Text.TextSetOptions.ApplyRtfDocumentDefaults, "");
            Items.Clear();
        }


        //Button click
        //Add Phone Number
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Utils _Utils = new Utils();
            PhoneNumber _NewPhone = new PhoneNumber();
            _NewPhone.Phone = SMS_TO.Text.ToString();

            //Check if exists
            bool _Exists = Items.Any(item => item.Phone == _NewPhone.Phone);
            if (_Exists)
            {
                _Utils._MessageDialog("Phone number alredy exists on list.");
            }
            else
            {
                bool _IsValid = _Utils.ValidPhone(_NewPhone);
                //Check if valid phone number
                if (_IsValid)
                {
                    Items.Add(_NewPhone);
                }
                else
                {
                    _Utils._MessageDialog("Phone number invalid.");
                    SMS_TO.Text = SMS_TO.PlaceholderText;
                };
            }
        }

        //GetMessages
        //Data from API
        private ObservableCollection<Message> GetMessages()
        {
            ApiCalls api = new ApiCalls();
            ObservableCollection<Message> messages = new ObservableCollection<Message>();
            List<Message> ResponseList;

            api._GetMesagges();

            ResponseList = api.ResponseList;
            
            foreach(Message Msg in ResponseList)
            {
                messages.Add(Msg);
            };

            return messages;
        }


        //List click
        //Delete phone number
        private void MyListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            PhoneNumber item = (PhoneNumber)e.ClickedItem;          
            Items.Remove(item);
        }


        //To sort Grid by Id Column
        private void MsgsGrid_Sorting(object sender, Microsoft.Toolkit.Uwp.UI.Controls.DataGridColumnEventArgs e)
        {
            if (e.Column.Tag.ToString() == "Id")
            {
                if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    MsgsGrid.ItemsSource = new ObservableCollection<Message>(from item in Messages
                                                                             orderby item.MessageId ascending
                                                                        select item);
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else
                {
                    MsgsGrid.ItemsSource = new ObservableCollection<Message>(from item in Messages
                                                                             orderby item.MessageId descending
                                                                        select item);
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }

            //Eliminar los indicadores sort de las columnas que son el Id
            foreach (var col in MsgsGrid.Columns)
            {
                if (col.Tag.ToString() != e.Column.Tag.ToString())
                {
                    col.SortDirection = null;
                }
            }

        }
    }
}
