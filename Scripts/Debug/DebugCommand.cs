using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DebugCommandBase
{
    private string _commandId;
    private string _commandDescription;
    private string _commandFormat;
    public Type _commandType;

    public string commandId { get { return _commandId; } }
    public string commandDescription { get { return _commandDescription; } }
    public string commandFormat { get { return _commandFormat; } }
    public Type commandType { get { return _commandType; } }

    public DebugCommandBase(string id, string description, string format, Type t)
    {
        this._commandId = id;
        this._commandDescription = description;
        this._commandFormat = format;
        this._commandType = t;
    }
}

public enum cType
{
    Int,
    String,
    Bool,

}

public class DebugCommand : DebugCommandBase
{
    private Action command;

    public DebugCommand(string id, string description, string format, Type t, Action command) : base(id, description, format, t)
    {
        this.command = command;
    }

    public void Invoke()
    {
        command.Invoke();
    }
}

public class DebugCommand<T1> : DebugCommandBase
{
    private Action<T1> command;

    public DebugCommand(string id, string description, string format, Type t, Action<T1> command) : base(id, description, format, t)
    {
        this.command = command;
    }

    public void Invoke(T1 value)
    {
        command.Invoke(value);
    }
}