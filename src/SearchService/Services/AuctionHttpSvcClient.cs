using MongoDB.Entities;

namespace SearchService
{
    public class AuctionHttpSvcClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public AuctionHttpSvcClient(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }        

        public async Task<List<Item>> GetItemForSearchDb()
        {
            //And this Query is going to give us the date of the auction that's been updated, the latest in our database,
            //and this is what we can send to our auction service so that the date parameter is passed as a query string.
            var lastUpdated = await DB.Find<Item,string>()
                              .Sort(x => x.Descending(x => x.UpdatedAt))
                              .Project(x => x.UpdatedAt.ToString())
                              .ExecuteFirstAsync();

            //So then we can actually make the call to our auction service.
            //And we use Jason async so it automatically deserializes the Json that we get back from the auction service.
            //And we're going to say that we want to deserialize this into a list of items.
            //And then we can pass our config and we can get our auction by providing Service URL.
            return await _httpClient.GetFromJsonAsync<List<Item>>(_config["AuctionServiceUrl"]
                   + "/api/auctions?date=" +lastUpdated);

        }
    }
}