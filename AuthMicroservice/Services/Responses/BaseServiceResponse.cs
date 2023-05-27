namespace AuthMicroservice.Services.Responses;

public abstract class BaseServiceResponse
{
    public bool Success { get; set; } = true;
    public string ErrorMessage { get; set; }
    public Errors Error { get; set; } = Errors.Ok;
}