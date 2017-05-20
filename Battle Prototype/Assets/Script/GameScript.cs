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
    public abstract string ToString(int depth);
}

public class GameScript {
    public ScriptBlock commands;

    public GameScript(ScriptBlock commands) {
        this.commands = commands;
    }

    public IEnumerator run(ExecutionContext context) {
        if (commands != null) {
            var iter = commands.run(context);

            while (iter != null && iter.MoveNext()) {
                yield return null;
            }
        }
    }

    public override string ToString()
    {
        return commands.ToString();
    }
}