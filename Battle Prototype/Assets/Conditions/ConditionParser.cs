using System.Collection;
using System.Collection.Generic;

public enum OperatorPrecedence {
    And,
    Or,
    Compare,
    Add,
    Max,
}

public class ConditionParser {
    private static HashSet<string> BinaryOperators = new HashSet<string>(new string[]{
        "and",
        "or",
        "+",
        "-",
        "<",
        ">",
        "<=",
        ">=",
    });

    private static Dictionary<string, OperatorPrecedence> operatorPrecedence = new Dictionary<string, OperatorPrecedence> {
        { "and", OperatorPrecedence.And }, 
        { "or", OperatorPrecedence.Or }, 
        { "<", OperatorPrecedence.Compare },
        { ">", OperatorPrecedence.Compare },
        { "<=", OperatorPrecedence.Compare },
        { ">=", OperatorPrecedence.Compare },
        { "+", OperatorPrecedence.Add }, 
        { "-", OperatorPrecedence.Add },
    };

    private string[] tokens;
    private int currentLocation = 0;

    private string peek(int offset = 0) {
        if (currentLocation + offset >= 0 && currentLocation + offset < tokens.Length) {
            return tokens[currentLocation + offset];
        } else {
            return "";
        }
    }

    private void advance() {
        ++currentLocation;
    }

    private bool optional(string next) {
        if (peek() == next) {
            advance();
            return true;
        } else {
            return false;
        }
    }

    private string next() {
        var result = peek();
        advance();
        return result;
    }

    public ConditionParser(string source) {
        this.tokens = source.Split(null);
    }

    public Condition parseCondition() {

    }

    public Condition parseBinaryCondition(OperatorPrecedence precendence = OperatorPrecedence.Max) {

    }

    public Condition parseUnaryCondition() {
        if (optional("not")) {
            return new NotCondition(parseUnaryCondition());
        } else {
            return parseSingleCondition();
        }
    }

    public bool isCompareNext() {
        var next = peek(1);
        return next == "<" || next == ">" || next == "<=" || next == ">=";
    }

    public Condition parseSingleCondition() {
        if (optional("(")) {
            var result = parseBinaryCondition();
            optional(")");
            return result;
        } else if (isCompareNext()) {
            var left = parseNumberValue();

        } else {
            return new BooleanCondition(next());
        }
    }

    public NumericalValue parseNumberValue() {

    }
}