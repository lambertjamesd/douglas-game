public class LessThanCondition : Condition {
    private NumericalValue left;
    private NumericalValue right;

    public LessThanCondition(NumericalValue left, NumericalValue right) {
        this.left = left;
        this.right = right;
    }

    public bool evaluate(GameState state) {
        return left.evaluate(state) < right.evaluate(state);
    }
}