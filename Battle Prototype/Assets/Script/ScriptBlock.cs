using System.Collections;
using System.Collections.Generic;

public class ScriptBlock : IEnumerable<ScriptCommand> {
    private List<ScriptCommand> commands = new List<ScriptCommand>();

    public ScriptBlock() {

    }

    public void addCommand(ScriptCommand command) {
        commands.Add(command);
    }

    public IEnumerator<ScriptCommand> GetEnumerator() {
        return commands.GetEnumerator();
    }

    System.Collections.IEnumerator IEnumerable.GetEnumerator() {
        return this.GetEnumerator();
    }
}