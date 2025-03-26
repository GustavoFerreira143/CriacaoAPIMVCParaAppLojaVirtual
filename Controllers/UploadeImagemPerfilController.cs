using Microsoft.AspNetCore.Mvc;
using ProjetoApiMVC.Models;

namespace ProjetoApiMVC.Controllers;

public class UploadImagemPerfilController : Controller
{
    private readonly UploadDeImagemPerfilModel _uploadImagemModel;

    public UploadImagemPerfilController()
    {
        _uploadImagemModel = new UploadDeImagemPerfilModel();
    }


    [HttpPost("/api/uploadimagemperfil")]
   public async Task<IActionResult> UploadImagem([FromForm] IFormFile imagem, [FromForm] string ID)
    {
        if (imagem == null || imagem.Length == 0)
            return BadRequest("Nenhuma imagem foi enviada.");

        if (!long.TryParse(ID, out long userId) || userId <= 0)
            return Unauthorized("ID do usuário inválido.");

        Retorno resposta = await _uploadImagemModel.ProcessarEEnviarImagemAsync(imagem, userId);

        if (resposta.Status == "Sucesso")
            return Ok(new { status = resposta.Status, link = resposta.Link });

        return StatusCode(500, "Erro ao enviar a imagem.");
    }

}
