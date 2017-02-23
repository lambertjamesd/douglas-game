using System.Linq;

public class NotCondition : Condition {
    private Condition subCondition;

    public NotCondition(Condition subCondition) {
        this.subCondition = subCondition;
    }

    public bool evaluate(GameState state) {
        return !subCondition.evaluate(state);
    }
}