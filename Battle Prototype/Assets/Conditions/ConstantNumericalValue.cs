
public abstract class ConstantNumericalValue : NumericalValue {
    private double constantValue = 0.0;

    public ConstantNumericalValue(double constantValue) {
        this.constantValue = constantValue;
    }

    public double evaluate(GameState state) {
        return constantValue;
    }
}