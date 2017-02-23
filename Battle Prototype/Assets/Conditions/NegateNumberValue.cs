public class NegateNumberValue : NumericalValue {
    private NumericalValue ofValue;

    public LessThanCondition(NumericalValue ofValue) {
        this.ofValue = ofValue;
    }

    public double evaluate(GameState state) {
        return -ofValue.evaluate(state);
    }
}