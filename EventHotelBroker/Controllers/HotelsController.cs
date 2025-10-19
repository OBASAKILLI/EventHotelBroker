using EventHotelBroker.Models;
using EventHotelBroker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHotelBroker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelsController : ControllerBase
{
    private readonly IHotelService _hotelService;
    private readonly ILogger<HotelsController> _logger;

    public HotelsController(IHotelService hotelService, ILogger<HotelsController> logger)
    {
        _hotelService = hotelService;
        _logger = logger;
    }

    /// <summary>
    /// Get all published hotels (public endpoint)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Hotel>>> GetHotels()
    {
        try
        {
            var hotels = await _hotelService.GetPublishedHotelsAsync();
            return Ok(hotels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting hotels");
            return StatusCode(500, "An error occurred while retrieving hotels");
        }
    }

    /// <summary>
    /// Get hotel by ID (public endpoint)
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Hotel>> GetHotel(int id)
    {
        try
        {
            var hotel = await _hotelService.GetHotelWithDetailsAsync(id);
            
            if (hotel == null)
            {
                return NotFound();
            }

            return Ok(hotel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting hotel {HotelId}", id);
            return StatusCode(500, "An error occurred while retrieving the hotel");
        }
    }

    /// <summary>
    /// Search hotels with filters (public endpoint)
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Hotel>>> SearchHotels(
        [FromQuery] string? keyword,
        [FromQuery] string? city,
        [FromQuery] int? minCapacity,
        [FromQuery] decimal? maxPrice)
    {
        try
        {
            var hotels = await _hotelService.SearchHotelsAsync(keyword, city, minCapacity, maxPrice);
            return Ok(hotels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching hotels");
            return StatusCode(500, "An error occurred while searching hotels");
        }
    }

    /// <summary>
    /// Get hotels by owner (requires authentication)
    /// </summary>
    [HttpGet("my-hotels")]
    [Authorize(Roles = "HotelOwner")]
    public async Task<ActionResult<IEnumerable<Hotel>>> GetMyHotels()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var hotels = await _hotelService.GetHotelsByOwnerAsync(userId);
            return Ok(hotels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting owner hotels");
            return StatusCode(500, "An error occurred while retrieving your hotels");
        }
    }

    /// <summary>
    /// Create new hotel (requires HotelOwner role)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "HotelOwner")]
    public async Task<ActionResult<Hotel>> CreateHotel([FromBody] Hotel hotel)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            hotel.OwnerId = userId;
            hotel.IsApproved = false; // Requires admin approval

            var createdHotel = await _hotelService.CreateHotelAsync(hotel);
            
            return CreatedAtAction(nameof(GetHotel), new { id = createdHotel.Id }, createdHotel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating hotel");
            return StatusCode(500, "An error occurred while creating the hotel");
        }
    }

    /// <summary>
    /// Update hotel (requires HotelOwner role and ownership)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "HotelOwner")]
    public async Task<ActionResult<Hotel>> UpdateHotel(int id, [FromBody] Hotel hotel)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var existingHotel = await _hotelService.GetHotelByIdAsync(id);
            
            if (existingHotel == null)
            {
                return NotFound();
            }

            // Check ownership
            if (existingHotel.OwnerId != userId)
            {
                return Forbid();
            }

            hotel.Id = id;
            hotel.OwnerId = userId;
            
            var updatedHotel = await _hotelService.UpdateHotelAsync(hotel);
            return Ok(updatedHotel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating hotel {HotelId}", id);
            return StatusCode(500, "An error occurred while updating the hotel");
        }
    }

    /// <summary>
    /// Delete hotel (requires HotelOwner role and ownership)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "HotelOwner")]
    public async Task<ActionResult> DeleteHotel(int id)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var existingHotel = await _hotelService.GetHotelByIdAsync(id);
            
            if (existingHotel == null)
            {
                return NotFound();
            }

            // Check ownership
            if (existingHotel.OwnerId != userId)
            {
                return Forbid();
            }

            await _hotelService.DeleteHotelAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting hotel {HotelId}", id);
            return StatusCode(500, "An error occurred while deleting the hotel");
        }
    }

    /// <summary>
    /// Approve hotel (Admin only)
    /// </summary>
    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> ApproveHotel(int id)
    {
        try
        {
            await _hotelService.ApproveHotelAsync(id);
            return Ok(new { message = "Hotel approved successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving hotel {HotelId}", id);
            return StatusCode(500, "An error occurred while approving the hotel");
        }
    }

    /// <summary>
    /// Reject hotel (Admin only)
    /// </summary>
    [HttpPost("{id}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> RejectHotel(int id)
    {
        try
        {
            await _hotelService.RejectHotelAsync(id);
            return Ok(new { message = "Hotel rejected successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting hotel {HotelId}", id);
            return StatusCode(500, "An error occurred while rejecting the hotel");
        }
    }
}
