public class BooleanCondition : Condition {
    private string booleanName;

    public BooleanCondition(string booleanName) {
        this.booleanName = booleanName;
    }

    public override bool evaluate(GameState state) {
        return state.getBoolean(booleanName);
    }
}