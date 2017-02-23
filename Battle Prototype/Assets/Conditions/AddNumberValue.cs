public class AddNumberValue : NumericalValue {
    private NumericalValue left;
    private NumericalValue right;

    public AddNumberValue(NumericalValue left, NumericalValue right) {
        this.left = left;
        this.right = right;
    }

    public override double evaluate(GameState state) {
        return left.evaluate(state) + right.evaluate(state);
    }
}