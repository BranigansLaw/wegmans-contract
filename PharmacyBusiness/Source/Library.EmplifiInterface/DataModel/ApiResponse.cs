using System.Text;

namespace Library.EmplifiInterface.DataModel;

public class ApiResponse
{
    public string? Status { get; set; }
    public List<ApiResponseMessage>? Messages { get; set; }

    public ApiResponse(string? status, List<ApiResponseMessage>? messages)
    {
        Status = status;
        Messages = messages;
    }

    public string BuildExceptionMessage()
    {
        var exceptionMessage = new StringBuilder();

        exceptionMessage.Append($"API Status: [{Status}]");

        if (Messages is not null)
        {
            foreach (var message in Messages)
            {
                exceptionMessage.Append($", {message.Message}");
                if (message.Substitutions is not null)
                {
                    foreach (var substitution in message.Substitutions)
                    {
                        exceptionMessage.Append($", {substitution}");
                    }
                }
            }
        }

        return exceptionMessage.ToString();
    }
}

public class ApiResponseMessage
{
    public string? Message { get; set; }
    public List<string>? Substitutions { get; set; }
}
