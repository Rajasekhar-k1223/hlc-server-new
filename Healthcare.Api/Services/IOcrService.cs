namespace Healthcare.Api.Services
{
    public interface IOcrService
    {
        Task<string> ExtractTextFromImageAsync(Stream imageStream);
    }
}
