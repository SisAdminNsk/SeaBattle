using SeaBattleApi.Services;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text;

namespace SeaBattleApi
{
    public static class Extentions
    {
        public static void AllowAllOrigins(this IServiceCollection services)
        {
            services.AddCors(options => {
                options.AddPolicy("AllowAllOrigins", builder => {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
        }

        public static void AddGameServices(this IServiceCollection services)
        {
            services.AddTransient<IGameSessionService, GameSessionService>();
            services.AddSingleton<IPlayerConnectionsService, PlayerConnectionsService>();
        }

        public static ArraySegment<byte> ToArraySegment<T>(this WebSocket socket, T data)
        {
            byte[] byteArray;

            if(data is string)
            {
                byteArray = Encoding.UTF8.GetBytes(data as string);

                return new ArraySegment<byte>(byteArray);
            }

            var jsonData = JsonSerializer.Serialize(data);
            byteArray = Encoding.UTF8.GetBytes(jsonData);

            return new ArraySegment<byte>(byteArray);
        }
    }
}
