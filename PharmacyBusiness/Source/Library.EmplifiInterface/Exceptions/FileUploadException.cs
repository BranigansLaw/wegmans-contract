namespace Library.EmplifiInterface.Exceptions;

public class FileUploadException : Exception
{
    public FileUploadException(string? message, string fileName) : base(message) 
    {
        FileName = fileName;
    }
    public string FileName { get; set; }
}
