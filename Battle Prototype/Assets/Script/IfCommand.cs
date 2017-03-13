using System.Collections;

public class IfCommand : ScriptCommand {
    private Condition condition;
    private ScriptBlock ifBlock;
    private ScriptBlock elseBlock = null;

    public IfCommand(Condition condition, ScriptBlock ifBlock) {
        this.condition = condition;
        this.ifBlock = ifBlock;
    }

    public void setElseBlock(ScriptBlock elseBlock) {
        this.elseBlock = elseBlock;
    }

    public override IEnumerator execute(ExecutionContext context) {
        ScriptBlock block = condition.evaluate(context.getGameState()) ? ifBlock : elseBlock;

        if (block != null) {
            var iter = block.run(context);

            while (iter != null && iter.MoveNext()) {
                yield return null;
            }
        }
    }
}