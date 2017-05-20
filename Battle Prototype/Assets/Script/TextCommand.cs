using System.Collections;
using System.Text;

public class TextCommand : ScriptCommand {
    private string message;

    public TextCommand(string message) {
        this.message = message;
    }

    public override IEnumerator execute(ExecutionContext context) {
        return context.emitText(message);
    }

    public override string ToString(int depth)
    {
        StringBuilder result = new StringBuilder();
        for (var i = 0; i < depth; ++i)
        {
            result.Append("    ");
        }
        result.Append(message);
        return result.ToString();
    }
}