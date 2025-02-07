using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using passionproject.Data;
using passionproject.Itinerary;
using passionproject.Models.Itinerary.DTOs;

namespace passionproject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DestinationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DestinationsController(ApplicationDbContext context)
        {
            _context = context;
        }

    /// <summary>
    /// Returns a list of all Destinations, each including their associated Activities
    /// </summary>
    /// <returns>
    /// 200 OK
    /// </returns>
    /// <example>
    /// curl -X 'GET' \'http://localhost:5074/api/Destinations' \ -> [{Destination},{Destination},..]
    /// </example>
     [HttpGet]
     public async Task<ActionResult<IEnumerable<DestinationDTO>>> GetDestinations()
    {
    var destinations = await _context.Destinations
        .Include(d => d.Activities)
        .ToListAsync();

       return destinations.Select(destination => new DestinationDTO
      {
        DestinationId = destination.DestinationId,
        Name = destination.Name,
        Location = destination.Location,
        StartDate = destination.StartDate,
        EndDate = destination.EndDate,
        Description = destination.Description,
        Budget = destination.Budget,
        Activities = destination.Activities.Select(a => new ActivityDTO
         {
            ActivityId = a.ActivityId,
            Name = a.Name,
            DateTime = a.DateTime,
            Location = a.Location,
            Description = a.Description,
            Cost = a.Cost,
            DestinationId = a.DestinationId
         }).ToList()
       }).ToList();
    }
   
    /// <summary>
    /// Returns a specific Destination by ID with its associated Activities
    /// </summary>
    /// <param name="destinationId">The ID of the Destination to retrieve</param>
    /// <returns>
    /// 200 OK {Destination}
    /// </returns>
    /// <example>
    /// curl -X 'GET' \'http://localhost:5074/api/Destinations'/3 \ -> {Destination}
    /// </example>
     [HttpGet("{id}")]
      public async Task<ActionResult<DestinationDTO>> GetDestination(int id)
    {
      var destination = await _context.Destinations
        .Include(d => d.Activities)
        .FirstOrDefaultAsync(d => d.DestinationId == id);

       if (destination == null)
      {
        return NotFound();
      }

       return new DestinationDTO
      {
        DestinationId = destination.DestinationId,
        Name = destination.Name,
        Location = destination.Location,
        StartDate = destination.StartDate,
        EndDate = destination.EndDate,
        Description = destination.Description,
        Budget = destination.Budget,
        Activities = destination.Activities.Select(a => new ActivityDTO
         {
            ActivityId = a.ActivityId,
            Name = a.Name,
            DateTime = a.DateTime,
            Location = a.Location,
            Description = a.Description,
            Cost = a.Cost,
            DestinationId = a.DestinationId
          }).ToList()
       };
    }
    // POST: api/Destinations
    /// <summary>
    /// Creates a new Destination in the database
    /// </summary>
    /// <param name="newDestination">The Destination object to add</param>
    /// <returns>
    /// 201 Created and {Destination}
    /// </returns>
    /// <example>
    /// curl -X 'POST' \'http://localhost:5074/api/Destinations'\ -> 
    /// {
    ///"destinationId": 2,
    ///"name": " Mexic Trip",
    ///"location": "Mexic City",
    ///"startDate": "2024-08-01T00:00:00",
    ///"endDate": "2024-08-10T00:00:00",
    ///"description": "Summer break trip to Mexico",
    ///"budget": 6000,
    ///"activities": []
    ///},
    /// </example>
        [HttpPost]
        public async Task<ActionResult<Destination>> PostDestination(Destination destination)
    {
            _context.Destinations.Add(destination);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDestination), new { id = destination.DestinationId }, destination);
    }

    // PUT: api/Destinations/{id}
    /// <summary>
    /// Updates a Destination in the database by ID
    /// </summary>
    /// <param name="destinationId">The ID of the Destination to update</param>
    /// <param name="destination">Updated destination information</param>
    /// <returns>
    /// 204 No Content
    /// </returns>
    /// <example>
    /// curl -X 'PUT' \
    /// 'http://localhost:5074/api/Destinations/2' \
    /// -H 'Content-Type: application/json' \
    /// -d '{
    ///   "destinationId": 2,
    ///   "name": "Mexic Trip",
    ///   "location": "Mexic City",
    ///   "startDate": "2024-08-01T00:00:00",
    ///   "endDate": "2024-08-10T00:00:00",
    ///   "description": "summer break trip to Mexico",
    ///   "budget": 6000
    /// }'
    /// -> 204
    /// </example>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDestination(int id, Destination destination)
    {
            if (id != destination.DestinationId)
            {
                return BadRequest();
            }

            _context.Entry(destination).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DestinationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
    }

    // DELETE: api/Destinations/5
    /// <summary>
    /// Deletes a Destination from the database
    /// </summary>
    /// <param name="destinationId">The ID of the Destination to delete</param>
    /// <returns>
    /// 204 No Content
    /// </returns>
    /// <example>
    /// curl -X 'DELETE' 'http://localhost:5074/api/Destinations/2' -> OK
    /// </example>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDestination(int id)
    {
            var destination = await _context.Destinations.FindAsync(id);
            if (destination == null)
            {
                return NotFound();
            }

            _context.Destinations.Remove(destination);
            await _context.SaveChangesAsync();

            return NoContent();
    }

        private bool DestinationExists(int id)
        {
            return _context.Destinations.Any(e => e.DestinationId == id);
        }
  }
}