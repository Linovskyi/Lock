namespace Consumer.Core.SignupService
{
    public class ServiceResponse
    {
        public bool IsSuccess { get; protected set; }

        public List<string> Errors { get; } = new();

        public void AddSuccess()
        {
            IsSuccess = true;
        }

        public void AddError(string error)
        {
            IsSuccess = false;
            Errors.Add(error);
        }

        public void AddError(IEnumerable<string> errors)
        {
            IsSuccess = false;
            Errors.AddRange(errors);
        }
    }

    public sealed class ServiceResponse<T> : ServiceResponse
    {
        public T? Value { get; private set; }

        public ServiceResponse() { }

        public ServiceResponse(T responseValue)
        {
            Value = responseValue;
            IsSuccess = true;
        }

        public void AddSuccessValue(T? value)
        {
            Value = value;
            IsSuccess = true;
        }

        public void AddFailureValue(T? value)
        {
            Value = value;
            IsSuccess = false;
        }

        public void AddFailureValue(T? value, string error)
        {
            Value = value;
            IsSuccess = false;
            Errors.Add(error);
        }

        public void AddFailureValue(T? value, IEnumerable<string> errors)
        {
            Value = value;
            IsSuccess = false;
            Errors.AddRange(errors);
        }
    }

}
