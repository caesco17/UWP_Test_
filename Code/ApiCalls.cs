using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using UWP_Test.Models;
using Newtonsoft.Json;

namespace UWP_Test.Code
{
    public class ApiCalls
    {
        public string jsonEnvio { get; set; }
        public string jsonRespuesta { get; set; }

        public Message Response { get; set; }

        public List<Message> ResponseList { get; set; }

        public SendingMessage Response2 { get; set; }


        public void _PostMessage(Message msg)
        {
            Utils utils = new Utils();
            var URL_RECEPCION = utils.GetConfigFromXML("URL_RECEPCION");
            var _cliente = new RestClient(URL_RECEPCION + "?Created_Date=" + msg.Created_Date.ToString() + "&SendTo=" + msg.SendTo.ToString() + "&MessageValue=" + msg.MessageValue.ToString());

            _cliente.Timeout = -1;
            var request = new RestRequest(Method.POST);

            request.AddHeader("Content-Type", "application/json");

            request.AddParameter("application/json", "{\"query\":\"\",\"variables\":{}}", ParameterType.RequestBody);

            IRestResponse _response = _cliente.Execute(request);

            Response = JsonConvert.DeserializeObject<Message>(_response.Content);
        }



        public void _PostSendingMessage(int IdGenerado, string confirmation_code)
        {
            Utils utils = new Utils();
            var URL_RECEPCION = utils.GetConfigFromXML("URL_RECEPCION2");
            var _cliente = new RestClient(URL_RECEPCION + "?MessageId=" + IdGenerado.ToString() + "&Sent_Date=" + DateTime.Now.ToString() + "&Confirmation_Code=" + confirmation_code);

            _cliente.Timeout = -1;
            var request = new RestRequest(Method.POST);

            request.AddHeader("Content-Type", "application/json");

            request.AddParameter("application/json", "{\"query\":\"\",\"variables\":{}}", ParameterType.RequestBody);

            IRestResponse _response = _cliente.Execute(request);

            Response2 = JsonConvert.DeserializeObject<SendingMessage>(_response.Content);
        }


        public async void _GetMesagges()
        {
            Utils utils = new Utils();
            string URL_RECEPCION = utils.GetConfigFromXML("URL_RECEPCION");

            HttpClient http = new HttpClient();
            Newtonsoft.Json.Linq.JObject JsonObject = new Newtonsoft.Json.Linq.JObject();
            jsonEnvio = JsonObject.ToString();
            StringContent oString = new StringContent(JsonObject.ToString());

            try
            {
                HttpResponseMessage response = http.GetAsync((URL_RECEPCION)).Result;
                string res = await response.Content.ReadAsStringAsync();

                if (response.StatusCode.ToString() == "OK")
                {
                    jsonRespuesta = res.ToString();
                    ResponseList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Message>>(res);
                }
                else
                {
                    ResponseList = null;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public string Twilio_MSG(Message msg, PhoneNumber number)
        {
            string result = string.Empty;

            Utils utils = new Utils();
            string accountSid = utils.GetConfigFromXML("accountSid");
            string authToken = utils.GetConfigFromXML("authToken");
            string From = utils.GetConfigFromXML("FromNumber");

            try
            {
                TwilioClient.Init(accountSid, authToken);

                var message = MessageResource.Create(
                body: msg.MessageValue,
                from: new Twilio.Types.PhoneNumber(From),
                to: new Twilio.Types.PhoneNumber("+" + number.Phone.ToString())
                );

                result = message.Sid;

            }catch(Exception ex)
            {
                utils._MessageDialog(ex.Message);
                result = "ERROR";
            }

            return result;
        }
    }
}
