using Tesseract;

namespace Healthcare.Api.Services
{
    public class OcrService : IOcrService
    {
        private readonly TesseractEngine _engine;

        public OcrService()
        {
            // Assuming tessdata is in the root or configured path
            // For now, initializing with English
            // You MUST ensure tessdata/*.traineddata exists at runtime
            _engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
        }

        public async Task<string> ExtractTextFromImageAsync(Stream imageStream)
        {
            using var memoryStream = new MemoryStream();
            await imageStream.CopyToAsync(memoryStream);
            var imageBytes = memoryStream.ToArray();

            using var pix = Pix.LoadFromMemory(imageBytes);
            using var page = _engine.Process(pix);
            
            return page.GetText();
        }
    }
}
