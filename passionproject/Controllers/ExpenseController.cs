using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using passionproject.Data;
using passionproject.Itinerary;
using passionproject.Models.Itinerary.DTOs;


namespace passionproject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpensesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExpensesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Expenses
        /// <summary>
        /// Returns a list of all Expenses with their Activity details and Member splits
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{Expense with splits and member details}]
        /// </returns>
        /// <example>
        /// curl -X 'GET' \'http://localhost:5074/api/Expenses' \ -> [{Expense, Activity, Splits with members}]
        /// </example>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseDTO>>> GetExpenses()
        {
            var expenses = await _context.Expenses
                .Include(e => e.Activity)
                .Include(e => e.ExpenseSplits)
                    .ThenInclude(es => es.Member)
                .ToListAsync();

            return expenses.Select(e => new ExpenseDTO
            {
                Id = e.Id,
                ActivityId = e.ActivityId,
                ActivityName = e.Activity.Name,
                Description = e.Description,
                Amount = e.Amount,
                Date = e.Date,
                Splits = e.ExpenseSplits.Select(es => new ExpenseSplitDTO
                {
                    MemberId = es.MemberId,
                    MemberName = es.Member.Name,
                    Share = es.Share,
                    IsPaid = es.IsPaid
                }).ToList()
            }).ToList();
        }

        // GET: api/Expenses/activity/1
        /// <summary>
        /// Returns all expenses for a specific activity with split details
        /// </summary>
        /// <param name="activityId">The specific Id of the Activity to get expenses for</param>
        /// <returns>
        /// 200 OK
        /// [{Expense}]
        /// </returns>
        /// <example>
        /// curl -X 'GET' \'http://localhost:5074/api/Expenses/activity/1' \ -> Expense with splits under the activityId:1
        /// </example>
        [HttpGet("activity/{activityId}")]
        public async Task<ActionResult<IEnumerable<ExpenseDTO>>> GetExpensesByActivity(int activityId)
        {
            var expenses = await _context.Expenses
                .Include(e => e.Activity)
                .Include(e => e.ExpenseSplits)
                    .ThenInclude(es => es.Member)
                .Where(e => e.ActivityId == activityId)
                .ToListAsync();

            return expenses.Select(e => new ExpenseDTO
            {
                Id = e.Id,
                ActivityId = e.ActivityId,
                ActivityName = e.Activity.Name,
                Description = e.Description,
                Amount = e.Amount,
                Date = e.Date,
                Splits = e.ExpenseSplits.Select(es => new ExpenseSplitDTO
                {
                    MemberId = es.MemberId,
                    MemberName = es.Member.Name,
                    Share = es.Share,
                    IsPaid = es.IsPaid
                }).ToList()
            }).ToList();
        }

        // POST: api/Expenses
        /// <summary>
        /// Creates a new expense with optional splits between members
        /// </summary>
        /// <param name="expenseDto">The expense information including ActivityId, Amount, and optional member splits</param>
        /// <returns>
        /// 201 Created
        /// </returns>
        /// <example>
        /// curl -X 'POST' \
        /// 'http://localhost:5074/api/Expenses' \
        /// -H 'Content-Type: application/json' \
        /// -d '{
        ///   "activityId": 1,
        ///   "description": "Group Dinner",
        ///   "amount": 100.00,
        ///   "date": "2024-07-02T19:00:00",
        ///   "splits": [
        ///     {"memberId": 1, "share": 33.50},
        ///     {"memberId": 2, "share": 66.50}
        ///   ]
        /// }'
        /// -> 201 Created 
        /// </example>
        [HttpPost]
        public async Task<ActionResult<Expense>> PostExpense(ExpenseCreateDTO expenseDto)
        {
            // Check if activity exists
            var activity = await _context.Activities.FindAsync(expenseDto.ActivityId);
            if (activity == null)
            {
                return BadRequest("Invalid ActivityId");
            }

            var expense = new Expense
            {
                ActivityId = expenseDto.ActivityId,
                Description = expenseDto.Description,
                Amount = expenseDto.Amount,
                Date = expenseDto.Date
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            if (expenseDto.Splits != null && expenseDto.Splits.Any())
            {
                var totalShares = expenseDto.Splits.Sum(s => s.Share);
                if (totalShares != expense.Amount)
                {
                    return BadRequest("Total shares must equal expense amount");
                }

                foreach (var split in expenseDto.Splits)
                {
                    var expenseSplit = new ExpenseSplit
                    {
                        ExpenseId = expense.Id,
                        MemberId = split.MemberId,
                        Share = split.Share,
                        IsPaid = false
                    };
                    _context.ExpenseSplits.Add(expenseSplit);
                }
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction("GetExpense", new { id = expense.Id }, expense);
        }

        // PUT: api/Expenses/5/markpaid/2
        /// <summary>
        /// Marks a member's expense split as paid
        /// </summary>
        /// <param name="expenseId">The ID of the Expense</param>
        /// <param name="memberId">The ID of the Member who paid</param>
        /// <returns>
        /// 204 No Content
        /// </returns>
        /// <example>
        /// curl -X 'PUT' \'http://localhost:5074/api/Expenses/1/markpaid/2' -> 204 No Content
        /// </example>
        [HttpPut("{expenseId}/markpaid/{memberId}")]
        public async Task<IActionResult> MarkExpensePaid(int expenseId, int memberId)
        {
            var expenseSplit = await _context.ExpenseSplits
                .FirstOrDefaultAsync(es => es.ExpenseId == expenseId && es.MemberId == memberId);

            if (expenseSplit == null)
            {
                return NotFound();
            }

            expenseSplit.IsPaid = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    
   
        // DELETE: api/Expenses/5
        /// <summary>
        /// Deletes expense splits
        /// </summary>
        /// <param name="id">The specific Id of the Expense to delete</param>
        /// <returns>
        /// 204 No Content
        /// </returns>
        /// <example>
        /// curl -X 'DELETE' \'http://localhost:5074/api/Expenses/1' -> 204 No Content
        /// </example>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var expense = await _context.Expenses
                    .Include(e => e.ExpenseSplits)
                    .FirstOrDefaultAsync(e => e.Id == id);

            if (expense == null)
            {
                return NotFound();
            }

            _context.ExpenseSplits.RemoveRange(expense.ExpenseSplits);
                
            _context.Expenses.Remove(expense);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Expenses/5/splits/2
        /// <summary>
        /// Removes a specific member's split from an expense
        /// </summary>
        /// <param name="expenseId">The specific Id of the Expense</param>
        /// <param name="memberId">The specific Id of the Member whose split should be removed</param>
        /// <returns>
        /// 204 No Content
        /// </returns>
        /// <example>
        /// curl -X 'DELETE' \'http://localhost:5074/api/Expenses/1/splits/2' -> 204 No Content
        /// </example>
        [HttpDelete("{expenseId}/splits/{memberId}")]
        public async Task<IActionResult> DeleteExpenseSplit(int expenseId, int memberId)
        {
            var expenseSplit = await _context.ExpenseSplits
                .FirstOrDefaultAsync(es => es.ExpenseId == expenseId && es.MemberId == memberId);

            if (expenseSplit == null)
            {
                return NotFound();
            }

            _context.ExpenseSplits.Remove(expenseSplit);
            await _context.SaveChangesAsync();

                return NoContent();
        }
                private bool ExpenseExists(int id)
                {
                    return _context.Expenses.Any(e => e.Id == id);
                }
}

}