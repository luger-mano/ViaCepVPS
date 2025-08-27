namespace ViaCepIntegracao.Exceptions
{
    public class ApiViaCepException : Exception
    {
        public ApiViaCepException(string message): base(message){ }
        public ApiViaCepException(string message, Exception ex): base(message, ex){ }
    }
}
