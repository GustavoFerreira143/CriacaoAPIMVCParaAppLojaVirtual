using Microsoft.AspNetCore.Mvc;
using ProjetoApiMVC.Models;
using dotenv.net;

namespace ProjetoApiMVC.Controllers;
 public class VerificaLoginController : Controller
 {
    VerificaLoginModel _apiValoresSimplesModel = new VerificaLoginModel(); 
    public VerificaLoginController()
    {
        _apiValoresSimplesModel = new VerificaLoginModel(); 
    }
    //----------------------------------------------------------------Verifica Login Controller----------------------------------------------------------------------------

    [HttpPost("api/VerificarLogin")]
    public IActionResult VerificarDuplicidade([FromBody] VerificaLogin request)
    {
        try
        {
            DotEnv.Load();
            var dicionario = DotEnv.Read();
            string MeuHashSecreto = dicionario["MeuTokenLoginUser"];

            if(MeuHashSecreto != request.MeuHashSecreto)
            {
                 return BadRequest(new { Message = "Chave de Acesso Incorreta Detectada" });
            }

            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Senha))
            {
                return BadRequest(new { Message = "Os parâmetros 'Email' e 'Senha' são obrigatórios." });
            }

            VerificaLoginModel ValoresDoUsuario = _apiValoresSimplesModel.LoginUsuario(request.Email, request.Senha);

            if (ValoresDoUsuario != null)
            {
                return Ok(ValoresDoUsuario);
            }
            else
            {
                return NotFound(new { Message = "Nenhuma Login Encontrado" });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Erro ao processar a requisição", Details = ex.Message });
        }
    }
}
public class VerificaLogin
{
    public string Email { get; set; }
    public string Senha { get; set; }
    public string MeuHashSecreto { get; set; }
}

 