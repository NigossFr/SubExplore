namespace SubExplore.Models.DTOs
{
    public interface IFormFile
    {
        string FileName { get; }
        string ContentType { get; }
        long Length { get; }
        Stream OpenReadStream();
    }
}
