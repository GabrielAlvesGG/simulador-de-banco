using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace simulador_de_banco.Controller
{
    public class HomeController : ControllerBase
    {
        // GET: HomeController
        public ActionResult Index()
        {
            return Ok();
        }

    }
}
