using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using dotenv.net;

namespace ProjetoApiMVC.Models
{
    public class VerificaLoginModel
    {
        public long id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Contato { get; set; }
        public string Senha { get; set; }
        public string? NomeEmpresa { get; set; }
        public string? CNPJ { get; set; }
        public string? CPF { get; set; }
        public bool AutorizadoVenda { get; set; }
        public string? FotoPerfil { get; set; }
        public string Token { get; set; }
        public Dictionary<string, List<string>>? RedesSociais { get; set; }


        public VerificaLoginModel()
        {
        }

        public string GenerateJwtToken(long userId)
        {
            DotEnv.Load();
            var dicionario = DotEnv.Read();
            string hashcode = dicionario["Meu_Hash"];
            var key = Encoding.ASCII.GetBytes(hashcode);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] 
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()), 
                    new Claim(ClaimTypes.Role, "User")
                }),
                Expires = null, 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private bool VerificarSenha(string senhaInformada, string senhaHashArmazenada)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(senhaInformada));

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString() == senhaHashArmazenada;
            }
        }

        public VerificaLoginModel LoginUsuario(string Email, string Senha)
        {
            VerificaLoginModel usuario = null; 

    try
    {
        DotEnv.Load();
        var dicionario = DotEnv.Read();
        string connectionString = dicionario["CONNECTION_STRING"];

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();


            string query = @"SELECT 
                                u.id, u.Nome, u.Email, u.NomeEmpresa, u.CNPJ, u.CPF, 
                                u.AutorizadoVenda, u.FotoPerfil, u.Contato, u.Senha, 
                                r.NomeRede, r.LinkRede, r.PerfilUserRede, r.IconeRede 
                                    FROM Usuarios AS u 
                                    LEFT JOIN RedesSociais AS r ON u.id = r.Usuario 
                                    WHERE u.Email = @Email;";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Email", Email);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                  
                        string senhaHashArmazenada = reader.GetString(9); 
                        
                        if (!VerificarSenha(Senha, senhaHashArmazenada))
                        {
                            throw new UnauthorizedAccessException("Senha incorreta.");
                        }

                        string token = GenerateJwtToken(reader.GetInt64(0));

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
                                Token = token,
                                RedesSociais = new Dictionary<string, List<string>>()
                            };
                        }
                        
                        if (!reader.IsDBNull(10)) 
                        {
                            string nomeRede = reader.GetString(10);
                            string iconeRede = reader.IsDBNull(13) ? "" : reader.GetString(13);
                            string linkRede = reader.IsDBNull(11) ? "" : reader.GetString(11);
                            string perfilUser = reader.IsDBNull(12) ? "" : reader.GetString(12);

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
}