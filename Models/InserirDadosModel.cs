using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using dotenv.net;

namespace ProjetoApiMVC.Models
{
    public class InserirDadosModel
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string? NomeEmpresa { get; set; }
        public string? CNPJ { get; set; }
        public string? CPF { get; set; }
        public string? Contato { get; set; }
        public bool AutorizadoVenda { get; set; }

        //----------------------------------------------------------------------------------Inserir Usuarios POST------------------------------------------------------------------------
        public static InserirDadosModel InserirUsuario(string Nome, string Email, string Senha, string NomeEmpresa, string CNPJ, string CPF, string Contato, bool AutorizadoVenda)
        {
            DotEnv.Load();
            var dicionario = DotEnv.Read();
            string connectionString = dicionario["CONNECTION_STRING"]; 

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

            // Gerar o hash da senha
            string senhaHash = CriarHashSenha(Senha);

            // Query de inserção segura usando parâmetros
            string sql = @"INSERT INTO Usuarios (Nome, Email, Senha, NomeEmpresa, CNPJ, CPF, Contato, AutorizadoVenda) 
                           VALUES (@Nome, @Email, @Senha, @NomeEmpresa, @CNPJ, @CPF, @Contato, @AutorizadoVenda)";

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
                        cmd.Parameters.AddWithValue("@Senha", senhaHash);  // Armazenando o hash da senha
                        cmd.Parameters.AddWithValue("@Contato", Contato);

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

                        cmd.Parameters.AddWithValue("@AutorizadoVenda", AutorizadoVenda);

                        // Executa o comando e verifica se alguma linha foi afetada
                        int linhasAfetadas = cmd.ExecuteNonQuery();
                        if (linhasAfetadas > 0)
                        {
                            // Retorna os dados inseridos
                            return new InserirDadosModel
                            {
                                Nome = Nome,
                                Email = Email,
                                Contato = Contato,
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

        // Método para criar um hash da senha utilizando o algoritmo SHA256
        private static string CriarHashSenha(string senha)
        {
            using (var sha256 = SHA256.Create())
            {
                // Gerando o hash da senha
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(senha));
                // Convertendo o hash para uma string hexadecimal
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
