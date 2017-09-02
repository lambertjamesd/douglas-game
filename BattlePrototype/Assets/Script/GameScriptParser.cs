using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void parseBlock(ScriptBlock block)
    {
        StackPosition last = getTop();
        string expectedPreSpace = last == null ? null : last.whitespace;
        StackPosition current = new StackPosition(null, block);
        stackPositions.Push(current);

        int i = 0;

        while (this.hasNext() && i < 10)
        {
            var next = peekNextLine();

            // ignore full whitespace lines
            if (next.hasContent())
            {
                var leadingSpace = next.getLeadingSpace();

                if (current.whitespace == null)
                {
                    if (expectedPreSpace == null ||
                        expectedPreSpace.Length < leadingSpace.Length && leadingSpace.Substring(0, expectedPreSpace.Length) == expectedPreSpace
                    )
                    {
                        current.whitespace = leadingSpace;
                    }
                    else
                    {
                        throw new GameScriptParseError("Expected indentation but none found", currentLine);
                    }
                }

                if (leadingSpace != current.whitespace)
                {
                    if (leadingSpace.Length < current.whitespace.Length && current.whitespace.Substring(0, leadingSpace.Length) == leadingSpace)
                    {
                        break;
                    }
                    else
                    {
                        throw new GameScriptParseError("Bad whitespace", currentLine);
                    }
                }
                else
                {
                    var command = next.getCommand();
                    moveNext();

                    if (command != null)
                    {
                        if (command == "if")
                        {
                            var ifBlock = new ScriptBlock(); 
                            parseBlock(ifBlock);
                            IfCommand result = new IfCommand(ConditionParser.parseCondition(next.getCommandParameters()), ifBlock);
                            block.addCommand(result);

                            var nextLine = peekNextLine();

                            while (leadingSpace == nextLine.getLeadingSpace() && 
                                nextLine.getCommand() == "elseif")
                            {
                                moveNext();
                                var nextBlock = new ScriptBlock();
                                parseBlock(nextBlock);
                                var nextIf = new IfCommand(ConditionParser.parseCondition(nextLine.getCommandParameters()), nextBlock);
                                result.setElseBlock(ScriptBlock.singleCommand(nextIf));
                                result = nextIf;
                                nextLine = peekNextLine();
                            }

                            if (leadingSpace == nextLine.getLeadingSpace() &&
                                nextLine.getCommand() == "else")
                            {
                                moveNext();
                                var elseBlock = new ScriptBlock();
                                parseBlock(elseBlock);
                                result.setElseBlock(elseBlock);
                            }
                        }
                        else if (command == "set")
                        {
                            var parts = next.getCommandParameters().Split(null, 3);

                            if (parts.Length != 3)
                            {
                                throw new GameScriptParseError("set expects 3 parameters", currentLine);
                            }
                            else
                            {
                                var type = parts[0];
                                var name = parts[1];
                                var value = parts[2];
                                if (type == "bool")
                                {
                                    block.addCommand(new SetBooleanCommand(name, ConditionParser.parseCondition(value)));
                                }
                                else if (type == "number")
                                {
                                    block.addCommand(new SetNumberCommand(name, ConditionParser.parseNumerValue(value)));
                                }
                                else if (type == "string")
                                {
                                    block.addCommand(new SetStringCommand(name, value));
                                }
                                else
                                {
                                    throw new GameScriptParseError("set expects first parameter to be bool, number, or string", currentLine);
                                }
                            }
                        }
                    }
                    else
                    {
                        block.addCommand(new TextCommand(next.getContent()));
                    }
                }
            }
            else
            {
                moveNext();
            }

            ++i;
        }

        stackPositions.Pop();
    }

    private GameScript parse()
    {
        var block = new ScriptBlock();
        GameScript result = new GameScript(block);

        parseBlock(block);

        return result;

    }

    public static GameScript parse(string source)
    {
        return new GameScriptParser(source).parse();
    }
}