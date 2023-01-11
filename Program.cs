using TweetSampler.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ITwitterVolumeAPIService, TwitterVolumeAPIService>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// start a background listener for tweets
using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    var tweetReader = services.GetRequiredService<ITwitterVolumeAPIService>();
    tweetReader.StartListening();
}

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
