
public abstract class NamedNumericalValue : NumericalValue {
    private string name;

    public NamedNumericalValue(string name) {
        this.name = name;
    }

    public override double evaluate(GameState state) {
        return state.getNumber(name);
    }
}