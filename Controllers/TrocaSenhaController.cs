using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetoApiMVC.Models;
using System.Security.Claims;

namespace ProjetoApiMVC.Controllers;

public class TrocaSenhaController : Controller
{
    private readonly UploadDeImagemPerfilModel _uploadImagemModel;

    public TrocaSenhaController()
    {
        _uploadImagemModel = new UploadDeImagemPerfilModel();
    }

    [Authorize]
    [HttpPost("/api/trocasenha")]
   public async Task<IActionResult> TrocaSenha([FromForm] IFormFile imagem)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!long.TryParse(userId, out long Id))
            {
               return Unauthorized("ID do usuário inválido.");
            }
        if (imagem == null || imagem.Length == 0)
            return BadRequest("Nenhuma imagem foi enviada.");


        Retorno resposta = await _uploadImagemModel.ProcessarEEnviarImagemAsync(imagem, Id);

        if (resposta.Status == "Sucesso")
            return Ok(new { status = resposta.Status, link = resposta.Link });

        return StatusCode(500, "Erro ao enviar a imagem.");
    }

}