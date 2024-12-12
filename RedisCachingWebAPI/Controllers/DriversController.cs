using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using RedisCachingWebAPI.Data;
using RedisCachingWebAPI.Models;
using RedisCachingWebAPI.Services;

namespace RedisCachingWebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class DriversController : ControllerBase
{
    

    private readonly ILogger<DriversController> _logger;
    private readonly ICacheServices _cacheService;
    private readonly ApplicationDbContext _db;

    public DriversController(ILogger<DriversController> logger, ICacheServices cacheService, ApplicationDbContext db)
    {
        _logger = logger;
        _cacheService = cacheService;
        _db = db;
    }
    [HttpGet("drivers")]
    public async Task<IActionResult> GetDrivers()
    {
        //check cache data
        var cacheData = _cacheService.GetData<IEnumerable<Driver>>("drivers");
        if(cacheData != null && cacheData.Count() > 0)
        {
            return Ok(cacheData);
        }
        cacheData = await _db.Drivers.ToListAsync();

        //set expiry time
        var expiryTime = DateTimeOffset.Now.AddSeconds(30);
        _cacheService.SetData<IEnumerable<Driver>>("drivers", cacheData, expiryTime);
        return Ok(cacheData);
    }

    [HttpPost("AddDriver")]
    public async Task<IActionResult> AddDriver(Driver value)
    {
        var addedObj = await _db.Drivers.AddAsync(value);
        var expiryTime = DateTimeOffset.Now.AddSeconds(30);
        _cacheService.SetData<Driver>($"driver{value.Id}",addedObj.Entity,expiryTime);

        await _db.SaveChangesAsync();
        return Ok(addedObj.Entity);
    }

    [HttpDelete("DeleteDriver")]

    public async Task<IActionResult> Delete(int id)
    {
        var exist = _db.Drivers.FirstOrDefaultAsync(x => x.Id == id);
        if(exist !=null)
        {
            _db.Remove(exist);
            _cacheService.RemoveData($"driver{id}");
            await _db.SaveChangesAsync();
            return NoContent();
        }
        return NotFound();
    }

}