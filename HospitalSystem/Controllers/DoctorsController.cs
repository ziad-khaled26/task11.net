using HospitalSystem.Data;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSystem.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly AppDbContext _context;

        public DoctorsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchName, string specialization, int page = 1)
        {
            int pageSize = 4;
            var query = _context.Doctors.AsQueryable();

            if (!string.IsNullOrEmpty(searchName))
                query = query.Where(d => d.Name.Contains(searchName));
            if (!string.IsNullOrEmpty(specialization))
                query = query.Where(d => d.Specialization == specialization);

            var doctors = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            return View(doctors);
        }
    }
}