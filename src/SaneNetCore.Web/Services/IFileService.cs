namespace scanner.Services
{
    public interface IFileService
    {
        string[] GetDirectories();
        string[] GetFiles(string path);
        string GetPath(string folder, string file);
    }
}