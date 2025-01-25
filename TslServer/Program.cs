using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace TslServer
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseUrls("http://localhost:6969");
            var app = builder.Build();
            app.UseWebSockets();
            app.Map("/ws", async context =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var ws = await context.WebSockets.AcceptWebSocketAsync();                                      
                            var message = "hello world";
                            var bytes = Encoding.UTF8.GetBytes(message);
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
        
    }
}
