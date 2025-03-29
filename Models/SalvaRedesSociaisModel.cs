using Microsoft.Data.SqlClient;
using dotenv.net;

namespace ProjetoApiMVC.Models;
  public class SalvaRedesSociaisNoBanco 
    {
        public SalvaRedesSociaisNoBanco()
        {
            
        }
        public bool SalvarRedes(long id, Dictionary<string, List<string>> redessociais)
        {
            try
            {
                DotEnv.Load();
                var dicionario = DotEnv.Read();
                string connectionString = dicionario["CONNECTION_STRING"];

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string queryDelete = "DELETE FROM RedesSociais WHERE Usuario = @Id";
                    using (SqlCommand cmdDelete = new SqlCommand(queryDelete, conn))
                    {
                        cmdDelete.Parameters.AddWithValue("@Id", id);
                        cmdDelete.ExecuteNonQuery(); 
                        Console.WriteLine("Sucesso ao apagar redes sociais anteriores.");
                    }


                    if (redessociais == null || redessociais.Count == 0)
                    {
                        Console.WriteLine("Nenhuma nova rede social para salvar.");
                        return true;
                    }

                    string queryInsert = "INSERT INTO RedesSociais (Usuario, NomeRede, LinkRede, IconeRede, PerfilUserRede) VALUES (@id, @NomeRede, @LinkRede, @IconeRede, @PerfilUserRede)";

                    foreach (var rede in redessociais)
                    {
                        string nomeRede = rede.Key;
                        List<string> valores = rede.Value;

                        if (valores.Count < 3)
                        {
                            Console.WriteLine($"Erro: A rede {nomeRede} não contém valores suficientes.");
                            continue;
                        }

                        string iconeRede = valores[0];
                        string linkRede = valores[1]; 
                        string perfilUser =  valores[2];

                        using (SqlCommand cmdInsert = new SqlCommand(queryInsert, conn))
                        {
                            cmdInsert.Parameters.AddWithValue("@id", id);
                            cmdInsert.Parameters.AddWithValue("@NomeRede", nomeRede);
                            cmdInsert.Parameters.AddWithValue("@LinkRede", linkRede);
                            cmdInsert.Parameters.AddWithValue("@IconeRede", iconeRede);
                            cmdInsert.Parameters.AddWithValue("@PerfilUserRede", perfilUser);

                            cmdInsert.ExecuteNonQuery(); 
                        }
                    }

                    Console.WriteLine("Redes sociais salvas com sucesso.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao acessar o banco de dados: {ex.Message}");
                return false;
            }
        }

    }