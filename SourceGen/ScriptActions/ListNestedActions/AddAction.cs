using System.Xml.Linq;

namespace LOCM3Gen.SourceGen.ScriptActions.ListNestedActions
{
  /// <summary>
  /// Nested script action for the <see cref="ListAction" />.
  /// Adds the value from the list.
  /// </summary>
  [ActionName("add")]
  public class AddAction : ScriptAction
  {
    /// <summary>
    /// The value to be added to the list.
    /// Parameter is parsed.
    /// </summary>
    [ActionParameter("value", true)]
    public string Value { get; set; }

    /// <inheritdoc />
    public AddAction(XElement actionXmlElement, ScriptDataContext dataContext, ScriptAction parentAction)
      : base(actionXmlElement, dataContext, parentAction)
    {
    }

    /// <inheritdoc />
    public override void Invoke()
    {
      if (!(ParentAction is ListAction))
      {
        // TODO: No parent list processing.
        return;
      }

      var list = ((ListAction) ParentAction).ListValues;
      if (!list.Contains(Value))
        list.Add(Value);
    }
  }
}