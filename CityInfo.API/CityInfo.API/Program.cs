using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        // Pluralsight: ASP.NET Core 6 Web API Fundamentals
        // https://app.pluralsight.com/course-player?clipId=d790ce89-f9b4-453b-87e7-2af982bd96f9

        builder.Services.AddControllers(options =>
        {
            options.ReturnHttpNotAcceptable = true;
        }).AddNewtonsoftJson()
        .AddXmlDataContractSerializerFormatters(); // For content negotiation

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

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

