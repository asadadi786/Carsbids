using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

//Check req prop and returns Bad Request if it fails validation e.g. here while creating Auction.
//It also allow us to Bind to prop's that are sent up as aurguments to our Endpoints as well. 
[ApiController]
//We also need a route so that the framework knows where to direct the Http request when it comes into our Service.
[Route("api/auctions")]
public class AuctionsController : ControllerBase
{
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;

    public AuctionsController(AuctionDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string date)
    {
        var query = _context.Auctions.OrderBy(x => x.Item.Make).AsQueryable();

        if(!string.IsNullOrEmpty(date))
        {
            query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
        }

        return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();

        // var auctions = await _context.Auctions
        //     .Include(x => x.Item)
        //     .OrderBy(x => x.Item.Make)
        //     .ToListAsync();

        // return _mapper.Map<List<AuctionDto>>(auctions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auction = await _context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        if(auction == null) return NotFound();

        return _mapper.Map<AuctionDto>(auction);
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
    {
        var auction = _mapper.Map<Auction>(auctionDto);

        //TODO: add current user as seller
        auction.Seller = "test";
        
        _context.Auctions.Add(auction);

        var result = await _context.SaveChangesAsync() > 0;

        if(!result) return BadRequest();
        
        return CreatedAtAction(nameof(GetAuctionById), 
            new {auction.Id}, _mapper.Map<AuctionDto>(auction));
    }

    [HttpPut("{id}")]

    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var aution = await _context.Auctions.Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        aution.Item.Make = updateAuctionDto.Make ?? aution.Item.Make;
        aution.Item.Model = updateAuctionDto.Model ?? aution.Item.Model;
        aution.Item.Color = updateAuctionDto.Color ?? aution.Item.Color; 
        aution.Item.Mileage = updateAuctionDto.Mileage ?? aution.Item.Mileage;
        aution.Item.Year = updateAuctionDto.Year ?? aution.Item.Year;   

        //TODO: check Seller == username
        var result = await _context.SaveChangesAsync() > 0;

        if(result) return  Ok();

        return BadRequest("Problem saving changes");     
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await _context.Auctions.FindAsync(id);

        if(auction == null) return NotFound();

        //TODO: check Seller == username

        _context.Auctions.Remove(auction);

        var result = await _context.SaveChangesAsync() > 0;

        if(!result) return BadRequest("Could not delete foom DB");

        return Ok();
    }
}