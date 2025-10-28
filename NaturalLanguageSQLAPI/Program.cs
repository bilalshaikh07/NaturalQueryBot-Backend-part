using Microsoft.EntityFrameworkCore;
using NaturalLanguageSQLAPI.Models;
using OpenAI_API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Register OpenAIAPI (from unofficial SDK)
builder.Services.AddSingleton<OpenAIAPI>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var apiKey = config["OpenAI:ApiKey"];
    return new OpenAIAPI(apiKey);
});

builder.Services.AddDbContext<ChatbotContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBconnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
