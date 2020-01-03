using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PrimeCollaborationManager.Data;

namespace PrimeCollaborationManager.Controllers
{
    [Authorize]
    public class TeamsController : Controller
    {
        private readonly AppDbContext _context;
        public TeamsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Teams
        public async Task<IActionResult> Index()
        {
            var userId = Helpers.UserHelper.GetBBUserId(HttpContext);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.BBUserId == userId);
            if (user == null)
            {
                _context.Users.Add(new User() { Email = Helpers.UserHelper.GetEmail(HttpContext), BBUserId = userId, TeamLimit = 5, TeamMemberLimit = 5 });
                _context.SaveChanges();
            }
            var teams = await _context.Teams.Where(t => t.UserId == user.UserId).ToListAsync();
            return View(teams);
        }

        // GET: Teams/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = (await GetCurrentUser()).UserId;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.BBUserId == userId);

            var team = await _context.Teams
                .FirstOrDefaultAsync(m => m.TeamId == id && m.UserId == user.UserId);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // GET: Teams/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teams/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Team team)
        {
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUser();
                team.UserId = user.UserId;
                _context.Add(team);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(team);
        }

        // GET: Teams/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userId = (await GetCurrentUser()).UserId;
            var team = await _context.Teams.FindAsync(id);
            if (team == null || team.UserId != userId)
            {
                return NotFound();
            }

            return View(team);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Name")] Team team)
        {
            if (id != team.TeamId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(team);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamExists(team.TeamId))
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
            return View(team);
        }

        // GET: Teams/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.FirstOrDefaultAsync(m => m.TeamId == id);

            var userId = (await GetCurrentUser()).UserId;
            if (team == null || team.UserId != userId)
            {
                return NotFound();
            }

            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var team = await _context.Teams.FindAsync(id);
            var userId = (await GetCurrentUser()).UserId;
            if (team == null || team.UserId != userId)
            {
                return NotFound();
            }

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeamExists(long id)
        {
            return _context.Teams.Any(e => e.TeamId == id);
        }

        private async Task<User> GetCurrentUser()
        {
            var userId = Helpers.UserHelper.GetBBUserId(HttpContext);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.BBUserId == userId);
            return user;
        }
    }
}
