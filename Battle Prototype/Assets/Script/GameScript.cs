using System.Collections;

public interface ExecutionContext {
    IEnumerator emitText(string text);
    void setNumber(string name, double value);
    void setBoolean(string name, bool value);
    void setString(string name, string value);
    GameState getGameState();
}

public abstract class ScriptCommand {
    public abstract IEnumerator execute(ExecutionContext context);
}

public class GameScript {
    public ScriptBlock commands;

    public GameScript(ScriptBlock commands) {
        this.commands = commands;
    }

    public IEnumerator run(ExecutionContext context) {
        if (commands != null) {
            foreach (ScriptCommand command in commands) {
                var current = command.execute(context);

                while (current.MoveNext()) {
                    yield return null;
                }
            }
        }
    }
}