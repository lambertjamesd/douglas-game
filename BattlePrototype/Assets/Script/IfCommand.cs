using System.Collections;
using System.Text;

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

    public override string ToString(int depth)
    {
        StringBuilder result = new StringBuilder();

        for (int i = 0; i < depth; ++i)
        {
            result.Append("    ");
        }

        result.Append("if\n");

        result.Append(ifBlock.ToString(depth + 1));

        if (elseBlock != null)
        {
            for (int i = 0; i < depth; ++i)
            {
                result.Append("    ");
            }

            result.Append("else\n");
            result.Append(elseBlock.ToString(depth + 1));
        }

        return result.ToString();
    }
}