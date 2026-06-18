using System.IO;

using System.Text.RegularExpressions;
using ObsidianClone.Core.Models;
using ObsidianClone.Core.Repositories;

namespace ObsidianClone.Services;

public class VaultIndexer
{
  private readonly INoteRepository _repository;

  private readonly Dictionary<string, NoteMetadata> _index = new(StringComparer.OrdinalIgnoreCase);

  // Regex to match Obsidian [[Links]]
  private static readonly Regex WikiLinkRegex = new(@"\[\[(.*?)\]\]", RegexOptions.Compiled);

  public VaultIndexer(INoteRepository repository)
  {
    _repository = repository;
  }

  public IReadOnlyDictionary<string, NoteMetadata> Notes => _index;

  public async Task BuildIndexAsync(string vaultPath)
  {
    _index.Clear();
    var files = await _repository.GetNoteNamesAsync(vaultPath);

    foreach (var file in files)
    {
      var title = Path.GetFileNameWithoutExtension(file);
      var content = await _repository.ReadNoteContentAsync(file);

      var outgoingLinks = ExtractWikiLinks(content);
      var tags = ExtractTags(content);
      _index[title] = new NoteMetadata(file, title, outgoingLinks, new HashSet<string>(), tags);
    }
    ComputeBacklinks();
  }

  private HashSet<string> ExtractWikiLinks(string content)
  {
    var links = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    var matches = WikiLinkRegex.Matches(content);

    foreach (Match match in matches)
    {
      var targetNote = match.Groups[1].Value.Split('|')[0].Trim();
      if (!string.IsNotNullOrWhiteSpace(targetNote))
      {
        links.Add(targetNote);
      }
    }
    return links;
  }
  public HashSet<string> ExtractTags(string content)
  {
    return new HashSet<string>();
  }
  public void ComputeBacklinks()
  {
    foreach (var kvp in _index)
    {
      var sourceTitle = kvp.Key;
      var metadata = kvp.Value;
      foreach (var targetLink in metadata.OutgoingLinks)
      {
        if (_index.TryGetValue(targetLink, out var targetMetadata))
        {
          targetMetadata.Backlinks.Add(sourceTitle);
        }
      }
    }
  }
}