
using Cachy.StackExchangeRedis;
using Cachy.WebApi.Contexts;

namespace Cachy.WebApi
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

            //builder.Services.AddScoped<MyCacheContext>();

            //builder.Services.AddRedisCacheContext<MyRedisCacheContext>(options => {
            //    options.Configuration = "redis-11265.c252.ap-southeast-1-1.ec2.redns.redis-cloud.com:11265,password=qzsXvAXiMo8M9mleD4kHwGlEmFevVCzN,defaultDatabase=1";
            //    options.InstanceName = "test";
            //});

            builder.Services.AddCacheContext<MyCacheContext>();
            //builder.Services.AddRedisCacheContext<MyCacheContext>(options =>
            //{
            //    options.Configuration = "redis-11265.c252.ap-southeast-1-1.ec2.redns.redis-cloud.com:11265,password=qzsXvAXiMo8M9mleD4kHwGlEmFevVCzN,defaultDatabase=1";
            //    options.InstanceName = "SampleInstance";
            //});

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
