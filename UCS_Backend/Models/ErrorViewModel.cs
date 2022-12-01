namespace UCS_Backend.Models;

public class ErrorViewModel

 /// <summary>
    /// Creates a class for ErroViewModel
    /// </summary> 
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}

