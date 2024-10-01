using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Npgsql;
using NpgsqlTypes;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace ValidaClienteAWSLambda;

public class Function
{
    private string connectionString = "Host=controle-pedidos-db.czi6qjrph1bx.us-east-1.rds.amazonaws.com:5432;Username=ControlePedidoUser;Password=CPapyTes;Database=ControlePedidosDb";

    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
    {
        string cpf = input.QueryStringParameters["cpf"];

        if(!IsCpfValid(cpf))
            return new APIGatewayProxyResponse
            {
                StatusCode = 400,
                Body = "Número de CPF inválido!"
            };

        bool cpfExists = await ValidateCPFAsync(cpf);

        if (cpfExists)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = "CPF cadastrado!"
            };
        }
        else
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 403,
                Body = "CPF não cadastrado!"
            };
        }
    }

    private async Task<bool> ValidateCPFAsync(string cpf)
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

    private static bool IsCpfValid(string cpf)
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
