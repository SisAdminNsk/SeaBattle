using SeaBattleApi;
using System.Net.Sockets;
using System.Net;


namespace SeaBattle
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddGameServices();
            builder.Services.AddMemoryCache();
            //builder.Services.AllowAllOrigins();

            string LocalIp = GetLocalIPAddress();

            builder.WebHost.UseUrls("http://localhost:8084", "http://" + LocalIp + ":8084");


            var app = builder.Build();
            //app.UseCors("AllowAll");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseCors(builder =>
            {
                builder.WithOrigins("http://localhost:4200") // Разрешить запросы с этого домена
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            });

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseWebSockets();

            app.MapControllers();

            app.Run();
        }

        static string GetLocalIPAddress()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint? endPoint = socket.LocalEndPoint as IPEndPoint;

                if (endPoint != null)
                {
                    return endPoint.Address.ToString();
                }
                else
                {
                    return "127.0.0.1";
                }
            }
        }
    }
}
