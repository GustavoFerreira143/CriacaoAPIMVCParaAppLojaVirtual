using Microsoft.AspNetCore.Mvc;
using ProjetoApiMVC.Models;
using dotenv.net;

namespace ProjetoApiMVC.Controllers;

public class TrocaSenhaController : Controller
{
    private readonly UploadDeImagemPerfilModel _uploadImagemModel;

    public TrocaSenhaController()
    {
        _uploadImagemModel = new UploadDeImagemPerfilModel();
    }

    [HttpPost("/api/trocasenha")]
        public async Task<IActionResult> TrocaSenha([FromBody] ValidaRequisicao Data)
        {
           try{ 
            DotEnv.Load();
            var dicionario = DotEnv.Read();
            string MeuHashSecreto = dicionario["MeuTokenTrocaSenha"];
            if (MeuHashSecreto != Data.HashSecreto)
            {
                return Unauthorized("Hash do usuário inválido.");
            }

            if (string.IsNullOrWhiteSpace(Data.Senha))
            {
                return BadRequest("Nenhuma senha foi informada.");
            }
            if (string.IsNullOrWhiteSpace(Data.Email))
            {
                return BadRequest("Nenhum Email foi informado.");
            }
            TrocaSenhaModel trocaSenhaModel = new TrocaSenhaModel();

            bool resposta =  trocaSenhaModel.TrocaSenha(Data.Email,Data.Senha);

            if (resposta)
            {
                return Ok("Senha alterada com sucesso.");
            }
            else
            {
                return BadRequest("Usuario não Encontrado");
            }
           }
           catch (Exception ex)
           {
             return StatusCode(500, new { Message = "Erro ao processar a requisição", Details = ex.Message });
           }
        }

    public class ValidaRequisicao
    {
        public string Email {get;set;}
        public string Senha {get; set;}
        public string HashSecreto {get; set;}
    }
}