namespace ValidaClienteAWSLambda.Models
{
    public class ResponseModel
    {
        public string Mensagem { get; set; }

        public ResponseModel(string mensagem)
        {
            Mensagem = mensagem;
        }
    }
}
