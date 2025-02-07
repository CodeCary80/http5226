using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using passionproject.Data;
using passionproject.Itinerary;
using passionproject.Models.Itinerary.DTOs;

namespace passionproject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivitiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ActivitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

    /// <summary>
    /// Returns a list of all Activities with their Destinations and Members
    /// </summary>
    /// <returns>
    /// 200 OK
    /// [{Activity},{Activity},..]
    /// </returns>
    /// <example>
    /// curl -X 'GET' \'http://localhost:5074/api/Activities' \ -> {[{Activity},{member}],[{Activity},{member}]}...
    /// </example>
   [HttpGet]
public async Task<ActionResult<IEnumerable<ActivityDTO>>> GetActivities()
{
    var activities = await _context.Activities
        .Include(a => a.Destination)
        .Include(a => a.ActivityMembers)
            .ThenInclude(am => am.Member)
        .ToListAsync();

    return activities.Select(activity => new ActivityDTO
    {
        ActivityId = activity.ActivityId,
        Name = activity.Name,
        DateTime = activity.DateTime,
        Location = activity.Location,
        Description = activity.Description,
        Cost = activity.Cost,
        DestinationId = activity.DestinationId,
        Destination = new DestinationSummaryDTO
        {
            DestinationId = activity.Destination.DestinationId,
            Name = activity.Destination.Name,
            Location = activity.Destination.Location
        },
        Members = activity.ActivityMembers.Select(am => new MemberSummaryDTO
        {
            MemberId = am.Member.MemberId,
            Name = am.Member.Name,
            Email = am.Member.Email,
            IsOrganizer = am.IsOrganizer,
            Notes = am.Notes
        }).ToList()
    }).ToList();
}

    /// GET: api/Activities/{id}
    /// <summary>
    /// Returns a specific Activity by ID with its Destination and Members
    /// </summary>
    /// <param name="id">The ID of the Activity to retrieve</param>
    /// <returns>
    /// 200 OK 
    /// </returns>
    /// <example>
    /// curl -X 'GET' \'http://localhost:5074/api/Activities/1' \ -> {[{Activity},{member}],[{Activity},{member}]}...
    /// </example>
     [HttpGet("{id}")]
public async Task<ActionResult<ActivityDTO>> GetActivity(int id)
{
    var activity = await _context.Activities
        .Include(a => a.Destination)
        .Include(a => a.ActivityMembers)
            .ThenInclude(am => am.Member)
        .FirstOrDefaultAsync(a => a.ActivityId == id);

    if (activity == null)
    {
        return NotFound();
    }

    return new ActivityDTO
    {
        ActivityId = activity.ActivityId,
        Name = activity.Name,
        DateTime = activity.DateTime,
        Location = activity.Location,
        Description = activity.Description,
        Cost = activity.Cost,
        DestinationId = activity.DestinationId,
        Destination = new DestinationSummaryDTO
        {
            DestinationId = activity.Destination.DestinationId,
            Name = activity.Destination.Name,
            Location = activity.Destination.Location
        },
        Members = activity.ActivityMembers.Select(am => new MemberSummaryDTO
        {
            MemberId = am.Member.MemberId,
            Name = am.Member.Name,
            Email = am.Member.Email,
            IsOrganizer = am.IsOrganizer,
            Notes = am.Notes
        }).ToList()
    };
}
    // POST: api/Activities
    /// <summary>
    /// Creates a new Activity linked to a Destination
    /// </summary>
    /// <param name="activity">The Activity object to add</param>
    /// <returns>
    /// 201 Created {Activity}
    /// 400 Bad Request
    /// </returns>
    /// <example>
    /// curl -X 'POST' \
    /// 'http://localhost:5074/api/Activities' \
    /// -H 'Content-Type: application/json' \
    /// -d '{
    ///   "name": "Winter Market",
    ///   "dateTime": "2024-12-24T00:00:00",
    ///   "location": "Toronto",
    ///   "description": "Christmas Market",
    ///   "cost": 600,
    ///   "destinationId": 3
    /// }'
    /// -> 201 Created {Activity}
    ///</example>
    [HttpPost]
    public async Task<ActionResult<Activity>> PostActivity(Activity activity)
{
    // Find the destination first
    var destination = await _context.Destinations.FindAsync(activity.DestinationId);
    if (destination == null)
     {
        return BadRequest("Invalid DestinationId");
     }

    // Set the destination
    activity.Destination = destination;
    
    _context.Activities.Add(activity);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetActivity), new { id = activity.ActivityId }, activity);
}

    // PUT: api/Activities/5
    /// <summary>
    /// Updates an existing Activity
    /// </summary>
    /// <param name="id">The ID of the Activity to update</param>
    /// <param name="activity">Updated activity information</param>
    /// <returns>
    /// 204 No Content
    /// 400 Bad Request
    /// 404 Not Found
    /// </returns>
    /// <example>
    /// curl -X 'PUT' \
    /// 'http://localhost:5074/api/Activities/1' \
    /// -H 'Content-Type: application/json' \
    /// -d '{
    ///   "activityId": 1,
    ///   "name": "Updated Tower Visit",
    ///   "dateTime": "2024-07-02T15:00:00",
    ///   "location": "Tokyo Tower",
    ///   "description": "Evening visit",
    ///   "cost": 1200,
    ///   "destinationId": 1
    /// }'
    /// -> 201 Created {Activity}
    /// </example>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutActivity(int id, Activity activity)
        {
            if (id != activity.ActivityId)
            {
                return BadRequest();
            }

            _context.Entry(activity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActivityExists(id))
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

        // DELETE: api/Activities/5
        /// <summary>
        /// Deletes an Activity
        /// </summary>
        /// <param name="id">The ID of the Activity to delete</param>
        /// <returns>
        /// 204 No Content
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// curl -X 'DELETE' 'http://localhost:5074/api/Activities/1' -> 204 No Content
        /// </example>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ActivityExists(int id)
        {
            return _context.Activities.Any(e => e.ActivityId == id);
        }
    }
}