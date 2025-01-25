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
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseUrls("http://localhost:6969");
            var app = builder.Build();
            app.UseWebSockets();
            app.Map("/ws", async context =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var ws = await context.WebSockets.AcceptWebSocketAsync();
                    
                        string x = await processPosts(client);
                        
                            var bytes = Encoding.UTF8.GetBytes(x);
                            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
                            if (ws.State == WebSocketState.Open)
                            {
                                await ws.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
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
            var json = await client.GetStringAsync("https://dummyjson.com/posts");
            Console.WriteLine(json);
            return json;
        }
    }
}