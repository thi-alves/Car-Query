namespace CarQuery.Models
{
    public class OperationResult
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }

        public OperationResult() { }

        public OperationResult(bool succeeded, string message) {
            Succeeded = succeeded;
            Message = message;
        }
    }
}
