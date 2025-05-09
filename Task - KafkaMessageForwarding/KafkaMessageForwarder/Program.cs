using KafkaMessageForwarder;
using KafkaMessageForwarder.Services;
namespace KafkaMessageCopier
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddSingleton<KafkaForwarderService>();
            builder.Services.AddHostedService<Worker>();
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
