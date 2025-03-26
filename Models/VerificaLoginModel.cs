using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProjetoApiMVC.Models;
using ProjetoApiMVC.Controllers;
using Microsoft.Data.SqlClient;

namespace ProjetoApiMVC.Models;

public class VerificaLoginModel
{
    public long  id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Contato { get; set; }    
    public string Senha { get; set; }
    public string? NomeEmpresa { get; set; }
    public string? CNPJ { get; set; }
    public string? CPF { get; set; }
    public bool AutorizadoVenda { get; set; }
    public string? FotoPerfil { get; set; }
    private readonly string connectionString = "Server=DESKTOP-USPO0UO\\SQLEXPRESS;Database=RentShopVT;User Id=admin;Password=1234567890;Trusted_Connection=True;TrustServerCertificate=True;";

//---------------------------------------------------------------------------Validação de Login de Usuário--------------------------------------------------------------------------

public List<VerificaLoginModel> LoginUsuario(string Email, string Senha)
{
    List<VerificaLoginModel> usuarios = new List<VerificaLoginModel>();

    try
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();

            string query = "SELECT id, Nome, Email, NomeEmpresa, CNPJ, CPF, AutorizadoVenda, FotoPerfil, Contato FROM Usuarios WHERE Email = @Email AND Senha = @Senha;";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Email", Email);
                cmd.Parameters.AddWithValue("@Senha", Senha);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        usuarios.Add(new VerificaLoginModel
                        {
                            id = reader.GetInt64(0),
                            Nome = reader.GetString(1),
                            Email = reader.GetString(2),
                            NomeEmpresa = reader.IsDBNull(3) ? null : reader.GetString(3),
                            CNPJ = reader.IsDBNull(4) ? null : reader.GetString(4),
                            CPF = reader.IsDBNull(5) ? null : reader.GetString(5),
                            AutorizadoVenda = reader.GetBoolean(6),
                            FotoPerfil = reader.IsDBNull(7) ? "personcircle.svg" : reader.GetString(7),
                            Contato = reader.IsDBNull(8) ? "Nenhum Telefone Cadastrado" : reader.GetString(8)
                        });
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {
        string mensagemErro = $"Erro ao tentar buscar o usuário. Detalhes: {ex.Message}";
        throw new InvalidOperationException(mensagemErro);
    }

    // Retorna null se a lista estiver vazia
    return usuarios.Count > 0 ? usuarios : null;
}
}