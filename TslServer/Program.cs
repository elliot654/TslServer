using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace TslServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using HttpClient client = new HttpClient();
            string heldData = null; //container to check new requests for updates. would be a model object not a string if not a 2 hour demo
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseUrls("http://localhost:6868"); //move to connectionstrings or config files for larger application or if security matters
            var app = builder.Build();
            app.UseWebSockets();
            app.Map("/ws", async context =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var ws = await context.WebSockets.AcceptWebSocketAsync();
                    while (true)
                    {
                        string x = await processPosts(client);
                        if (heldData != x) //only send to the client if there is new data from api
                        {
                            var bytes = Encoding.UTF8.GetBytes(x);
                            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
                            if (ws.State == WebSocketState.Open)
                            {
                                await ws.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
                            }
                            else
                            {
                                break;
                            }
                            heldData = x;
                        }
                        Thread.Sleep(1000); //polling rate would be a more accessable variable maybe option for client depending on use case of real project
                    }
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            });
            await app.RunAsync();
        }
        static async Task<string> processPosts(HttpClient client)
        {
            var json = await client.GetStringAsync("http://dev-sample-api.tsl-timing.com/sample-data");
            Console.WriteLine(json);
            return json;
        }
    }
}
