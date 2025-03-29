using Microsoft.AspNetCore.Mvc;
using ProjetoApiMVC.Models;

namespace ProjetoApiMVC.Controllers;
public class SalvaRedesSociaisController : Controller
{


    public SalvaRedesSociaisController()
    {

    }
    [HttpPost("/api/salvaredes")]
    public IActionResult SalvarRedesSociais([FromBody] RedesSociaisRequest redesSociais)
    {
        Console.WriteLine(redesSociais.RedesSociais);
        if (redesSociais == null || redesSociais.RedesSociais == null)
        {
            return BadRequest("Dados inválidos.");
        }

        
        SalvaRedesSociaisNoBanco redes = new SalvaRedesSociaisNoBanco();
        bool resultado = redes.SalvarRedes(redesSociais.Id, redesSociais.RedesSociais);

        if (resultado)
        {
            return Ok("Redes sociais salvas com sucesso.");
        }
        else
        {
            return StatusCode(500, "Erro ao salvar as redes sociais.");
        }
    } 

}
public class RedesSociaisRequest
{
    public long Id { get; set; }
    public Dictionary<string, List<string>> RedesSociais { get; set; }
}