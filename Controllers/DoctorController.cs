using Microsoft.AspNetCore.Mvc;
using MvcCoreProceduresEF.Models;
using MvcCoreProceduresEF.Repositories;
using System.Threading.Tasks;

namespace MvcCoreProceduresEF.Controllers
{
    public class DoctorController : Controller
    {

        private RepositoryDoctor repo;

        public DoctorController(RepositoryDoctor repo)
        {
            this.repo = repo;
        }
        public async Task<IActionResult> Index()
        {
            List<string> especialidades = await this.repo.GetEspecialidadesAsync();
            ViewData["ESPECIALIDADES"] = especialidades;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(string especialidad, int incremento , string metodo)
        {
            List<string> especialidades = await this.repo.GetEspecialidadesAsync();
            ViewData["ESPECIALIDADES"] = especialidades;
            if (metodo == "sp")
            {
                await this.repo.IncrementarSalarioRawAsync(especialidad, incremento);
            }
            else
            {
                await this.repo.IncrementarSalarioEntityFrameWorkAsync(especialidad, incremento);
            }
            List<Doctor> doctores = await this.repo.MostrarDoctoresEspecialidadAsync(especialidad);
            return View(doctores);
        }

    }
}
