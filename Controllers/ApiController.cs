using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProjetoApiMVC.Models;

namespace ProjetoApiMVC.Controllers;

public class ApiController : ControllerBase
{
    //-----------------------------------------------------------------------------------Pegar Valores Post-------------------------------------------------------------------        
    [HttpPost("api/reqPost")]
    public async Task<ActionResult> ConsultarPost()
    {
        try
        {


            List<ApiGetModel> usuarios = ApiGetModel.BuscarUsuarios();

            return Ok(usuarios);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro" + ex.Message);
            return BadRequest(ex.Message);
        }
    }
    //-------------------------------------------------------------------------------------Pegar Valores GET---------------------------------------------------------------------
    [HttpGet("api/reqGet")]
    public async Task<ActionResult> ConsultarGet()
    {
        try
        {

            List<ApiGetModel> usuarios = ApiGetModel.BuscarUsuariosGet();

            return Ok(usuarios);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro" + ex.Message);
            return BadRequest(ex.Message);
        }
    }

    //------------------------------------------------------------------------------------Inserir valores Insert---------------------------------------------------------------------   
[HttpPost("api/InserirValores")]
public async Task<ActionResult> InserirValores([FromBody] ConsultaRequest request)
{
    try
    {
        // Verificação obrigatória para Nome, Email e Senha
        if (request == null ||
            string.IsNullOrWhiteSpace(request.Nome) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Senha))
        {
            return BadRequest(new { Message = "Os campos Nome, Email e Senha são obrigatórios." });
        }

        // Verificação de NomeEmpresa, CNPJ e CPF
        bool isEmpresa = !string.IsNullOrWhiteSpace(request.NomeEmpresa) && !string.IsNullOrWhiteSpace(request.CNPJ);
        bool isPessoaFisica = !string.IsNullOrWhiteSpace(request.CPF);

        // Caso NomeEmpresa seja informado, CPF não pode ser
        if (isEmpresa && isPessoaFisica)
        {
            return BadRequest(new { Message = "Se NomeEmpresa for informado, CPF não pode ser preenchido." });
        }

        // Caso NomeEmpresa e CNPJ não tenham valores, CPF deve ter valor
        if (!isEmpresa && !isPessoaFisica && string.IsNullOrWhiteSpace(request.CPF))
        {
            return BadRequest(new { Message = "Se NomeEmpresa e CNPJ não forem informados, o CPF deve ser informado." });
        }

        // Caso NomeEmpresa e CNPJ estejam preenchidos, CPF não pode ser
        if (isEmpresa && isPessoaFisica)
        {
            return BadRequest(new { Message = "Não é possível informar NomeEmpresa e CNPJ com CPF ao mesmo tempo." });
        }

        // Caso NomeEmpresa ou CNPJ estejam preenchidos sem o outro, é erro
        if ((isEmpresa && string.IsNullOrWhiteSpace(request.CNPJ)) || 
            (string.IsNullOrWhiteSpace(request.NomeEmpresa) && !string.IsNullOrWhiteSpace(request.CNPJ)))
        {
            return BadRequest(new { Message = "Se NomeEmpresa for informado, o CNPJ também deve ser." });
        }

        // Insere os dados no banco
        ApiGetModel usuario = ApiGetModel.InserirUsuario(
            request.Nome,
            request.Email,
            request.Senha,
            request.NomeEmpresa,
            request.CNPJ,
            request.CPF,
            request.AutorizadoVenda
        );

        return Ok(usuario);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Erro: " + ex.Message);
        return BadRequest(new { Message = "Erro ao processar a requisição", Details = ex.Message });
    }
}
}

//-------------------------------------------------------------------------Valores Esperados--------------------------------------------------------------------------

public class ConsultaRequest
{
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }
    public string NomeEmpresa { get; set; }
    public string CNPJ { get; set; }
    public string CPF { get; set; }
    public bool AutorizadoVenda { get; set; }
}

