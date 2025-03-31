using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetoApiMVC.Models;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace ProjetoApiMVC.Controllers;
public class TrocaContatoEmailController : Controller
{


    public TrocaContatoEmailController()
    {

    }
    [Authorize]
    [HttpPost("/api/TrocaContatoEmail")]
    public IActionResult TrocaInformacaoContato([FromBody] ValoresTroca valores)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!long.TryParse(userId, out long Id))
        {
            return Unauthorized("Erro ao trocar Redes.");
        }
        if (string.IsNullOrEmpty(valores.Coluna) || string.IsNullOrEmpty(valores.Valor))
        {
            return Unauthorized("Os valores não devem estar vazios");
        }

        if (valores.Coluna != "Email" && valores.Coluna != "Contato")
        {
            return Unauthorized("Tipo de Coluna Inválida.");
        }
        if (valores.Coluna == "Email")
        {
            if (!EmailValido(valores.Valor))
            {
                return Unauthorized("Email Invalido.");
            }
        }
        if (valores.Coluna == "Contato")
        {
            if (!TelefoneValido(valores.Valor, 10)) // Define um mínimo de 10 caracteres
            {
                return Unauthorized("Número de telefone inválido.");
            }
        }
        TrocaContatoEmailModel redes = new TrocaContatoEmailModel();
        bool resultado = redes.TrocaInfoContato(Id, valores.Coluna, valores.Valor);

        if (resultado)
        {
            return Ok("Contato Trocado com Sucesso.");
        }
        else
        {
            return StatusCode(500, "Erro ao Trocar Contato.");
        }
    }
    private bool EmailValido(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
    private bool TelefoneValido(string telefone, int minimoCaracteres)
    {
        return telefone.Length >= minimoCaracteres && telefone.All(char.IsDigit);
    }

}
public class ValoresTroca
{
    public string Coluna { get; set; }
    public string Valor { get; set; }
}