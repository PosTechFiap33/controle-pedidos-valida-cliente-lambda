using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Npgsql;
using NpgsqlTypes;
using System.Text.Json;
using ValidaClienteAWSLambda.Models;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace ValidaClienteAWSLambda;

public class Function
{
    private string connectionString = Environment.GetEnvironmentVariable("LAMBDA_DB_CONNECTION");

    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
    {
        //string cpf = input.QueryStringParameters["cpf"];
        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(input),
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };

        //try
        //{
        //    if (!IsCpfValid(cpf))
        //        return new APIGatewayProxyResponse
        //        {
        //            StatusCode = 400,
        //            Body = JsonSerializer.Serialize(new ResponseModel("Número de CPF inválido!")),
        //            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        //        };

        //    bool cpfExists = await ValidateCPFAsync(cpf);

        //    if (cpfExists)
        //    {
        //        return new APIGatewayProxyResponse
        //        {
        //            StatusCode = 200,
        //            Body = JsonSerializer.Serialize(new ResponseModel("CPF cadastrado!")),
        //            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        //        };
        //    }
        //    else
        //    {
        //        return new APIGatewayProxyResponse
        //        {
        //            StatusCode = 403,
        //            Body = JsonSerializer.Serialize(new ResponseModel("CPF não cadastrado!")),
        //            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        //        };
        //    }
        //}
        //catch (Exception ex) when (ex is NpgsqlException || ex is InvalidOperationException)
        //{
        //    return new APIGatewayProxyResponse
        //    {
        //        StatusCode = 500,
        //        Body = JsonSerializer.Serialize(new ResponseModel("Erro ao conectar ao banco de dados!")),
        //        Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        //    };
        //}
    }

    public async Task<bool> ValidateCPFAsync(string cpf)
    {
        try
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("SELECT COUNT(*) FROM \"Clientes\" WHERE \"Cpf\" = @cpf", connection))
                {
                    command.Parameters.Add(new NpgsqlParameter("@cpf", NpgsqlDbType.Varchar) { Value = cpf });

                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result) > 0;
                }
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Erro ao conectar ao banco de dados", ex);
        }
    }

    public static bool IsCpfValid(string cpf)
    {
        int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        cpf = cpf.Trim().Replace(".", "").Replace("-", "");
        if (cpf.Length != 11 || cpf.All(c => c == cpf[0]))
            return false;

        string tempCpf = cpf.Substring(0, 9);
        int soma = tempCpf.Select((t, i) => int.Parse(t.ToString()) * multiplicador1[i]).Sum();

        int resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;
        tempCpf += resto;

        soma = tempCpf.Select((t, i) => int.Parse(t.ToString()) * multiplicador2[i]).Sum();

        resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;
        tempCpf += resto;

        return cpf.EndsWith(tempCpf.Substring(9, 2));
    }
}