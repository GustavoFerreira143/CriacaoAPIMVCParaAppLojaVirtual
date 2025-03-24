using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProjetoApiMVC.Models;
using ProjetoApiMVC.Controllers;
using Microsoft.Data.SqlClient;

namespace ProjetoApiMVC.Models;

public class ApiModel
{
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }
    public string? NomeEmpresa { get; set; }
    public string? CNPJ { get; set; }
    public string? CPF { get; set; }
    public bool AutorizadoVenda { get; set; }
    //----------------------------------------------------------------------Buscar Usuarios Post------------------------------------------------------------------------------------

    public static List<ApiModel> BuscarUsuarios()
    {

        List<ApiModel> usuarios = new List<ApiModel>();

        string connectionString = "Server=DESKTOP-USPO0UO\\SQLEXPRESS;Database=RentShopVT;User Id=admin;Password=1234567890;Trusted_Connection=True;TrustServerCertificate=True;";

        using (SqlConnection conn = new SqlConnection(connectionString))
        {

            conn.Open();
            string sql = "SELECT Nome, Email, Senha, NomeEmpresa, CNPJ, CPF, AutorizadoVenda FROM Usuarios";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        usuarios.Add(new ApiModel
                        {
                           Nome =  reader.GetString(0),  // Nome
                           Email = reader.GetString(1),  // Email
                           Senha = reader.GetString(2),  // Senha
                           NomeEmpresa = reader.IsDBNull(3) ? null : reader.GetString(3),  // NomeEmpresa (pode ser NULL)
                           CNPJ = reader.IsDBNull(4) ? null : reader.GetString(4),  // CNPJ (pode ser NULL)
                           CPF = reader.IsDBNull(5) ? null : reader.GetString(5),  // CPF (pode ser NULL)
                           AutorizadoVenda = reader.GetBoolean(6)   // AutorizadoVenda
                        }
                        );
                    }
                }
            }
        }
        return usuarios;
    }

    //-------------------------------------------------------------------------------Buscar Usuários GET-----------------------------------------------------------------------------
    public static List<ApiModel> BuscarUsuariosGet()
    {

        List<ApiModel> usuarios = new List<ApiModel>();

        string connectionString = "Server=DESKTOP-USPO0UO\\SQLEXPRESS;Database=RentShopVT;User Id=admin;Password=1234567890;Trusted_Connection=True;TrustServerCertificate=True;";

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = "SELECT Nome, Email, Senha, NomeEmpresa, CNPJ, CPF, AutorizadoVenda FROM Usuarios";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        usuarios.Add(new ApiModel
                        {
                           Nome =  reader.GetString(0),  // Nome
                           Email = reader.GetString(1),  // Email
                           Senha = reader.GetString(2),  // Senha
                           NomeEmpresa = reader.IsDBNull(3) ? null : reader.GetString(3),  // NomeEmpresa (pode ser NULL)
                           CNPJ = reader.IsDBNull(4) ? null : reader.GetString(4),  // CNPJ (pode ser NULL)
                           CPF = reader.IsDBNull(5) ? null : reader.GetString(5),  // CPF (pode ser NULL)
                           AutorizadoVenda = reader.GetBoolean(6)   // AutorizadoVenda
                        }
                        );
                    }
                }
            }
        }
        return usuarios;
    }

}

