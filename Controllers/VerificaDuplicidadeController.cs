using Microsoft.AspNetCore.Mvc;
using ProjetoApiMVC.Models;

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
}

}