using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using passionproject.Data;
using passionproject.Itinerary;
using passionproject.Models.Itinerary.DTOs;

namespace passionproject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MembersController(ApplicationDbContext context)
        {
            _context = context;
        }
        
         // GET: api/Members
        /// <summary>
        /// Returns a list of all Members
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{Member}]
        /// </returns>
        /// <example>
        /// GET: api/Members -> 
        /// [
        ///   {
        ///     member information,
        ///     activities: [
        ///       {
        ///         activity information,
        ///         destination: {
        ///           destination information
        ///         }
        ///       }
        ///     ]
        ///   } 
        /// ]
        /// -> 200 ok
        /// </example>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetMembers()
     {
       var members = await _context.Members
        .Include(m => m.ActivityMembers)
            .ThenInclude(am => am.Activity)
                .ThenInclude(a => a.Destination)
        .ToListAsync();

        return members.Select(member => new
       {
        member.MemberId,
        member.Name,
        member.Email,
        member.DietaryRestrictions,
        member.HealthConsiderations,
        member.EmergencyContact,
        Activities = member.ActivityMembers.Select(am => new
          {
            am.Activity.ActivityId,
            am.Activity.Name,
            am.Activity.DateTime,
            am.Activity.Location,
            am.Activity.Description,
            am.Activity.Cost,
            am.IsOrganizer,
            am.Notes,
            Destination = new
              {
                am.Activity.Destination.DestinationId,
                am.Activity.Destination.Name,
                am.Activity.Destination.Location
              }
           }).ToList()
        }).ToList();
}
        // GET: api/Members/{id}
        /// <summary>
        /// Returns a specific Members by ID
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{Member}]
        /// </returns>
        /// <example>
        /// GET: api/Members -> 
        /// [
        ///   {
        ///     member information,
        ///     activities: [
        ///       {
        ///         activity information,
        ///         destination: {
        ///           destination information
        ///         }
        ///       }
        ///     ]
        ///   } 
        /// ]
        /// -> 200 ok
        /// </example>

        [HttpGet("{id}")]
        public async Task<ActionResult<MemberDTO>> GetMember(int id)
    {
        var member = await _context.Members
        .Include(m => m.ActivityMembers)
            .ThenInclude(am => am.Activity)
                .ThenInclude(a => a.Destination)
        .FirstOrDefaultAsync(m => m.MemberId == id);

        if (member == null)
       {
        return NotFound();
       }

        return new MemberDTO
       {
        MemberId = member.MemberId,
        Name = member.Name,
        Email = member.Email,
        DietaryRestrictions = member.DietaryRestrictions,
        HealthConsiderations = member.HealthConsiderations,
        EmergencyContact = member.EmergencyContact,
        Activities = member.ActivityMembers.Select(am => new ActivityDetailDTO
          {
            ActivityId = am.Activity.ActivityId,
            Name = am.Activity.Name,
            DateTime = am.Activity.DateTime,
            Location = am.Activity.Location,
            Description = am.Activity.Description,
            Cost = am.Activity.Cost,
            IsOrganizer = am.IsOrganizer,
            Notes = am.Notes,
            Destination = new DestinationSummaryDTO
            {
                DestinationId = am.Activity.Destination.DestinationId,
                Name = am.Activity.Destination.Name,
                Location = am.Activity.Destination.Location
            }
          }).ToList()
        };
}
        // POST: api/Members
        /// <summary>
        /// Creates a new Member in the database
        /// </summary>
        /// <param name="member">The Member information to add</param>
        /// <returns>
        /// 201 Created {Member}
        /// </returns>
        /// <example>
        /// curl -X 'POST' \
        /// 'http://localhost:5074/api/Members' \
        /// -H 'Content-Type: application/json' \
        /// -d '{
        ///   "name": "John Smith",
        ///   "email": "john@email.com",
        ///   "dietaryRestrictions": "No seafood",
        ///   "healthConsiderations": "None",
        ///   "emergencyContact": "123-456-7890"
        /// }'
        /// -> 201 Created {Member}
        /// </example>
        [HttpPost]
        public async Task<ActionResult<Member>> PostMember(Member member)
        {
            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMember), new { id = member.MemberId }, member);
        }

        // PUT: api/Members/5
        /// <summary>
        /// Updates a Member in the database
        /// </summary>
        /// <param name="id">The ID of the Member to update</param>
        /// <param name="member">Updated member information</param> 
        /// <returns>
        /// 204 No Content
        /// 400 Bad Request
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// curl -X 'PUT' \
        /// 'http://localhost:5074/api/Members/1' \
        /// -H 'Content-Type: application/json' \
        /// -d '{
        ///   "memberId": 1,
        ///   "name": "John Smith Updated",
        ///   "email": "john.updated@email.com",
        ///   "dietaryRestrictions": "No seafood, no nuts",
        ///   "healthConsiderations": "None",
        ///   "emergencyContact": "123-456-7890"
        /// }'
        /// -> 204
        /// </example>  
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMember(int id, Member member)
    {
            if (id != member.MemberId)
            {
                return BadRequest();
            }

            _context.Entry(member).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(id))
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

        // DELETE: api/Members/5
        /// <summary>
        /// Deletes a Member from the database
        /// </summary>
        /// <param name="id">The ID of the Member to delete</param>
        /// <returns>
        /// 204 No Content
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// curl -X 'DELETE' 'http://localhost:5074/api/Members/1' -> 204
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(int id)
    {
            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            return NoContent();
    }

        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.MemberId == id);
        }
    }
}