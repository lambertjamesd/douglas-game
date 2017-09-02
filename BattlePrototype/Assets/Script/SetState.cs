using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBooleanCommand : ScriptCommand
{
    private string name;
    private Condition condition;

    public SetBooleanCommand(string name, Condition condition)
    {
        this.name = name;
        this.condition = condition;
    }

    public override IEnumerator execute(ExecutionContext context)
    {
        context.setBoolean(name, condition.evaluate(context.getGameState()));
        return null;
    }

    public override string ToString(int depth)
    {
        return name + " = " + condition.ToString();
    }
}

public class SetNumberCommand : ScriptCommand
{
    private string name;
    private NumericalValue value;

    public SetNumberCommand(string name, NumericalValue value)
    {
        this.name = name;
        this.value = value;
    }

    public override IEnumerator execute(ExecutionContext context)
    {
        context.setNumber(name, value.evaluate(context.getGameState()));
        return null;
    }

    public override string ToString(int depth)
    {
        return name + " = " + value.ToString();
    }
}

public class SetStringCommand : ScriptCommand
{
    private string name;
    private string value;

    public SetStringCommand(string name, string value)
    {
        this.name = name;
        this.value = value;
    }

    public override IEnumerator execute(ExecutionContext context)
    {
        context.setString(name, value);
        return null;
    }

    public override string ToString(int depth)
    {
        return name + " = " + value;
    }
}