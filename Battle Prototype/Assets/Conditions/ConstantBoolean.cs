public class ConstantBoolean : Condition {
    private bool booleanValue;

    public ConstantBoolean(bool booleanValue) {
        this.booleanValue = booleanValue;
    }

    public override bool evaluate(GameState state) {
        return booleanValue;
    }
}