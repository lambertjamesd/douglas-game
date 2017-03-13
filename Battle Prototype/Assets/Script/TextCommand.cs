using System.Collections;

public class TextCommand : ScriptCommand {
    private string message;

    public TextCommand(string message) {
        this.message = message;
    }

    public override IEnumerator execute(ExecutionContext context) {
        return context.emitText(message);
    }
}