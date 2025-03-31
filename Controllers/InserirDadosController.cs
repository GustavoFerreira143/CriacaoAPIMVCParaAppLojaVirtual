using Microsoft.AspNetCore.Mvc;
using ProjetoApiMVC.Models;
using dotenv.net;
using System.Text.RegularExpressions;
//------------------------------------------------------------------------------------Inserir valores Insert---------------------------------------------------------------------   

namespace ProjetoApiMVC.Controllers;

    public class InserirDadosController : Controller
    {
        [HttpPost("api/InserirValores")]
        public async Task<ActionResult> InserirValores([FromBody] ConsultaRequest request)
        {
            try
            {
                DotEnv.Load();
                var dicionario = DotEnv.Read();
                string MeuHashSecreto = dicionario["MeuTokenCadastrauser"];
                if(MeuHashSecreto != request.HashSecreto)
                {
                    return BadRequest(new { Message = "Token Inválido ou Inexistente Detectado." });
                }
                // Verificação obrigatória para Nome, Email e Senha
                if (request == null ||
                    string.IsNullOrWhiteSpace(request.Nome) ||
                    string.IsNullOrWhiteSpace(request.Email) ||
                    string.IsNullOrWhiteSpace(request.Senha)
                    || string.IsNullOrWhiteSpace(request.Contato))
                {
                    return BadRequest(new { Message = "Os campos Nome, Email e Senha são obrigatórios." });
                }
                if(!EmailValido(request.Email))
                {
                    return BadRequest(new { Message = "Email Inválido Detectado" });
                }

                bool isEmpresa = !string.IsNullOrWhiteSpace(request.NomeEmpresa) && !string.IsNullOrWhiteSpace(request.CNPJ);
                bool isPessoaFisica = !string.IsNullOrWhiteSpace(request.CPF);

                if (isEmpresa && isPessoaFisica)
                {
                    return BadRequest(new { Message = "Se NomeEmpresa for informado, CPF não pode ser preenchido." });
                }

                if (!isEmpresa && !isPessoaFisica && string.IsNullOrWhiteSpace(request.CPF))
                {
                    return BadRequest(new { Message = "Se NomeEmpresa e CNPJ não forem informados, o CPF deve ser informado." });
                }

                if (isEmpresa && isPessoaFisica)
                {
                    return BadRequest(new { Message = "Não é possível informar NomeEmpresa e CNPJ com CPF ao mesmo tempo." });
                }

                if ((isEmpresa && string.IsNullOrWhiteSpace(request.CNPJ)) || 
                    (string.IsNullOrWhiteSpace(request.NomeEmpresa) && !string.IsNullOrWhiteSpace(request.CNPJ)))
                {
                    return BadRequest(new { Message = "Se NomeEmpresa for informado, o CNPJ também deve ser." });
                }
                if(!string.IsNullOrWhiteSpace(request.CPF) && request.CPF.Length == 11)
                {
                    if(!ValidarCPF(request.CPF))
                    {

                       return BadRequest(new { Message = "CPF Inválido Detectado" });
                    }
                }
                if(!string.IsNullOrWhiteSpace(request.CNPJ) && request.CNPJ.Length == 14)
                {
                    if(!ValidarCNPJ(request.CNPJ))
                    {
                       return BadRequest(new { Message = "CNPJ Inválido Detectado" });
                    }
                }     
                           
                // Insere os dados no banco
                InserirDadosModel usuario = InserirDadosModel.InserirUsuario(
                    request.Nome,
                    request.Email,
                    request.Senha,
                    request.NomeEmpresa,
                    request.CNPJ,
                    request.CPF,
                    request.Contato,
                    request.AutorizadoVenda
                );

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro: " + ex.Message);
                return BadRequest(new { Message = "Erro ao processar a requisição", Details = ex.Message });
            }
        }
        private bool EmailValido(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
                private bool SenhaPossuiCaractereEspecial(string senha)
        {
            return senha.Any(ch => !char.IsLetterOrDigit(ch));
        }
        public static bool ValidarCPF(string cpf)
        {
            cpf = cpf.Replace(".", "").Replace("-", "").Trim();

            // Verifica se o CPF tem 11 caracteres
            if (cpf.Length != 11) return false;

            // CPF não pode ser uma sequência de números repetidos
            if (cpf.Distinct().Count() == 1) return false;

            // Cálculo do primeiro dígito verificador
            int soma = 0;
            int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            for (int i = 0; i < 9; i++)
                soma += int.Parse(cpf[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            int digito1 = (resto < 2) ? 0 : 11 - resto;

            // Cálculo do segundo dígito verificador
            soma = 0;
            int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            for (int i = 0; i < 10; i++)
                soma += int.Parse(cpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            int digito2 = (resto < 2) ? 0 : 11 - resto;

            // Compara os dois dígitos verificadores calculados com os fornecidos
            return cpf.EndsWith(digito1.ToString() + digito2.ToString());
        }
        public static bool ValidarCNPJ(string cnpj)
        {
            cnpj = cnpj.Replace(".", "").Replace("/", "").Replace("-", "").Trim();

            // Verifica se o CNPJ tem 14 caracteres
            if (cnpj.Length != 14) return false;

            // CNPJ não pode ser uma sequência de números repetidos
            if (cnpj.Distinct().Count() == 1) return false;

            // Cálculo do primeiro dígito verificador
            int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma = 0;

            for (int i = 0; i < 12; i++)
                soma += int.Parse(cnpj[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            int digito1 = (resto < 2) ? 0 : 11 - resto;

            // Cálculo do segundo dígito verificador
            int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            soma = 0;

            for (int i = 0; i < 13; i++)
                soma += int.Parse(cnpj[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            int digito2 = (resto < 2) ? 0 : 11 - resto;

            // Compara os dois dígitos verificadores calculados com os fornecidos
            return cnpj.EndsWith(digito1.ToString() + digito2.ToString());
        }
    }
    //-------------------------------------------------------------------------Valores Esperados--------------------------------------------------------------------------

public class ConsultaRequest
{
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }
    public string NomeEmpresa { get; set; }
    public string CNPJ { get; set; }
    public string CPF { get; set; }
    public string Contato { get; set; }
    public bool AutorizadoVenda { get; set; }
    public string HashSecreto {get; set;}
}
