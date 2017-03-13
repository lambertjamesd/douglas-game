using System;
using System.Collections;
using System.Collections.Generic;

class StackPosition {
    public string whitespace;
    public ScriptBlock scriptBlock;

    public StackPosition(string whitespace, ScriptBlock scriptBlock) {
        this.whitespace = whitespace;
        this.scriptBlock = scriptBlock;
    }
}

public class GameScriptParseError : Exception {
    public GameScriptParseError(string message, int line) : base(message + " on line " + (line + 1)) {

    }
}

public class SingleLine {
    private string line;
    private string leadingSpace;
    private string command;
    private string commandParameters;

    public SingleLine(string line) {
        this.line = line.Trim();

        int resultIndex = 0;

        while (resultIndex < line.Length && Char.IsWhiteSpace(line[resultIndex])) {
            ++resultIndex;
        }

        leadingSpace = line.Substring(0, resultIndex);

        if (this.hasContent() && this.line[0] == '(') {
            var commandParts = this.line.Substring(1, this.line.Length - 2).Split(null, 2);

            command = commandParts[0];

            if (commandParts.Length > 1) {
                commandParameters = commandParts[1];
            } else {
                commandParameters = "";
            }
        }
    }

    public string getContent() {
        return line;
    }

    public bool hasContent() {
        return line.Length > 0;
    }

    public string getLeadingSpace() {
        return leadingSpace;
    }

    public string getCommand() {
        return command;
    }

    public string getCommandParameters() {
        return commandParameters;
    }
}

public class GameScriptParser {
    private string[] lines;
    private Stack<StackPosition> stackPositions = new Stack<StackPosition>();
    private int currentLine = 0;

    private GameScriptParser(string source) {
        lines = source.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
    }

    private StackPosition getTop() {
        if (stackPositions.Count > 0) {
            return stackPositions.Peek();
        } else {
            return null;
        }
    }

    private string peekNext() {
        if (currentLine < lines.Length) {
            return lines[currentLine];
        } else {
            return "";
        }
    }

    private SingleLine peekNextLine() {
        return new SingleLine(peekNext());
    }

    private bool hasNext() {
        return currentLine < lines.Length;
    }

    private void moveNext() {
        ++currentLine;
    }

    private void parseBlock(ScriptBlock block) {
        StackPosition last = getTop();
        string expectedPreSpace = last == null ? null : last.whitespace;
        StackPosition current = new StackPosition(null, block);
        stackPositions.Push(current);

        while (this.hasNext()) {
            var next = peekNextLine();

            // ignore full whitespace lines
            if (next.hasContent()) {
                var leadingSpace = next.getLeadingSpace();

                if (current.whitespace == null) {
                    if (expectedPreSpace == null ||
                        expectedPreSpace.Length < leadingSpace.Length && leadingSpace.Substring(0, expectedPreSpace.Length) == leadingSpace
                    ) {
                        current.whitespace = leadingSpace;
                    } else {
                        throw new GameScriptParseError("Expected indentation but none found", currentLine);
                    }
                }

                if (leadingSpace != current.whitespace) {
                    if (leadingSpace.Length < current.whitespace.Length && current.whitespace.Substring(0, leadingSpace.Length) == leadingSpace) {
                        break;
                    } else {
                        throw new GameScriptParseError("Bad whitespace", currentLine);
                    }
                } else {
                    var command = next.getCommand();
                    moveNext();

                    if (command != null) {
                        if (command == "if") {
                            var ifBlock = new ScriptBlock(); 
                            parseBlock(ifBlock);
                            IfCommand result = new IfCommand(ConditionParser.parseCondition(next.getCommandParameters()), ifBlock);

                            var nextLine = peekNextLine();

                            while (leadingSpace == nextLine.getLeadingSpace() && 
                                nextLine.getCommand() == "elseif") {
                                var nextBlock = new ScriptBlock();
                                parseBlock(nextBlock);
                                var nextIf = new IfCommand(ConditionParser.parseCondition(nextLine.getCommandParameters()), nextBlock);
                                result.setElseBlock(ScriptBlock.singleCommand(nextIf));
                                result = nextIf;
                                nextLine = peekNextLine();
                            }

                            if (leadingSpace == nextLine.getLeadingSpace() &&
                                nextLine.getCommand() == "else") {
                                var elseBlock = new ScriptBlock();
                                result.setElseBlock(elseBlock);
                            }
                        }
                    } else {
                        block.addCommand(new TextCommand(next.getContent()));
                    }
                }
            }
        }

        stackPositions.Pop();
    }

    private GameScript parse() {
        var block = new ScriptBlock();
        GameScript result = new GameScript(block);

        return result;

    }

    public static GameScript parse(string source) {
        return new GameScriptParser(source).parse();
    }
}