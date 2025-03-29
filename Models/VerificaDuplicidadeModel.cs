using Microsoft.Data.SqlClient;
using dotenv.net;

namespace ProjetoApiMVC.Models;

public class VerificaDuplicidadeModel
{


//----------------------------------------------- Método para verificar se há duplicidade de um valor em uma coluna específica------------------------------------------------------
    public bool VerificarDuplicidade(string coluna, string valor)
    {
        try
        {
            DotEnv.Load();
            var dicionario = DotEnv.Read();
            string connectionString = dicionario["CONNECTION_STRING"];
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


