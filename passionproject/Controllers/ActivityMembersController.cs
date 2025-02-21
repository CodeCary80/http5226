using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using passionproject.Data;
using passionproject.Itinerary;
using passionproject.Models.Itinerary.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace passionproject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityMembersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ActivityMembersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ActivityMembers
        /// <summary>
        /// Returns all Activity-Member relationships
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{ActivityMember}]
        /// </returns>
        /// <example>
        /// GET: api/ActivityMembers ->
        /// 
        ///   {
        ///     relationship information,
        ///     activity: { activity information },
        ///     member: { member information }
        ///   }
        /// ]
        /// </example>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActivityMemberDTO>>> GetActivityMembers()
    {
        var activityMembers = await _context.ActivityMembers
           .Include(am => am.Activity)
           .Include(am => am.Member)
           .ToListAsync();

        return activityMembers.Select(am => new ActivityMemberDTO
        {
         ActivityId = am.ActivityId,
         MemberId = am.MemberId,
         IsOrganizer = am.IsOrganizer,
         Notes = am.Notes,
         Activity = new ActivityDTO
           {
            ActivityId = am.Activity.ActivityId,
            Name = am.Activity.Name,
            DateTime = am.Activity.DateTime,
            Location = am.Activity.Location,
            Description = am.Activity.Description,
            Cost = am.Activity.Cost,
            DestinationId = am.Activity.DestinationId
           },
         Member = new MemberDTO
           {
            MemberId = am.Member.MemberId,
            Name = am.Member.Name,
            Email = am.Member.Email,
            DietaryRestrictions = am.Member.DietaryRestrictions,
            HealthConsiderations = am.Member.HealthConsiderations,
            EmergencyContact = am.Member.EmergencyContact
            }
        }).ToList();
    }

        // GET: api/ActivityMembers/activity/{id}
        /// <summary>
        /// Returns a Activity-Member relationships by activity id
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{ActivityMember}]
        /// </returns>
        /// <example>
        /// GET: api/ActivityMembers ->
        /// 
        ///   {
        ///     relationship information,
        ///     activity: { activity information },
        ///     member: { member information }
        ///   }
        /// ]
        /// </example>
        [HttpGet("activity/{activityId}")]
         public async Task<ActionResult<IEnumerable<ActivityMemberDTO>>> GetActivityMembersByActivity(int activityId)
    {
         var activityMembers = await _context.ActivityMembers
            .Include(am => am.Member)
            .Where(am => am.ActivityId == activityId)
            .ToListAsync();

        return activityMembers.Select(am => new ActivityMemberDTO
      {
        ActivityId = am.ActivityId,
        MemberId = am.MemberId,
        IsOrganizer = am.IsOrganizer,
        Notes = am.Notes,
        Member = new MemberDTO
        {
            MemberId = am.Member.MemberId,
            Name = am.Member.Name,
            Email = am.Member.Email,
            DietaryRestrictions = am.Member.DietaryRestrictions,
            HealthConsiderations = am.Member.HealthConsiderations,
            EmergencyContact = am.Member.EmergencyContact
        }
       }).ToList();
    }

        // GET: api/ActivityMembers/member/{id}
        // GET: api/ActivityMembers/activity/{id}
        /// <summary>
        /// Returns a Activity-Member relationships by member id
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{ActivityMember}]
        /// </returns>
        /// <example>
        /// GET: api/ActivityMembers ->
        /// 
        ///   {
        ///     relationship information,
        ///     activity: { activity information },
        ///     member: { member information }
        ///   }
        /// ]
        /// </example>
        [HttpGet("member/{memberId}")]
        public async Task<ActionResult<IEnumerable<ActivityMember>>> GetActivityMembersByMember(int memberId)
    {
            return await _context.ActivityMembers
                .Include(am => am.Activity)
                .Where(am => am.MemberId == memberId)
                .ToListAsync();
     }

         // POST: api/ActivityMembers
        /// <summary>
        /// Creates a new Activity-Member relationship
        /// </summary>
        /// <param name="activityMember">The Activity-Member relationship to create</param>
        /// <returns>
        /// 201 Created
        /// 400 Bad Request
        /// </returns>
        /// <example>
        /// curl -X 'POST' \
        /// 'http://localhost:5074/api/ActivityMembers' \
        /// -H 'Content-Type: application/json' \
        /// -d '{
        ///   "activityId": 1,
        ///   "memberId": 1,
        ///   "isOrganizer": true,
        ///   "notes": "Tour guide contact needed"
        /// }'
        /// -> 201
        /// </example>
         [HttpPost]
         public async Task<ActionResult<ActivityMember>> PostActivityMember(ActivityMember activityMember)
     {
         // Verify that both Activity and Member exist
         var activity = await _context.Activities.FindAsync(activityMember.ActivityId);
         var member = await _context.Members.FindAsync(activityMember.MemberId);

         if (activity == null)
       {
        return BadRequest("Invalid ActivityId");
       }
         if (member == null)
       {
        return BadRequest("Invalid MemberId");
       }

        _context.ActivityMembers.Add(activityMember);
        try
       {
        await _context.SaveChangesAsync();
       }
        catch (DbUpdateException)
       {
        if (ActivityMemberExists(activityMember.ActivityId, activityMember.MemberId))
         {
            return Conflict();
         }
        else
         {
            throw;
         }
        }

         return CreatedAtAction("GetActivityMembers", new { }, activityMember);
     }

        /// PUT: api/ActivityMembers/5/2
        /// <summary>
        /// Updates an Activity-Member relationship
        /// </summary>
        /// <param name="activityId">The Activity ID</param>
        /// <param name="memberId">The Member ID</param>
        /// <param name="activityMember">Updated relationship information</param>
        /// <returns>
        /// 204 No Content
        /// 400 Bad Request
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// curl -X 'PUT' \
        /// 'http://localhost:5074/api/ActivityMembers/1/1' \
        /// -H 'Content-Type: application/json' \
        /// -d '{
        ///   "activityId": 1,
        ///   "memberId": 1,
        ///   "isOrganizer": true,
        ///   "notes": "Updated: Lead coordinator for activity"
        /// }'
        /// -> 204 No Content
        /// </example>
        [HttpPut("{activityId}/{memberId}")]
        public async Task<IActionResult> PutActivityMember(int activityId, int memberId, ActivityMember activityMember)
        {
            if (activityId != activityMember.ActivityId || memberId != activityMember.MemberId)
            {
                return BadRequest("Activity ID or Member ID mismatch");
            }

            // Check if the activity-member relationship exists
            var existingActivityMember = await _context.ActivityMembers
                .FindAsync(activityId, memberId);

            if (existingActivityMember == null)
            {
                return NotFound();
            }

            // Update the mutable properties
            existingActivityMember.IsOrganizer = activityMember.IsOrganizer;
            existingActivityMember.Notes = activityMember.Notes;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActivityMemberExists(activityId, memberId))
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

        // DELETE: api/ActivityMembers/5/{id}
        /// <summary>
        /// Deletes an Activity-Member relationship
        /// </summary>
        /// <param name="activityId">The Activity ID</param>
        /// <param name="memberId">The Member ID</param>
        /// <returns>
        /// 204 No Content
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// curl -X 'DELETE' 'http://localhost:5074/api/ActivityMembers/1/1' -> 204
        /// </example>
        [HttpDelete("{activityId}/{memberId}")]
        public async Task<IActionResult> DeleteActivityMember(int activityId, int memberId)
        {
            var activityMember = await _context.ActivityMembers
                .FindAsync(activityId, memberId);
            if (activityMember == null)
            {
                return NotFound();
            }

            _context.ActivityMembers.Remove(activityMember);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ActivityMemberExists(int activityId, int memberId)
        {
            return _context.ActivityMembers.Any(e => e.ActivityId == activityId && e.MemberId == memberId);
        }
    }
}