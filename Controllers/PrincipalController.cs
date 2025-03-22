using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProjetoApiMVC.Models;

namespace ProjetoApiMVC.Controllers;

public class PrincipalController : Controller
{

    public IActionResult Index()
    {
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
