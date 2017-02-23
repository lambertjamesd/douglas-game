using System.Linq;

public class AndCondition : Condition {
    private Condition[] subConditions;

    public AndCondition(Condition[] subConditions) {
        this.subConditions = subConditions;
    }

    public bool evaluate(GameState state) {
        return subConditions.All(subCondition => subCondition.evaluate(state));
    }
}