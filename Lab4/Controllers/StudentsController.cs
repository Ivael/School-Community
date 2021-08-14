using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab4.Data;
using Lab4.Models.ViewModels;
using Lab4.Models;

namespace Lab4.Controllers
{
    public class StudentsController : Controller
    {
        private readonly SchoolCommunityContext _context;

        public StudentsController(SchoolCommunityContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string id)
        {
            var mod = new CommunityViewModel();

            mod.Students = await _context.Students.Include(x => x.Membership).ThenInclude(membership => membership.Community)
                 .AsNoTracking().OrderBy(x => x.id.ToString()).ToListAsync();

            if (id != null)
            {

                mod.Memberships = mod.Students.Where(x => x.id.ToString() == id).Single().Membership;
            }

            return View(mod);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }


        public IActionResult Create()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,LastName,FirstName,Enrollmentdate")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,LastName,FirstName,Enrollmentdate")] Student student)
        {
            if (id != student.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.id))
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
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.id == id);
        }
        public async Task<IActionResult> EditMemberships(int id)
        {
            CommunityViewModel communityViewModel = new CommunityViewModel();
            communityViewModel.Memberships = await _context.CommunityMemberships.Where(i => i.StudentID == id)
                     .ToListAsync();
            communityViewModel.Students = await _context.Students.Where(i => i.id == id)
                   .ToListAsync();

            communityViewModel.Communities = await _context.Communities
                  .ToListAsync();


            return View(communityViewModel);
        }
        public async Task<IActionResult> AddMemberships(int studentId, string communityId)
        {
            CommunityMembership cs = new CommunityMembership();
            cs.CommunityID = communityId;
            cs.StudentID = studentId;
            _context.CommunityMemberships.Add(cs);
            await _context.SaveChangesAsync();

            return RedirectToAction("EditMemberships", new { id = studentId });
        }

        public async Task<IActionResult> RemoveMemberships(int studentId, string communityId)
        {
            CommunityMembership cs = new CommunityMembership();
            cs.CommunityID = communityId;
            cs.StudentID = studentId;
            _context.CommunityMemberships.Remove(cs);
            await _context.SaveChangesAsync();

            return RedirectToAction("EditMemberships", new { id = studentId });
        }

    }
}
