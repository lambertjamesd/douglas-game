public class BooleanCondition : Condition {
    private string booleanName;

    public OrCondition(string booleanName) {
        this.booleanName = booleanName;
    }

    public bool evaluate(GameState state) {
        return state.getBoolean(booleanName);
    }
}