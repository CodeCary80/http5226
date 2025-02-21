using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using passionproject.Data;
using passionproject.Itinerary;
using passionproject.Models.Itinerary.DTOs;

namespace passionproject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityRatingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ActivityRatingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ActivityRatings
        /// <summary>
        /// Returns a list of all Activity Ratings with their Activity and Member details
        /// </summary>
        /// <returns>
        /// 200 OK 
        /// [{ActivityRating}]
        /// </returns>
        /// <example>
        /// curl -X 'GET' \'http://localhost:5074/api/ActivityRatings' \ -> 200 OK [{Rating info, Activity info, Member info}]
        /// </example>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActivityRatingDTO>>> GetRatings()
        {
            var ratings = await _context.ActivityRatings
                .Include(r => r.Activity)
                .Include(r => r.Member)
                .ToListAsync();

            return ratings.Select(r => new ActivityRatingDTO
            {
                Id = r.Id,
                ActivityId = r.ActivityId,
                ActivityName = r.Activity.Name,
                MemberId = r.MemberId,
                MemberName = r.Member.Name,
                Rating = r.Rating,
                Comment = r.Comment
            }).ToList();
        }

        // GET: api/ActivityRatings/activity/1
        /// <summary>
        /// Returns all ratings for a specific activity with member details
        /// </summary>
        /// <param name="activityId">The ID of the Activity to get ratings for</param>
        /// <returns>
        /// 200 OK
        /// [{ActivityRating}]
        /// </returns>
        /// <example>
        /// curl -X 'GET' \'http://localhost:5074/api/ActivityRatings/activity/1' \ -> 200 OK acivityId:1's Rating info and Member info
        /// </example>
        [HttpGet("activity/{activityId}")]
        public async Task<ActionResult<IEnumerable<ActivityRatingDTO>>> GetActivityRatings(int activityId)
        {
            var ratings = await _context.ActivityRatings
                .Include(r => r.Activity)
                .Include(r => r.Member)
                .Where(r => r.ActivityId == activityId)
                .ToListAsync();

            return ratings.Select(r => new ActivityRatingDTO
            {
                Id = r.Id,
                ActivityId = r.ActivityId,
                ActivityName = r.Activity.Name,
                MemberId = r.MemberId,
                MemberName = r.Member.Name,
                Rating = r.Rating,
                Comment = r.Comment
            }).ToList();
        }

        // POST: api/ActivityRatings
        /// <summary>
        /// Creates a new rating for an activity by a member
        /// </summary>
        /// <param name="ratingDto">The rating information including ActivityId, MemberId, Rating (1-5), and Comment</param>
        /// <returns>
        /// 201 Created 
        /// </returns>
        /// <example>
        /// curl -X 'POST' \
        /// 'http://localhost:5074/api/ActivityRatings' \
        /// -H 'Content-Type: application/json' \
        /// -d '{
        ///   "activityId": 1,
        ///   "memberId": 1,
        ///   "rating": 5,
        ///   "comment": "Great memory!"
        /// }'
        /// -> 201 Created {ActivityRating:5}
        /// </example>
        [HttpPost]
        public async Task<ActionResult<ActivityRatingDTO>> PostRating(ActivityRatingCreateDTO ratingDto)
        {
            // Validate rating value
            if (ratingDto.Rating < 1 || ratingDto.Rating > 5)
            {
                return BadRequest("Rating must be between 1 and 5");
            }

            // Check if activity exists
            var activity = await _context.Activities.FindAsync(ratingDto.ActivityId);
            if (activity == null)
            {
                return BadRequest("Invalid ActivityId");
            }

            // Check if member exists
            var member = await _context.Members.FindAsync(ratingDto.MemberId);
            if (member == null)
            {
                return BadRequest("Invalid MemberId");
            }

            // Check if member has already rated this activity
            var existingRating = await _context.ActivityRatings
                .FirstOrDefaultAsync(r => r.ActivityId == ratingDto.ActivityId && r.MemberId == ratingDto.MemberId);

            if (existingRating != null)
            {
                return BadRequest("Member has already rated this activity");
            }

            var rating = new ActivityRating
            {
                ActivityId = ratingDto.ActivityId,
                MemberId = ratingDto.MemberId,
                Rating = ratingDto.Rating,
                Comment = ratingDto.Comment
            };

            _context.ActivityRatings.Add(rating);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetActivityRatings), 
                new { activityId = rating.ActivityId }, 
                new ActivityRatingDTO
                {
                    Id = rating.Id,
                    ActivityId = rating.ActivityId,
                    ActivityName = activity.Name,
                    MemberId = rating.MemberId,
                    MemberName = member.Name,
                    Rating = rating.Rating,
                    Comment = rating.Comment
                });
        }
        // PUT: api/ActivityRatings/5
        /// <summary>
        /// Updates an existing rating
        /// </summary>
        /// <param name="id">The ID of the Rating to update</param>
        /// <param name="ratingDto">Updated rating information including Rating (1-5) and Comment</param>
        /// <returns>
        /// 204 No Content
        /// </returns>
        /// <example>
        /// curl -X 'PUT' \
        /// 'http://localhost:5074/api/ActivityRatings/1' \
        /// -H 'Content-Type: application/json' \
        /// -d '{
        ///   "activityId": 1,
        ///   "memberId": 1,
        ///   "rating": 4,
        ///   "comment": "Updated: Homebound due to the snowstorm"
        /// }'
        /// -> 204 No Content
        /// </example>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRating(int id, ActivityRatingCreateDTO ratingDto)
        {
        
            if (ratingDto.Rating < 1 || ratingDto.Rating > 5)
            {
                return BadRequest("Rating must be between 1 and 5");
            }

        
            var rating = await _context.ActivityRatings
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rating == null)
            {
                return NotFound();
            }
        
            var activity = await _context.Activities.FindAsync(ratingDto.ActivityId);
            if (activity == null)
            {
                return BadRequest("Invalid ActivityId");
            }

            var member = await _context.Members.FindAsync(ratingDto.MemberId);
            if (member == null)
            {
                return BadRequest("Invalid MemberId");
            }

            rating.Rating = ratingDto.Rating;
            rating.Comment = ratingDto.Comment;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RatingExists(id))
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

        private bool RatingExists(int id)
        {
            return _context.ActivityRatings.Any(e => e.Id == id);
        }

        // DELETE: api/ActivityRatings/5
        /// <summary>
        /// Deletes a rating from the database
        /// </summary>
        /// <param name="id">The ID of the Rating to delete</param>
        /// <returns>
        /// 204 No Content
        /// </returns>
        /// <example>
        /// curl -X 'DELETE' 'http://localhost:5074/api/ActivityRatings/1' -> 204 No Content
        /// </example>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRating(int id)
        {
            var rating = await _context.ActivityRatings.FindAsync(id);
            if (rating == null)
            {
                return NotFound();
            }

            _context.ActivityRatings.Remove(rating);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}