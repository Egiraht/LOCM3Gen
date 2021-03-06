using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace LOCM3Gen.SourceGen.ScriptActions
{
  /// <summary>
  /// Script action for defining a list of values.
  /// Invokes nested actions to manipulate the values in the list.
  /// </summary>
  [ActionName("list")]
  public class ListAction : ScriptAction
  {
    /// <summary>
    /// List name.
    /// </summary>
    [ActionParameter("name")]
    public string Name { get; set; } = "";

    /// <summary>
    /// List of values that can be accessible by the nested actions.
    /// </summary>
    public List<string> ListValues;

    /// <inheritdoc />
    public ListAction(XElement actionXmlElement, ScriptDataContext dataContext, ScriptAction parentAction)
      : base(actionXmlElement, dataContext, parentAction)
    {
    }

    /// <inheritdoc />
    public override void Invoke()
    {
      if (!Regex.IsMatch(Name, @"^\w+$"))
        throw new ScriptException("Invalid list name provided. Name cannot be empty and must contain only alphanumeric characters and underscores.",
          ActionXmlElement, "name");

      if (!DataContext.Lists.ContainsKey(Name))
        DataContext.Lists.Add(Name, new List<string>());

      ListValues = DataContext.Lists[Name];

      foreach (var nestedXmlElement in NestedXmlElements)
        ScriptReader.ExecuteElement(nestedXmlElement, DataContext, this);
    }
  }
}
