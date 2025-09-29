using System.Text.Json.Serialization;

namespace Suggestions.API.Contracts;

public class ErrorModel
{
    public ErrorModel() { }

    public ErrorModel(string key, string[] messages)
    {
        Key = key;
        Messages = messages;
    }
    public string Key { get; set; }

    public string[] Messages { get; set; }
}

public class BaseResponse<TOutput>
{
    public BaseResponse()
    {

    }

    public BaseResponse(TOutput output)
    {
        Data = output;
        Message = string.Empty;
    }

    [JsonPropertyName("data")]
    public TOutput Data { get; set; }
    [JsonPropertyName("errors")]
    public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();

    /// <summary>
    /// global validation message. It is used as a success message if Success is false, 
    /// otherwise it is an error message
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("success")]
    public bool Success { get; set; } = true;

    public BaseResponse<TOutput> AddErrorResponse(string key, string[] errorMessages)
    {
        Errors.Add(new ErrorModel(key, errorMessages));
        Success = false;
        return this;
    }
    //public BaseResponse<TOutput> ErrorResponse(string key, string errorMessage)
    //{
    //    var errorResponse = new BaseResponse<TOutput>();
    //    errorResponse.Errors.Add(new ErrorModel(key, errorMessage));
    //    Success = false;
    //    return errorResponse;
    //}
}