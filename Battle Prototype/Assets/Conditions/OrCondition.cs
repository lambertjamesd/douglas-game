using System.Linq;

public class OrCondition : Condition {
    private Condition[] subConditions;

    public OrCondition(Condition[] subConditions) {
        this.subConditions = subConditions;
    }

    public bool evaluate(GameState state) {
        return subConditions.Any(subCondition => subCondition.evaluate(state));
    }
}