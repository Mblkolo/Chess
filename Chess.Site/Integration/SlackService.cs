using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chess.Site.Integration
{
    using System.Net.Http;
    using System.Text;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    public class SlackService
    {
        private readonly SlackOptions slackOptions;

        public SlackService(IOptions<SlackOptions> options)
        {
            slackOptions = options?.Value;
        }

        public void SendMessage(string message)
        {
            if (string.IsNullOrEmpty(slackOptions?.PostUrl))
                return;

            using (var client = new HttpClient())
            {
                var payload = new Payload
                {
                    Text = message
                };

                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                client.PostAsync(slackOptions.PostUrl, content).GetAwaiter().GetResult();
            }
            
        }

        private class Payload
        {
            [JsonProperty("text")]
            public string Text { get; set; }

            [JsonProperty("username")]
            public string Username { get; } = "chessbot";

            [JsonProperty("icon_emoji")]
            public string IconEmoji { get; } = ":horse:";
        }
    }
}
