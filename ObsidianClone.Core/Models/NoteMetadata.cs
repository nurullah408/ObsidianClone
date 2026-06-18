namespace ObsidianClone.Models;

public record NoteMetadata(
  string FilePath,
  string Title,
  HashSet<string> OutgoingLinks,
  HashSet<string> BackLinks,
  HashSet<string> Tags
);