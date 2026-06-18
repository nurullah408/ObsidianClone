using System.IO;

namespace Obsidian.Core.Repositories;

public class LocalFileNoteRepository : INoteRepository
{
  public static Task<Enumerable<string>> GetNoteNamesAsync(string vaultPath)
  {
    if (!Directory.Exists(vaultPath))
    {
      throw new DirectoryNotFoundException($"The vault directory wasn't found ${vaultPath}");
    }

    var files = Directory.EnumerateFiles(vaultPath, "*.md", SearchOption.AllDirectories);
    return Task.FromResult(files);
  }
  public static async Task<string> ReadNoteContentAsync(string filePath)
  {
    return await File.ReadAllTextAsync(filePath);
  }
  public static async Task WriteNoteContentAsync(string filePath, string markdownContent)
  {
    await File.WriteAllTextAsync(filePath, markdownContent);
  }
}