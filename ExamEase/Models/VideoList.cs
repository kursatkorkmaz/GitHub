using Microsoft.AspNetCore.Mvc;

namespace ExamEase.Models
{
    public class VideoList : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
