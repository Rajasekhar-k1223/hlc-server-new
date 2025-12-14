namespace Healthcare.Api.Services
{
    public interface IAiService
    {
        Task TriggerTrainingAsync(string documentText, string documentType);
        Task<string> AnalyzeDocumentAsync(string documentText);
        Task<string> AnalyzeFileAsync(string filePath);
    }
}
