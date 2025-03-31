using Microsoft.Data.SqlClient;
using dotenv.net;

namespace ProjetoApiMVC.Models;
public class TrocaContatoEmailModel
{


    public TrocaContatoEmailModel()
    {

    }
public bool TrocaInfoContato(long id, string coluna, string valor)
{
    try
    {
        DotEnv.Load();
        var dicionario = DotEnv.Read();
        string connectionString = dicionario["CONNECTION_STRING"];

        HashSet<string> colunasPermitidas = new HashSet<string> { "Email", "Contato"};

        if (!colunasPermitidas.Contains(coluna))
        {
            Console.WriteLine("Tentativa de atualização em coluna não permitida.");
            return false;
        }

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string queryAtualiza = $"UPDATE Usuarios SET {coluna} = @Valor WHERE Id = @Id";

            using (SqlCommand cmd = new SqlCommand(queryAtualiza, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Valor", valor);

                int rowsAffected = cmd.ExecuteNonQuery();

                // Retorna true apenas se alguma linha foi realmente atualizada
                return rowsAffected > 0;
            }
        }
    }
    catch (SqlException sqlEx)
    {
        Console.WriteLine($"Erro SQL: {sqlEx.Message}");
        return false;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro inesperado: {ex.Message}");
        return false;
    }
}
}