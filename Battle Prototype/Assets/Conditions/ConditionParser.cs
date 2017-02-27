using System;
using System.Collections;
using System.Collections.Generic;

public enum OperatorPrecedence {
    And,
    Or,
    Compare,
    Add,
    Max,
}

abstract class  TreeNode {
    public abstract Condition buildCondition();
    public abstract NumericalValue buildNumberValue();
}

public class ParseError : Exception {
    public ParseError(string message) :
        base(message) {

    }
}

class StringValue : TreeNode {
    public string stringValue;

    public StringValue(string stringValue) {
        this.stringValue = stringValue;
    }

    public override Condition buildCondition() {
        return new BooleanCondition(stringValue);
    }

    public override NumericalValue buildNumberValue() {
        return new NamedNumericalValue(stringValue);
    }
}

class UnaryOperator : TreeNode  {
    public string op;
    public TreeNode internalStringValue;

    public UnaryOperator(string op, TreeNode internalStringValue) {
        this.op = op;
        this.internalStringValue = internalStringValue;
    }

    public override Condition buildCondition() {
        if (op == "not") {
            return new NotCondition(internalStringValue.buildCondition());
        } else {
            throw new ParseError(op + " is not a valid boolean unary operator");
        }
    }

    public override NumericalValue buildNumberValue() {
        if (op == "-") {
            return new NegateNumberValue(internalStringValue.buildNumberValue());
        } else {
            throw new ParseError(op + " is not a valid number unary operator");
        }
    }
}

class BinaryOperator : TreeNode {
    public string op;
    public TreeNode left;
    public TreeNode right;

    public BinaryOperator(string op, TreeNode left, TreeNode right) {
        this.op = op;
        this.left = left;
        this.right = right;
    }

    public override Condition buildCondition() {
        if (op == "and") {
            return new AndCondition(new Condition[]{left.buildCondition(), right.buildCondition()});
        } else if (op == "or") {
            return new OrCondition(new Condition[]{left.buildCondition(), right.buildCondition()});
        } else if (op == ">=" || op == "<=" || op == ">" || op == "<") {
            NumericalValue leftValue = left.buildNumberValue();
            NumericalValue rightValue = right.buildNumberValue();

            if (op == "<=") {
                return new NotCondition(new LessThanCondition(rightValue, leftValue));
            } else if (op == ">") {
                return new LessThanCondition(rightValue, leftValue);
            } else if (op == ">=") {
                return new NotCondition(new LessThanCondition(leftValue, rightValue));
            } else {
                return new LessThanCondition(leftValue, rightValue);
            }
        } else {
            throw new ParseError(op + " is not a valid boolean binary operator");
        }
    }

    public override NumericalValue buildNumberValue() {
        if (op == "-") {
            return new AddNumberValue(left.buildNumberValue(), new NegateNumberValue(right.buildNumberValue()));
        } else if (op == "+") {
            return new AddNumberValue(left.buildNumberValue(), right.buildNumberValue());
        } else {
            throw new ParseError(op + " is not a valid number binary operator");
        }
    }
}


public class ConditionParser {
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

    public static Condition parseCondition(string source) {
        ConditionParser parser = new ConditionParser(source);
        TreeNode tree = parser.parseBinaryOperator();
        return tree.buildCondition();
    }

    public static NumericalValue parseNumerValue(string source) {
        ConditionParser parser = new ConditionParser(source);
        TreeNode tree = parser.parseBinaryOperator();
        return tree.buildNumberValue();
    }

    public ConditionParser(string source) {
        this.tokens = source.Split(null);
    }

    private bool isBinaryOperator() {
        return operatorPrecedence.ContainsKey(this.peek());
    }

    private TreeNode parseBinaryOperator(OperatorPrecedence minPrecedence = OperatorPrecedence.Max) {
        TreeNode result = parseUnaryOperator();

        while (isBinaryOperator()) {
            string op = peek();
            OperatorPrecedence precendence = operatorPrecedence[op];

            if (precendence >= minPrecedence) {
                advance();
                result = new BinaryOperator(op, result, this.parseBinaryOperator(minPrecedence + 1));
            } else {
                break;
            }
        }

        return result;
    }

    private TreeNode parseUnaryOperator() {
        if (optional("not")) {
            return new UnaryOperator("not", parseBinaryOperator(OperatorPrecedence.Compare));
        } else {
            return parseSingleStringValue();
        }
    }

    private TreeNode parseSingleStringValue() {
        if (optional("(")) {
            var result = parseBinaryOperator();
            optional(")");
            return result;
        } else {
            return new StringValue(this.next());
        }
    }
}