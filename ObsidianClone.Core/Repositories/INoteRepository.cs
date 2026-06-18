using System.IO;

namespace ObsidianClone.Core.Repositories;

public interface INoteRepository
{
  Task<Enumerable<string>> GetNoteNamesAsync(string vaultPath);
  Task<string> ReadNoteContentAsync(string filePath);
  Task WriteNoteContentAsync(string filePath, string markdownContent);
}