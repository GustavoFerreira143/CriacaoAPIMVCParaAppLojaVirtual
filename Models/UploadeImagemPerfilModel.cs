using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using dotenv.net;

namespace ProjetoApiMVC.Models;
public class UploadDeImagemPerfilModel
{
    private readonly string _uploadPath;
    private readonly string _connectionString;

    public UploadDeImagemPerfilModel()
    {
        _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        DotEnv.Load();
        var dicionario = DotEnv.Read();
        _connectionString = dicionario["CONNECTION_STRING"];

        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    public async Task<Retorno> ProcessarEEnviarImagemAsync(IFormFile file, long userID)
    {
        try
        {
            // Verifica se há uma imagem antiga do usuário
            string imagemAntiga = BuscarImagemAntiga(userID);
            if (!string.IsNullOrEmpty(imagemAntiga))
            {
                // Deleta o arquivo anterior antes de salvar o novo
                string caminhoAntigo = Path.Combine(_uploadPath, Path.GetFileName(imagemAntiga));
                if (File.Exists(caminhoAntigo))
                {
                    File.Delete(caminhoAntigo);
                }
            }

            // Gera um nome único com timestamp
            string extensao = Path.GetExtension(file.FileName).ToLower();
            string nomeArquivo = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}{extensao}";

            // Cria um hash do nome do arquivo
            string nomeComHash = GerarHash(nomeArquivo) + extensao;
            string caminhoArquivo = Path.Combine(_uploadPath, nomeComHash);

            // Salva a imagem no servidor
            using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Gera a URL da imagem
            string urlImagem = $"/uploads/{nomeComHash}";

            // Atualiza o link da imagem no banco de dados
            if (AtualizarImagemNoBanco(userID, urlImagem))
            {
                return new Retorno
                {
                    Status = "Sucesso",
                    Link = urlImagem
                };
            }

            return new Retorno { Status = "Erro ao atualizar no banco", Link = "" };
        }
        catch (Exception ex)
        {
            return new Retorno { Status = $"Erro: {ex.Message}", Link = "" };
        }
    }

    private string GerarHash(string input)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }

    private string BuscarImagemAntiga(long userID)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT FotoPerfil FROM Usuarios WHERE ID = @ID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", userID);
                    var result = cmd.ExecuteScalar();
                    return result?.ToString();
                }
            }
        }
        catch
        {
            return null;
        }
    }

    private bool AtualizarImagemNoBanco(long userID, string urlImagem)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "UPDATE Usuarios SET FotoPerfil = @Url WHERE ID = @ID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Url", urlImagem);
                    cmd.Parameters.AddWithValue("@ID", userID);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        catch
        {
            return false;
        }
    }
}

public class Retorno
{
    public string Status { get; set; }
    public string Link { get; set; }
}