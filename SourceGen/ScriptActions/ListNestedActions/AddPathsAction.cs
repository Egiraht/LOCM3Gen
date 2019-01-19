using System.IO;
using System.Xml.Linq;

namespace LOCM3Gen.SourceGen.ScriptActions.ListNestedActions
{
  /// <summary>
  /// Nested script action for the <see cref="ListAction" />.
  /// Collects all the file paths in the directory that match the file pattern and adds them to the list.
  /// </summary>
  [ActionName("add-paths")]
  public class AddPathsAction : ScriptAction
  {
    /// <summary>
    /// Source directory where the files will be searched.
    /// Parameter is parsed.
    /// </summary>
    [ActionParameter("source-dir", true)]
    public string SourceDirectory { get; set; } = "";

    /// <summary>
    /// File pattern for choosing the file paths to be collected.
    /// </summary>
    [ActionParameter("file-pattern", false)]
    public string FilePattern { get; set; } = "";

    /// <summary>
    /// If "true" search files in subdirectories, otherwise search only the top level directory.
    /// </summary>
    [ActionParameter("recursive", false)]
    public bool Recursive { get; set; }

    /// <summary>
    /// If "true" file paths will be added as relative to the source path, otherwise absolute paths will be added.
    /// </summary>
    [ActionParameter("relative", false)]
    public bool Relative { get; set; }

    /// <inheritdoc />
    public AddPathsAction(XElement actionXmlElement, ScriptDataContext dataContext, ScriptAction parentAction)
      : base(actionXmlElement, dataContext, parentAction)
    {
    }

    /// <inheritdoc />
    public override void Invoke()
    {
      if (!(ParentAction is ListAction))
        throw new ScriptException("No parent \"list\" action provided.", ActionXmlElement);

      if (string.IsNullOrWhiteSpace(SourceDirectory) || !Directory.Exists(SourceDirectory))
        throw new ScriptException($"Invalid source directory: \"{SourceDirectory}\".", ActionXmlElement, "source-dir");

      var sourceDirectory = Path.GetFullPath(SourceDirectory);
      var list = ((ListAction) ParentAction).ListValues;
      foreach (var fileName in Directory.EnumerateFiles(sourceDirectory, FilePattern,
        Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
      {
        list.Add(Relative ? fileName.Replace(sourceDirectory, ".") : fileName);
      }
    }
  }
}
