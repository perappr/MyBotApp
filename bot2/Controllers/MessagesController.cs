using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Configuration;

namespace bot2
{

    //[BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                //// calculate something for us to return
                //int length = (activity.Text ?? string.Empty).Length;

                //// return our reply to the user
                //Activity reply = activity.CreateReply($"You sent {activity.Text} which was {length} characters");
                //await connector.Conversations.ReplyToActivityAsync(reply);

                string ResponseString="";


                LUISDataModel LDM = await GetIntentFromLUIS(activity.Text);
                if (LDM.entities.Count() > 0)
                //if (LDM.entities.Count() > 0)
                    {
                    //switch (LDM.topintent.intent)
                    switch (LDM.entities[0].type)
                    {
                        case "StockSymbol":
                            ResponseString = await GetStock(LDM.entities[0].entity);
                            break;
                        case "WeatherLocation":
                            ResponseString = await GetWeather(LDM.entities[0].entity);
                            break;
                        default:
                            ResponseString = "Sorry, I am not getting you...";
                            break;
                    }
                }
                else
                {
                    ResponseString = "Sorry, I am not getting you...";
                }

                // return our reply to the user  
                Activity reply = activity.CreateReply(ResponseString);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }

        private static async Task<LUISDataModel> GetIntentFromLUIS(string Query)
        {
            Query = Uri.EscapeDataString(Query);
            LUISDataModel Data = new LUISDataModel();
            using (HttpClient client = new HttpClient())
            {

                string RequestURI = ConfigurationManager.AppSettings["LuisURI"] + "&q=" + Query;
                HttpResponseMessage msg = await client.GetAsync(RequestURI);

                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                    Data = JsonConvert.DeserializeObject<LUISDataModel>(JsonDataResponse);
                }
            }
            return Data;
        }
        private async Task<string> GetStock(string StockSymbol)
        {
            double? dblStockValue = await StockPrice.GetStockPriceAsync(StockSymbol);
            if (dblStockValue == null)
            {
                return string.Format("This \"{0}\" is not an valid stock symbol", StockSymbol);
            }
            else
            {
                return string.Format("Stock Price of {0} is {1}", StockSymbol, dblStockValue);
            }
        }

        private async Task<string> GetWeather(string WeatherLocation)
        {
            string ApiKey = ConfigurationManager.AppSettings["OpenWeatherMap_App_Id"];
            WeatherService weatherService = new WeatherService();
            string city, time, condition;

            string weatherFormat = "Hello, It's {0} in {1}.. with low temp at {2} and high at {3}..";

            var weather = await weatherService.GetWeatherData(WeatherLocation, ApiKey, "en");

            string description = weather.weather.FirstOrDefault()?.description;
            string lowAt = weather.main.temp_min + "";
            string highAt = weather.main.temp_min + "";
            string cityName = "";


            cityName = weather.name + ", " + weather.sys.country;


            //Build a reply message


            return string.Format(weatherFormat, description, cityName, lowAt, highAt);

        }

    }
}
