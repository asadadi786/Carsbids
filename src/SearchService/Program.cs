using System.Net;
using Polly;
using Polly.Extensions.Http;
using SearchService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container

builder.Services.AddControllers();
builder.Services.AddHttpClient<AuctionHttpSvcClient>().AddPolicyHandler(GetPolicy());

var app = builder.Build();

//Configure the Http request pipeline
app.UseAuthorization();//user access rights

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(async () =>
{
    try
    {
        await DbInitializer.InitDb(app);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
});

app.Run();

static IAsyncPolicy<HttpResponseMessage> GetPolicy() //this method will run if our auction service is down
    => HttpPolicyExtensions
        .HandleTransientHttpError() //gonna handle the exception
        .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
        .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3)); //wait for every 3 secs and retry until auction service is up again and make success request.