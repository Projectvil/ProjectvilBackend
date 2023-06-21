using System.Text.Json.Serialization;

namespace AuthMicroservice.Services.Responses;

public class ErrorViewModel
{
    [JsonPropertyName("error_message")]
    public string ErrorMessage { get; set; }
}