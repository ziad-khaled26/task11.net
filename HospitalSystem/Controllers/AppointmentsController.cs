using HospitalSystem.Data;
using HospitalSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalSystem.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly AppDbContext _context;

        public AppointmentsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var appointments = _context.Appointments.Include(a => a.Doctor).ToList();
            return View(appointments);
        }

        [HttpGet]
        public IActionResult Book(int doctorId)
        {
            var doctor = _context.Doctors.Find(doctorId);
            if (doctor == null) return NotFound();

            ViewBag.DoctorName = doctor.Name;
            return View(new Appointment { DoctorId = doctorId, Date = DateTime.Today });
        }

        [HttpPost]
        public IActionResult Book(Appointment model)
        {
            if (model.Date.DayOfWeek == DayOfWeek.Friday || model.Date.DayOfWeek == DayOfWeek.Saturday)
            {
                ModelState.AddModelError("Date", "Available days are Sunday to Thursday only.");
            }

            // 2. Time Slots (9:00 AM -> 4:30 PM, 30 min slots)
            if (model.Time < new TimeSpan(9, 0, 0) || model.Time > new TimeSpan(16, 30, 0) || model.Time.Minutes % 30 != 0)
            {
                ModelState.AddModelError("Time", "Time must be between 9:00 AM and 4:30 PM (e.g., 9:00, 9:30).");
            }

            bool isBooked = _context.Appointments.Any(a =>
                a.DoctorId == model.DoctorId && a.Date == model.Date && a.Time == model.Time);

            if (isBooked)
            {
                ModelState.AddModelError("Time", "This time slot is already booked for this doctor.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.DoctorName = _context.Doctors.Find(model.DoctorId)?.Name;
                return View(model);
            }

            _context.Appointments.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}