namespace BetterAddressBook.Services.Interfaces;

public interface IImageService
{
    public Task<byte[]> ConvertToByteArrayAsync(IFormFile file);
    public string ConvertToFile(byte[] fileData, string extension);
}