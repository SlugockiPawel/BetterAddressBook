using BetterAddressBook.Services.Interfaces;

namespace BetterAddressBook.Services;

public class ImageService : IImageService
{
    private const string DefaultImage = "img/DefaultContactImage.png";
    private readonly string[] _suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };

    public async Task<byte[]> ConvertToByteArrayAsync(IFormFile file)
    {
        try
        {
            using MemoryStream memoryStream = new();
            await file.CopyToAsync(memoryStream);
            var byteFile = memoryStream.ToArray();
            return byteFile;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public string ConvertToFile(byte[] fileData, string extension)
    {
        if (fileData is null)
            return DefaultImage;

        try
        {
            var imageBase64Data = Convert.ToBase64String(fileData);
            return string.Format($"data:{extension};base64,{imageBase64Data}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
}