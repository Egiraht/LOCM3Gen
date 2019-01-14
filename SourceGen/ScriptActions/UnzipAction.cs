using System.IO;
using System.IO.Compression;
using System.Xml.Linq;

namespace LOCM3Gen.SourceGen.ScriptActions
{
  /// <summary>
  /// Script action for unzipping a file from the archive.
  /// </summary>
  [ActionName("unzip")]
  public class UnzipAction : ScriptAction
  {
    /// <summary>
    /// Path to the zip-archive file.
    /// Parameter is parsed.
    /// </summary>
    [ActionParameter("archive", true)]
    public string ArchivePath { get; set; }

    /// <summary>
    /// Internal path in the archive of the file to be extracted.
    /// Parameter is parsed.
    /// </summary>
    [ActionParameter("entry", true)]
    public string EntryPath { get; set; }

    /// <summary>
    /// Target directory where the file will be extracted.
    /// Parameter is parsed.
    /// </summary>
    [ActionParameter("target-dir", true)]
    public string TargetDirectory { get; set; }

    /// <summary>
    /// If "true" existing file will not be overwritten during extraction.
    /// </summary>
    [ActionParameter("keep-existing", false)]
    public string KeepExistingFiles { get; set; }

    /// <inheritdoc />
    public UnzipAction(XElement actionXmlElement, ScriptDataContext dataContext, ScriptAction parentAction)
      : base(actionXmlElement, dataContext, parentAction)
    {
    }

    /// <inheritdoc />
    public override void Invoke()
    {
      var archivePath = Path.GetFullPath(ArchivePath);
      if (string.IsNullOrWhiteSpace(archivePath) || !File.Exists(archivePath))
      {
        // TODO: Wrong zip archive path processing.
        return;
      }

      using (var archive = ZipFile.OpenRead(archivePath))
      {
        var entryStream = archive.GetEntry(EntryPath);
        if (entryStream == null)
        {
          // TODO: Wrong zip archive entry path.
          return;
        }

        if (!Directory.Exists(TargetDirectory))
          Directory.CreateDirectory(TargetDirectory);

        var targetFileName = Path.Combine(TargetDirectory, Path.GetFileName(EntryPath));
        if (KeepExistingFiles.ToLower() == "true" && File.Exists(targetFileName))
          return;

        using (var targetFile = new StreamWriter(targetFileName))
        {
          entryStream.Open().CopyTo(targetFile.BaseStream);
          targetFile.Flush();
        }
      }
    }
  }
}