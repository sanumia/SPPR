namespace Lab1.BlazorWasm.Models
{
    public class ResponseData<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public static ResponseData<T> Error(string message) => new()
        {
            Success = false,
            ErrorMessage = message
        };
    }
}
