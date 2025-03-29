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
    public Dictionary<string, List<string>>? RedesSociais { get; set; }

    private readonly string connectionString = "Server=DESKTOP-USPO0UO\\SQLEXPRESS;Database=RentShopVT;User Id=admin;Password=1234567890;Trusted_Connection=True;TrustServerCertificate=True;";

//---------------------------------------------------------------------------Validação de Login de Usuário--------------------------------------------------------------------------

public VerificaLoginModel LoginUsuario(string Email, string Senha)
{
    VerificaLoginModel usuario = null; // Armazena um único usuário

    try
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();

            string query = @"SELECT 
                                u.id, u.Nome, u.Email, u.NomeEmpresa, u.CNPJ, u.CPF, 
                                u.AutorizadoVenda, u.FotoPerfil, u.Contato, 
                                r.NomeRede, r.LinkRede, r.PerfilUserRede, r.IconeRede 
                             FROM Usuarios AS u 
                             LEFT JOIN RedesSociais AS r ON u.id = r.Usuario 
                             WHERE u.Email = @Email AND u.Senha = @Senha;";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Email", Email);
                cmd.Parameters.AddWithValue("@Senha", Senha);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        if (usuario == null)
                        {
                            usuario = new VerificaLoginModel
                            {
                                id = reader.GetInt64(0),
                                Nome = reader.GetString(1),
                                Email = reader.GetString(2),
                                NomeEmpresa = reader.IsDBNull(3) ? null : reader.GetString(3),
                                CNPJ = reader.IsDBNull(4) ? null : reader.GetString(4),
                                CPF = reader.IsDBNull(5) ? null : reader.GetString(5),
                                AutorizadoVenda = reader.GetBoolean(6),
                                FotoPerfil = reader.IsDBNull(7) ? "personcircle.svg" : reader.GetString(7),
                                Contato = reader.IsDBNull(8) ? "Nenhum Telefone Cadastrado" : reader.GetString(8),
                                RedesSociais = new Dictionary<string, List<string>>()
                            };
                        }

                        if (!reader.IsDBNull(9)) 
                        {
                            string nomeRede = reader.GetString(9);
                            string iconeRede = reader.IsDBNull(12) ? "" : reader.GetString(12);
                            string linkRede = reader.IsDBNull(10) ? "" : reader.GetString(10);
                            string perfilUser = reader.IsDBNull(11) ? "" : reader.GetString(11);


                            if (!usuario.RedesSociais.ContainsKey(nomeRede))
                            {
                                usuario.RedesSociais[nomeRede] = new List<string>();
                            }

                            usuario.RedesSociais[nomeRede].Add(iconeRede);   
                            usuario.RedesSociais[nomeRede].Add(linkRede);    
                            usuario.RedesSociais[nomeRede].Add(perfilUser);  
                        }
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

    return usuario; 
}
}