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

public class GameScriptParser {
    private string[] lines;
    private Stack<StackPosition> stackPositions = new Stack<StackPosition>();
    private int currentLine = 0;

    public GameScriptParser(string source) {
        lines = source.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
    }

    public string getNext() {
        if (currentLine < lines.Length) {
            string result = lines[currentLine];
            ++currentLine;
            return result;
        } else {
            return "";
        }
    }

    public bool hasNext() {
        return currentLine < lines.Length;
    }

    public string getLeadingWhitespace(string input) {
        return "";
    }

    public void parseBlock(ScriptBlock block) {
        stackPositions.Push(new StackPosition(null, block));

        while (this.hasNext()) {
            var next = getNext();


        }

        stackPositions.Pop();
    }

    public GameScript parse() {
        var block = new ScriptBlock();
        GameScript result = new GameScript(block);

        return result;

    }

    public static GameScript parse(string source) {
        return new GameScriptParser(source).parse();
    }
}