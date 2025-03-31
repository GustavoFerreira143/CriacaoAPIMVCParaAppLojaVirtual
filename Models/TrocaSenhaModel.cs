using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using dotenv.net;

namespace ProjetoApiMVC.Models;

public class TrocaSenhaModel
{
    private readonly string _connectionString;
    //----------------------------------------------------------------------Troca Senha de Usuario------------------------------------------------------------------------------------
    public TrocaSenhaModel()
        {
            try
            {
                DotEnv.Load();
                var dicionario = DotEnv.Read();
                _connectionString = dicionario["CONNECTION_STRING"];

                if (string.IsNullOrEmpty(_connectionString))
                {
                    throw new InvalidOperationException("A variável de ambiente CONNECTION_STRING não foi definida.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao carregar a configuração do banco de dados: " + ex.Message);
            }
        }

        public bool TrocaSenha(string email, string senha)
        {
            try
            {
                string senhaHash = CriarHashSenha(senha);

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = "UPDATE Usuarios SET Senha = @HashSenha WHERE Email = @Email";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@HashSenha", senhaHash);
                        cmd.Parameters.AddWithValue("@Email", email);

                        int rowsAffected = cmd.ExecuteNonQuery(); 
                        
                        if (rowsAffected == 0)
                        {
                            throw new Exception("Nenhum usuário foi encontrado com esse email.");
                        }
                    }
                }
                return true;
            }
            catch (SqlException ex)
            {
                
                return false;
            }
            catch (Exception ex)
            {
                
                return false;
            }
        }
        private static string CriarHashSenha(string senha)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(senha));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
}