using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProjetoApiMVC.Models;
using ProjetoApiMVC.Controllers;
using Microsoft.Data.SqlClient;

namespace ProjetoApiMVC.Models;

public class VerificaDuplicidadeModel
{

    private readonly string connectionString = "Server=DESKTOP-USPO0UO\\SQLEXPRESS;Database=RentShopVT;User Id=admin;Password=1234567890;Trusted_Connection=True;TrustServerCertificate=True;";

//----------------------------------------------- Método para verificar se há duplicidade de um valor em uma coluna específica------------------------------------------------------
    public bool VerificarDuplicidade(string coluna, string valor)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open(); 

                string query = $"SELECT COUNT(*) FROM Usuarios WHERE {coluna} = @Valor";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {

                    cmd.Parameters.AddWithValue("@Valor", valor);

                   
                    int count = (int)cmd.ExecuteScalar();

                    return count > 0; 
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao acessar o banco de dados: {ex.Message}");
            return false;
        }
    }


}


