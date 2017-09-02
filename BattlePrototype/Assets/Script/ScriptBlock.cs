using System.Collections;
using System.Collections.Generic;
using System.Text;

public class ScriptBlock : IEnumerable<ScriptCommand> {
    private List<ScriptCommand> commands = new List<ScriptCommand>();

    public ScriptBlock() {

    }

    public void addCommand(ScriptCommand command) {
        commands.Add(command);
    }

    public static ScriptBlock singleCommand(ScriptCommand command) {
        var result = new ScriptBlock();
        result.addCommand(command);
        return result;
    }

    public IEnumerator<ScriptCommand> GetEnumerator() {
        return commands.GetEnumerator();
    }

    System.Collections.IEnumerator IEnumerable.GetEnumerator() {
        return this.GetEnumerator();
    }
    
    public IEnumerator run(ExecutionContext context) {
        if (commands != null) {
            foreach (ScriptCommand command in commands) {
                var current = command.execute(context);

                while (current != null && current.MoveNext()) {
                    yield return null;
                }
            }
        }
    }

    public string ToString(int depth)
    {
        StringBuilder result = new StringBuilder();

        foreach (var command in commands)
        {
            for (int i = 0; i < depth; ++i)
            {
                result.Append("    ");
            }
            result.Append(command.ToString(depth));
            result.Append("\n");
        }

        return result.ToString();
    }

    public override string ToString()
    {
        return ToString(0);
    }
}