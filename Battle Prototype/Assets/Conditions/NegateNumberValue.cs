public class NegateNumberValue : NumericalValue {
    private NumericalValue ofValue;

    public NegateNumberValue(NumericalValue ofValue) {
        this.ofValue = ofValue;
    }

    public override double evaluate(GameState state) {
        return -ofValue.evaluate(state);
    }
}