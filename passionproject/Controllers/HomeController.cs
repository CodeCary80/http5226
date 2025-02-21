using Microsoft.AspNetCore.Mvc;
using passionproject.Data;
using passionproject.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using passionproject.Itinerary; 

namespace passionproject.Controllers
{
    [Authorize] 
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Destination Management
        /// <summary>
        /// Displays list of all destinations with their associating activities and members inside (if existing)
        /// </summary>
        /// <returns>View with list of destinations</returns>
         public async Task<IActionResult> Index()
       {
          var destinations = await _context.Destinations
            .Include(d => d.Activities)
                .ThenInclude(a => a.ActivityMembers)
                    .ThenInclude(am => am.Member)
            .Select(d => new DestinationViewModel
            {
                DestinationId = d.DestinationId,
                Name = d.Name,
                Location = d.Location,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                Description = d.Description,
                Budget = d.Budget,
                Activities = d.Activities.Select(a => new ActivityViewModel
                {
                    ActivityId = a.ActivityId,
                    Name = a.Name,
                    DateTime = a.DateTime,
                    Location = a.Location,
                    Description = a.Description,
                    Cost = a.Cost,

                    Members = a.ActivityMembers.Select(am => new MemberViewModel
                {
                    MemberId = am.Member.MemberId,
                    Name = am.Member.Name,
                    Email = am.Member.Email,
                    IsOrganizer = am.IsOrganizer,
                    Notes = am.Notes
                   }).ToList()
                }).ToList()
            })
               .ToListAsync();

             return View(destinations);
        }
        /// <summary>
        /// Shows detailed view of a specific destination including all activities (if existing)
        /// </summary>
        /// <param name="id">Destination Id</param>
        /// <returns>View with destination details</returns>
        public async Task<IActionResult> DestinationDetails(int id)
        {
            var destination = await _context.Destinations
                .Include(d => d.Activities)
                    .ThenInclude(a => a.ActivityMembers)
                        .ThenInclude(am => am.Member)
                .FirstOrDefaultAsync(d => d.DestinationId == id);

            if (destination == null)
            {
                return NotFound();
            }

            var viewModel = new DestinationViewModel
            {
                DestinationId = destination.DestinationId,
                Name = destination.Name,
                Location = destination.Location,
                StartDate = destination.StartDate,
                EndDate = destination.EndDate,
                Description = destination.Description,
                Budget = destination.Budget,
                Activities = destination.Activities.Select(a => new ActivityViewModel
                {
                        ActivityId = a.ActivityId,
                        Name = a.Name,
                        DateTime = a.DateTime,
                        Location = a.Location,
                        Description = a.Description,
                        Cost = a.Cost,
                        Members = a.ActivityMembers.Select(am => new MemberViewModel
                        {
                            MemberId = am.Member.MemberId,
                            Name = am.Member.Name,
                            Email = am.Member.Email,
                            IsOrganizer = am.IsOrganizer,
                            Notes = am.Notes
                        }).ToList()
                    }).ToList()
            };

            return View(viewModel);
        }

            /// <summary>
            /// Displays form to create a new destination
            /// </summary>
            [HttpGet]
            public IActionResult Create()
            {
                ViewBag.AllMembers = _context.Members.ToList();
                return View();
            }
            
            /// <summary>
            /// Handles submission of new destination form
            /// </summary>
            /// <param name="model">Destination data from form</param>
            [HttpPost]
            public async Task<IActionResult> Create(DestinationViewModel model)
            {
                if (ModelState.IsValid)
                {
                    var destination = new Destination
                    {
                        Name = model.Name,
                        Location = model.Location,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        Description = model.Description,
                        Budget = model.Budget
                    };

                        _context.Destinations.Add(destination);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                }
                return View(model);
            }


            // Destination Management
            /// <summary>
            /// Displays form to edit an existing destination
            /// </summary>
            /// <param name="id">Destination ID to edit</param>
            [HttpGet]
            public async Task<IActionResult> Edit(int id)
            {
                var destination = await _context.Destinations.FindAsync(id);
                if (destination == null)
                {
                    return NotFound();
                }

                ViewBag.AllMembers = await _context.Members.ToListAsync();
                    
                var viewModel = new DestinationViewModel
                {
                    DestinationId = destination.DestinationId,
                    Name = destination.Name,
                    Location = destination.Location,
                    StartDate = destination.StartDate,
                    EndDate = destination.EndDate,
                    Description = destination.Description,
                    Budget = destination.Budget
                };

                return View(viewModel);
            }

            /// <summary>
            /// Handles update of existing destination
            /// </summary>
            /// <param name="id">Destination Id</param>
            /// <param name="model">Updated destination data</param>
            [HttpPost]
            public async Task<IActionResult> Edit(int id, DestinationViewModel model)
            {
                if (id != model.DestinationId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        var destination = await _context.Destinations.FindAsync(id);
                        if (destination == null)
                        {
                            return NotFound();
                        }

                        destination.Name = model.Name;
                        destination.Location = model.Location;
                        destination.StartDate = model.StartDate;
                        destination.EndDate = model.EndDate;
                        destination.Description = model.Description;
                        destination.Budget = model.Budget;

                        _context.Update(destination);
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
                        return RedirectToAction(nameof(Index));
                }
                    return View(model);
            }

                private bool DestinationExists(int id)
            {
                    return _context.Destinations.Any(e => e.DestinationId == id);
            }

            /// <summary>
            /// Shows confirmation page for deleting a destination
            /// </summary>
            /// <param name="id">Destination Id to delete</param>
            [HttpGet]
            public async Task<IActionResult> Delete(int id)
            {
                    var destination = await _context.Destinations
                        .FirstOrDefaultAsync(d => d.DestinationId == id);
                    
                    if (destination == null)
                    {
                        return NotFound();
                    }

                    var viewModel = new DestinationViewModel
                    {
                        DestinationId = destination.DestinationId,
                        Name = destination.Name,
                        Location = destination.Location,
                        StartDate = destination.StartDate,
                        EndDate = destination.EndDate,
                        Description = destination.Description,
                        Budget = destination.Budget
                    };

                    return View(viewModel);
            }

            /// <summary>
            /// Handles deletion of destination and its related activities/members
            /// </summary>
            /// <param name="id">Destination Id to delete</param>
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                    var destination = await _context.Destinations
                        .Include(d => d.Activities)
                            .ThenInclude(a => a.ActivityMembers)
                        .FirstOrDefaultAsync(d => d.DestinationId == id);

                    if (destination == null)
                    {
                        return NotFound();
                    }

                    _context.Destinations.Remove(destination);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
            }


            // Activity Management
            /// <summary>
            /// Shows form to create a new activity for a destination
            /// </summary>
            /// <param name="destinationId">Id of destination to add activity to</param>
            [HttpGet]
            public IActionResult CreateActivity(int destinationId)
            {
                    ViewBag.DestinationId = destinationId;
                    return View();
            }

            // Activity Management
            /// <summary>
            /// Creates new activity for a destination
            /// </summary>
            /// <param name="destinationId">Destination Id</param>
            /// <param name="model">Activity data</param>
            [HttpPost]
            public async Task<IActionResult> CreateActivity(int destinationId, ActivityViewModel model)
            {
                    Console.WriteLine($"Received destinationId: {destinationId}");
                    Console.WriteLine($"Model state valid: {ModelState.IsValid}");

                    if (ModelState.IsValid)
                    {
                        try
                        {
                            var activity = new Activity
                            {
                                DestinationId = destinationId,
                                Name = model.Name,
                                DateTime = model.DateTime,
                                Location = model.Location,
                                Description = model.Description,
                                Cost = model.Cost
                            };

                            _context.Activities.Add(activity);
                            await _context.SaveChangesAsync();
                           
                            Console.WriteLine($"Activity saved with ID: {activity.ActivityId}");

                            return RedirectToAction(nameof(DestinationDetails), new { id = destinationId });
                        }
                        catch (Exception ex)
                        {   
                            Console.WriteLine($"Error saving activity: {ex.Message}");
                            ModelState.AddModelError("", "Error saving activity: " + ex.Message);
                        }
                    }
                    else
                    {
                        foreach (var modelState in ModelState.Values)
                        {
                            foreach (var error in modelState.Errors)
                            {
                                Console.WriteLine($"Validation error: {error.ErrorMessage}");
                            }
                        }
                    }

                    ViewBag.DestinationId = destinationId;
                    return View(model);
            }

            /// <summary>
            /// Displays form to edit an activity
            /// </summary>
            /// <param name="id">Activity Id to edit</param>
            [HttpGet]
            public async Task<IActionResult> EditActivity(int id)
            {
                    var activity = await _context.Activities.FindAsync(id);
                    if (activity == null)
                    {
                        return NotFound();
                    }

                    var viewModel = new ActivityViewModel
                    {
                        ActivityId = activity.ActivityId,
                        Name = activity.Name,
                        DateTime = activity.DateTime,
                        Location = activity.Location,
                        Description = activity.Description,
                        Cost = activity.Cost
                    };

                    return View(viewModel);
            }

            /// <summary>
            /// Handles update of existing activity
            /// </summary>
            /// <param name="id">Activity Id</param>
            /// <param name="model">Updated activity data</param>
            [HttpPost]
            public async Task<IActionResult> EditActivity(int id, ActivityViewModel model)
            {
                    if (id != model.ActivityId)
                    {
                        return NotFound();
                    }

                    if (ModelState.IsValid)
                    {
                        var activity = await _context.Activities.FindAsync(id);
                        if (activity == null)
                        {
                            return NotFound();
                        }

                        activity.Name = model.Name;
                        activity.DateTime = model.DateTime;
                        activity.Location = model.Location;
                        activity.Description = model.Description;
                        activity.Cost = model.Cost;

                        _context.Update(activity);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(DestinationDetails), new { id = activity.DestinationId });
                    }
                    return View(model);
            }

            /// <summary>
            /// Shows confirmation page for deleting an activity
            /// </summary>
            /// <param name="id">Activity Id to delete</param>
            [HttpGet]
            public async Task<IActionResult> DeleteActivity(int id)
            {
                    var activity = await _context.Activities
                        .Include(a => a.Destination)
                        .FirstOrDefaultAsync(a => a.ActivityId == id);

                    if (activity == null)
                    {
                        return NotFound();
                    }

                    return View(activity);
            }

            /// <summary>
            /// Handles deletion of activity
            /// </summary>
            /// <param name="id">Activity Id to delete</param>
            [HttpPost, ActionName("DeleteActivity")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteActivityConfirmed(int id)
            {
                    var activity = await _context.Activities.FindAsync(id);
                    if (activity == null)
                    {
                        return NotFound();
                    }

                    var destinationId = activity.DestinationId;
                    _context.Activities.Remove(activity);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(DestinationDetails), new { id = destinationId });
            }


            // Member Management
            /// <summary>
            /// Adds a member to an activity with specified role
            /// </summary>
            /// <param name="activityId">Activity Id</param>
            /// <param name="memberId">Member Id</param>
            /// <param name="isOrganizer">Whether member is organizer</param>
            /// <param name="notes">Additional notes</param>
            [HttpPost]
            public async Task<IActionResult> AddMember(int activityId, int memberId, bool isOrganizer, string notes)
            {
                    var activityMember = new ActivityMember
                    {
                        ActivityId = activityId,
                        MemberId = memberId,
                        IsOrganizer = isOrganizer,
                        Notes = notes
                    };

                    _context.ActivityMembers.Add(activityMember);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(ActivityDetails), new { id = activityId });
            }

            // Member Management
            /// <summary>
            /// Removes a member from an activity
            /// </summary>
            /// <param name="activityId">Activity Id to delete</param>
            /// <param name="memberId">member Id to delete</param>
            [HttpPost]
            public async Task<IActionResult> RemoveMember(int activityId, int memberId)
            {
                    var activityMember = await _context.ActivityMembers
                        .FirstOrDefaultAsync(am => am.ActivityId == activityId && am.MemberId == memberId);

                    if (activityMember != null)
                    {
                        _context.ActivityMembers.Remove(activityMember);
                        await _context.SaveChangesAsync();
                    }

                    return RedirectToAction(nameof(ActivityDetails), new { id = activityId });
            }

            /// <summary>
            /// Shows detailed view of an activity including members
            /// </summary>
            /// <param name="id">Activity Id</param>
            public async Task<IActionResult> ActivityDetails(int id)
            {
                    var activity = await _context.Activities
                        .Include(a => a.ActivityMembers)
                            .ThenInclude(am => am.Member)
                        .Include(a => a.Destination)
                        .FirstOrDefaultAsync(a => a.ActivityId == id);

                    if (activity == null)
                    {
                        return NotFound();
                    }

                 
                    var allMembers = await _context.Members.ToListAsync();

                
                    var existingMemberIds = activity.ActivityMembers.Select(am => am.MemberId).ToList();
                    var availableMembers = allMembers.Where(m => !existingMemberIds.Contains(m.MemberId)).ToList();

                    ViewBag.AvailableMembers = availableMembers;
                    ViewBag.DestinationId = activity.DestinationId;

                    var viewModel = new ActivityViewModel
                    {
                        ActivityId = activity.ActivityId,
                        Name = activity.Name,
                        DateTime = activity.DateTime,
                        Location = activity.Location,
                        Description = activity.Description,
                        Cost = activity.Cost,
                        Members = activity.ActivityMembers.Select(am => new MemberViewModel
                        {
                            MemberId = am.Member.MemberId,
                            Name = am.Member.Name,
                            Email = am.Member.Email,
                            DietaryRestrictions = am.Member.DietaryRestrictions,
                            HealthConsiderations = am.Member.HealthConsiderations,
                            EmergencyContact = am.Member.EmergencyContact,
                            IsOrganizer = am.IsOrganizer,
                            Notes = am.Notes
                        }).ToList()
                    };

                    return View(viewModel);
            }


            /// <summary>
            /// Updates a member's role and notes for an activity
            /// </summary>
            /// <param name="activityId">Activity Id to edit</param>
            /// <param name="memberId">member Id to edit</param>
            /// <param name="isOrganizer">Assign role</param>
            /// <param name="notes">remark relationship or other notes</param>
            [HttpPost]
            public async Task<IActionResult> EditMember(int activityId, int memberId, bool isOrganizer, string notes)
            {
                    var activityMember = await _context.ActivityMembers
                        .FirstOrDefaultAsync(am => am.ActivityId == activityId && am.MemberId == memberId);

                    if (activityMember == null)
                    {
                        return NotFound();
                    }

                    activityMember.IsOrganizer = isOrganizer;
                    activityMember.Notes = notes;

                    try
                    {
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(ActivityDetails), new { id = activityId });
                    }
                    catch (DbUpdateConcurrencyException)
                    {

                        return RedirectToAction(nameof(ActivityDetails), new { id = activityId });
                    }
            }
                

            /// <summary>
            /// Creates a new member in the system
            /// </summary>
            public IActionResult CreateMember()
            {
                    ViewBag.ActivityId = TempData["ActivityId"]; 
                    return View();
            }

            [HttpPost]
            public async Task<IActionResult> CreateMember(Member member)
            {
                    if (ModelState.IsValid)
                    {
                        _context.Members.Add(member);
                        await _context.SaveChangesAsync();

                        if (TempData["ActivityId"] != null)
                        {
                            return RedirectToAction(nameof(ActivityDetails), 
                                new { id = TempData["ActivityId"] });
                        }

                        return RedirectToAction(nameof(Index));
                    }
                    return View(member);
            }


            /// <summary>
            /// Updates full member information including activity-specific details
            /// </summary>
            /// <param name="activityId">Activity Id to edit</param>
            /// <param name="memberId">Member Id to edit</param>
            /// <param name="name">Name to edit</param>
            /// <param name="email">Email to edit</param>
            /// <param name="dietaryRestrictions">DietaryRestrictions to edit</param>
            /// <param name="healthConsiderations">HealthConsiderations to edit</param>
            /// <param name="emergencyContact">EmergencyContact to edit</param>
            /// <param name="isOrganizer">Assign roles</param>
            /// <param name="notes">Relationship for management</param>         
            [HttpPost]
            public async Task<IActionResult> EditMemberFull(int activityId, int memberId, string name, 
                    string email, string dietaryRestrictions, string healthConsiderations, 
                    string emergencyContact, bool isOrganizer, string notes)
            {
                    var member = await _context.Members.FindAsync(memberId);
                    if (member == null)
                    {
                        return NotFound();
                    }

                    member.Name = name;
                    member.Email = email;
                    member.DietaryRestrictions = dietaryRestrictions;
                    member.HealthConsiderations = healthConsiderations;
                    member.EmergencyContact = emergencyContact;

                    var activityMember = await _context.ActivityMembers
                        .FirstOrDefaultAsync(am => am.ActivityId == activityId && am.MemberId == memberId);
                    if (activityMember != null)
                    {
                        activityMember.IsOrganizer = isOrganizer;
                        activityMember.Notes = notes;
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(ActivityDetails), new { id = activityId });
            }
                
    }
   
}