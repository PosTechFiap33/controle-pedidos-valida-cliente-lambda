# ValidaClienteAWSLambda
Esta função AWS Lambda valida um número de CPF (Cadastro de Pessoas Físicas) brasileiro e verifica sua existência em um banco de dados PostgreSQL. A função é projetada para ser acionada por um evento do API Gateway.

### <u>Funcionalidade</u>

1.	Validação de CPF:
- A função primeiro valida o formato e o dígito verificador do número de CPF fornecido.
- Se o CPF for inválido, retorna uma resposta 400 Bad Request com uma mensagem indicando o CPF inválido.
2.	Verificação no Banco de Dados:
- Se o CPF for válido, a função verifica sua existência em um banco de dados PostgreSQL.
- Se o CPF existir no banco de dados, retorna uma resposta 200 OK com uma mensagem indicando que o CPF está cadastrado.
- Se o CPF não existir, retorna uma resposta 403 Forbidden com uma mensagem indicando que o CPF não está cadastrado.
3.	Tratamento de Erros:
- Se houver um erro ao conectar ao banco de dados, a função retorna uma resposta 500 Internal Server Error com uma mensagem indicando um erro de conexão com o banco de dados.

---

### <u>Variáveis de Ambiente</u>
- LAMBDA_DB_CONNECTION: A string de conexão para o banco de dados PostgreSQL.

### <u>Requisição do API Gateway</u>
A função espera uma requisição do API Gateway com o número de CPF fornecido como um parâmetro de caminho.

---

### <u>Requisição do API Gateway</u>

A função espera uma requisição do API Gateway com o número de CPF fornecido como um parâmetro de caminho.

<u>Exemplo de requisição</u>

```
{
  "pathParameters": {
    "cpf": "12345678909"
  }
}
```