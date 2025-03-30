using Microsoft.AspNetCore.Mvc;
using ProjetoApiMVC.Models;
using Microsoft.AspNetCore.Authorization;
using dotenv.net;

namespace ProjetoApiMVC.Controllers;
public class VerificaDuplicidadeController : Controller
{
    private readonly VerificaDuplicidadeModel _apiValoresSimplesModel;

    public VerificaDuplicidadeController()
    {
        _apiValoresSimplesModel = new VerificaDuplicidadeModel(); 
    }
//------------------------------------------ Método para verificar enviar valores para o Model e processar informações Recebidas

    [HttpPost("api/VerificarDuplicidade")]
    public IActionResult VerificarDuplicidade([FromBody] DuplicidadeRequest request)
    {
        try
        {
            DotEnv.Load();
            var dicionario = DotEnv.Read();
            string MeuHashSecreto = dicionario["MeuTokenVerificaDuplicidade"];

            if(MeuHashSecreto != request.MeuHashSecreto)
            {
                 return BadRequest(new { Message = "Chave de Acesso Incorreta Detectada" });
            }

            if (string.IsNullOrWhiteSpace(request.Coluna) || string.IsNullOrWhiteSpace(request.Valor))
            {
                return BadRequest(new { Message = "Os parâmetros 'coluna' e 'valor' são obrigatórios." });
            }

            bool existeDuplicidade = _apiValoresSimplesModel.VerificarDuplicidade(request.Coluna, request.Valor);
            if (existeDuplicidade)
            {
                return Ok(new { Message = "Duplicidade encontrada." });
            }
            else
            {
                return NotFound(new { Message = "Nenhuma duplicidade encontrada." });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Erro ao processar a requisição", Details = ex.Message });
        }
    }


// Classe auxiliar para receber os parâmetros do JSON no corpo da requisição 

public class DuplicidadeRequest
{
    public string Coluna { get; set; }
    public string Valor { get; set; }
    public string MeuHashSecreto { get; set; }
}

}