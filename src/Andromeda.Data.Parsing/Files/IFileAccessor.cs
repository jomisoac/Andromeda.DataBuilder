namespace Andromeda.Data.Parsing.Files
{
    public interface IFileAccessor
    {
        string Name { get; }
        string FullPath { get; }
        byte[] Data { get; }
    }
}
