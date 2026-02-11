using Microsoft.AspNetCore.Mvc;
using MvcCoreProceduresEF.Models;
using MvcCoreProceduresEF.Repositories;

namespace MvcCoreProceduresEF.Controllers
{
    public class EnfermosController : Controller
    {
        private RepositoryEnfermo repo;

        public EnfermosController(RepositoryEnfermo repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            List<Enfermo> enfermos = await this.repo.GetEnfermosAsync();
            return View(enfermos);
        }
        public async Task<IActionResult> Details (string inscripcion)
        {
            Enfermo enfermo = await this.repo.FindEnfermoAsyc(inscripcion);
            return View(enfermo);
        }

        public async Task<IActionResult> Delete(string inscripcion)
        {
            await this.repo.DeleteEnfermoAsync(inscripcion);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> DeleteRaw(string inscripcion)
        {
            await this.repo.DeleteEnfermoRawAsync(inscripcion);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create (Enfermo e)
        {
            await this.repo.CreateEnfermoAsync( e.Apellido,  e.Direccion,  e.FechaNacimiento,  e.Genero,  e.Nss);
            return RedirectToAction("Index");
        }
    }
}

