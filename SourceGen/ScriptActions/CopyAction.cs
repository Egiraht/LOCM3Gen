using System.IO;
using System.Xml.Linq;

namespace LOCM3Gen.SourceGen.ScriptActions
{
  /// <summary>
  /// Script action for copying files matching the file pattern from one directory to another.
  /// </summary>
  [ActionName("copy")]
  public class CopyAction : ScriptAction
  {
    /// <summary>
    /// Source directory where to copy files from.
    /// Parameter is parsed.
    /// </summary>
    [ActionParameter("source-dir", true)]
    public string SourceDirectory { get; set; } = "";

    /// <summary>
    /// Target directory where to copy files to.
    /// Parameter is parsed.
    /// </summary>
    [ActionParameter("target-dir", true)]
    public string TargetDirectory { get; set; } = "";

    /// <summary>
    /// File pattern for choosing the files to be copied.
    /// </summary>
    [ActionParameter("file-pattern", false)]
    public string FilePattern { get; set; } = "";

    /// <summary>
    /// If "true" search files in subdirectories, otherwise search only the top level directory.
    /// </summary>
    [ActionParameter("recursive", false)]
    public bool Recursive { get; set; }

    /// <summary>
    /// If "true" parse every file during copying.
    /// </summary>
    [ActionParameter("parse", false)]
    public bool Parse { get; set; }

    /// <summary>
    /// If "true" existing files will not be overwritten during copying.
    /// </summary>
    [ActionParameter("keep-existing", false)]
    public bool KeepExistingFiles { get; set; }

    /// <inheritdoc />
    public CopyAction(XElement actionXmlElement, ScriptDataContext dataContext, ScriptAction parentAction)
      : base(actionXmlElement, dataContext, parentAction)
    {
    }

    /// <inheritdoc />
    public override void Invoke()
    {
      if (string.IsNullOrWhiteSpace(SourceDirectory) || !Directory.Exists(SourceDirectory))
        throw new ScriptException($"Invalid source directory: \"{SourceDirectory}\".", ActionXmlElement, "source-dir");

      if (string.IsNullOrWhiteSpace(TargetDirectory))
        throw new ScriptException("Empty target directory provided.", ActionXmlElement, "target-dir");

      var sourceDirectory = Path.GetFullPath(SourceDirectory);
      foreach (var fileName in Directory.EnumerateFiles(sourceDirectory, FilePattern,
        Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
      {
        var targetFileName = fileName.Replace(sourceDirectory, TargetDirectory);
        var targetFileDirectory = Path.GetDirectoryName(targetFileName);

        if (!Directory.Exists(targetFileDirectory))
        {
          // ReSharper disable once AssignNullToNotNullAttribute
          Directory.CreateDirectory(targetFileDirectory);
        }

        if (KeepExistingFiles && File.Exists(targetFileName))
          continue;

        File.Copy(fileName, targetFileName, true);

        if (Parse)
          ParseFile(targetFileName);
      }
    }
  }
}
