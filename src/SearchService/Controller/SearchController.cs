using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;

namespace SearchService;

[ApiController]
[Route("api/search")]
public class SearchController: ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Item>>> SearchItems([FromQuery]SearchParams searchParams)
    {
        var query = DB.PagedSearch<Item, Item>();
        
        if (!string.IsNullOrEmpty(searchParams.SearchTerm))
        {
            query.Match(Search.Full, searchParams.SearchTerm).SortByTextScore();
        }

        //sorting
        query = searchParams.OrderBy switch
        {
            "make" => query.Sort(x => x.Ascending(a => a.Make)),
            "new"  => query.Sort(x => x.Descending(a => a.CreatedAt)),
            _ => query.Sort(x => x.Ascending(a => a.AuctionEnd))//this is default sort, showing auctions that ends sooner.
        };

        //filtering
        query = searchParams.FilterBy switch
        {
            "finished" => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
            "endingsoon" => query.Match(x => x.AuctionEnd < DateTime.UtcNow.AddHours(6)),
            _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow) //this is default filter, showing only Live auctions.
        };

        if(!string.IsNullOrEmpty(searchParams.Seller))
        {
            query.Match(x => x.Seller == searchParams.Seller);
        }

        if(!string.IsNullOrEmpty(searchParams.Winner))
        {
            query.Match(x => x.Winner == searchParams.Winner);
        }
        
        query.PageNumber(searchParams.PageNumber);
        query.PageSize(searchParams.PageSize);
        
        var result = await query.ExecuteAsync();
        
        return Ok(new
        {
            result = result.Results,
            pageCount = result.PageCount,
            totalCount = result.TotalCount
        });
    }
}