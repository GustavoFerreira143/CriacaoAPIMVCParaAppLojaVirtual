using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProjetoApiMVC.Models;
using ProjetoApiMVC.Controllers;
using Microsoft.Data.SqlClient;

namespace ProjetoApiMVC.Models;

public class ApiGetModel
{
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }
    public string NomeEmpresa { get; set; }
    public string CNPJ { get; set; }
    public string CPF { get; set; }
    public bool AutorizadoVenda { get; set; }
    //----------------------------------------------------------------------Buscar Usuarios Post------------------------------------------------------------------------------------
    public static List<ApiGetModel> BuscarUsuarios()
    {

        List<ApiGetModel> usuarios = new List<ApiGetModel>();

        string connectionString = "Server=DESKTOP-USPO0UO\\SQLEXPRESS;Database=RentShopVT;User Id=admin;Password=1234567890;Trusted_Connection=True;TrustServerCertificate=True;";

        using (SqlConnection conn = new SqlConnection(connectionString))
        {

            conn.Open();
            string sql = "SELECT Nome, Email, Senha, CPF FROM Usuarios";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        usuarios.Add(new ApiGetModel
                        {
                            Nome = reader.GetString(0),
                            Email = reader.GetString(1),
                            Senha = reader.GetString(2),
                            CPF = reader.GetString(3)
                        });
                    }
                }
            }
        }
        return usuarios;
    }

    //-------------------------------------------------------------------------------Buscar Usuários GET-----------------------------------------------------------------------------
    public static List<ApiGetModel> BuscarUsuariosGet()
    {

        List<ApiGetModel> usuarios = new List<ApiGetModel>();

        string connectionString = "Server=DESKTOP-USPO0UO\\SQLEXPRESS;Database=RentShopVT;User Id=admin;Password=1234567890;Trusted_Connection=True;TrustServerCertificate=True;";

        using (SqlConnection conn = new SqlConnection(connectionString))
        {

            conn.Open();
            string sql = "SELECT Nome, Email, Senha, CPF FROM Usuarios";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        usuarios.Add(new ApiGetModel
                        {
                            Nome = reader.GetString(0),
                            Email = reader.GetString(1),
                            Senha = reader.GetString(2),
                            CPF = reader.GetString(3)
                        });
                    }
                }
            }
        }
        return usuarios;
    }

    //----------------------------------------------------------------------------------Inserir Usuarios POST------------------------------------------------------------------------

    public static ApiGetModel InserirUsuario(string Nome, string Email, string Senha, string NomeEmpresa, string CNPJ, string CPF, bool AutorizadoVenda)
    {

        // String de conexão com o banco de dados
        string connectionString = "Server=DESKTOP-USPO0UO\\SQLEXPRESS;Database=RentShopVT;User Id=admin;Password=1234567890;Trusted_Connection=True;TrustServerCertificate=True;";

    if (!string.IsNullOrEmpty(CNPJ))
    {
        string checkCNPJSql = "SELECT COUNT(*) FROM Usuarios WHERE CNPJ = @CNPJ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(checkCNPJSql, conn))
            {
                cmd.Parameters.AddWithValue("@CNPJ", CNPJ);
                int countCNPJ = (int)cmd.ExecuteScalar();
                if (countCNPJ > 0)
                {
                    throw new InvalidOperationException("O CNPJ já está cadastrado.");
                }
            }
        }
    }

    if (!string.IsNullOrEmpty(CPF))
    {
        string checkCPFSql = "SELECT COUNT(*) FROM Usuarios WHERE CPF = @CPF";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(checkCPFSql, conn))
            {
                cmd.Parameters.AddWithValue("@CPF", CPF);
                int countCPF = (int)cmd.ExecuteScalar();
                if (countCPF > 0)
                {
                    throw new InvalidOperationException("O CPF já está cadastrado.");
                }
            }
        }
    }

    // Query de inserção segura usando parâmetros
    string sql = @"INSERT INTO Usuarios (Nome, Email, Senha, NomeEmpresa, CNPJ, CPF, AutorizadoVenda) 
                   VALUES (@Nome, @Email, @Senha, @NomeEmpresa, @CNPJ, @CPF, @AutorizadoVenda)";

    try
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                // Definição dos parâmetros para evitar injeção SQL
                cmd.Parameters.AddWithValue("@Nome", Nome);
                cmd.Parameters.AddWithValue("@Email", Email);
                cmd.Parameters.AddWithValue("@Senha", Senha); 

                // Verificação de valores nulos ou vazios para os campos opcionais
                if (string.IsNullOrEmpty(NomeEmpresa))
                    cmd.Parameters.AddWithValue("@NomeEmpresa", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@NomeEmpresa", NomeEmpresa);

                // Verifica se o CNPJ e CPF são nulos ou vazios
                if (string.IsNullOrEmpty(CNPJ))
                    cmd.Parameters.AddWithValue("@CNPJ", DBNull.Value); 
                else
                    cmd.Parameters.AddWithValue("@CNPJ", CNPJ);

                if (string.IsNullOrEmpty(CPF))
                    cmd.Parameters.AddWithValue("@CPF", DBNull.Value);   
                else
                    cmd.Parameters.AddWithValue("@CPF", CPF);

                // Verifica se o campo AutorizadoVenda não é nulo
                cmd.Parameters.AddWithValue("@AutorizadoVenda", AutorizadoVenda);

                // Executa o comando e verifica se alguma linha foi afetada
                int linhasAfetadas = cmd.ExecuteNonQuery();
                if (linhasAfetadas > 0)
                {
                    // Retorna os dados inseridos
                    return new ApiGetModel
                    {
                        Nome = Nome,
                        Email = Email,
                        Senha = "Senha protegida", // Evita expor a senha no retorno
                        NomeEmpresa = NomeEmpresa,
                        CNPJ = CNPJ,
                        CPF = CPF,
                        AutorizadoVenda = AutorizadoVenda
                    };
                }
            }
        }
    }
    catch (Exception ex)
    {
        string mensagemErro = $"Erro ao tentar inserir o usuário. Detalhes: {ex.Message}";
        throw new InvalidOperationException(mensagemErro);  
    }

    // Retorna null em caso de erro
    return null;
}
}

