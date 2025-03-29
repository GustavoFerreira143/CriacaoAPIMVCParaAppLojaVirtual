using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetoApiMVC.Models;
using System.Security.Claims;

namespace ProjetoApiMVC.Controllers;
public class SalvaRedesSociaisController : Controller
{


    public SalvaRedesSociaisController()
    {

    }
    [Authorize]
    [HttpPost("/api/salvaredes")]
    public IActionResult SalvarRedesSociais([FromBody] RedesSociaisRequest redesSociais)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!long.TryParse(userId, out long Id))
            {
                 return Unauthorized("Erro ao salvar as redes sociais.");
            }
        if (redesSociais == null || redesSociais.RedesSociais == null)
        {
            return BadRequest("Dados inválidos.");
        }

        SalvaRedesSociaisNoBanco redes = new SalvaRedesSociaisNoBanco();
        bool resultado = redes.SalvarRedes(Id, redesSociais.RedesSociais);

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
    public Dictionary<string, List<string>> RedesSociais { get; set; }
}