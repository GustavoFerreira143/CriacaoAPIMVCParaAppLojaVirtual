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
            List<ApiModel> usuarios = ApiModel.BuscarUsuarios();

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
            List<ApiModel> usuarios = ApiModel.BuscarUsuariosGet();

            return Ok(usuarios);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro" + ex.Message);
            return BadRequest(ex.Message);
        }
    }

}


