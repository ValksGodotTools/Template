using Godot;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System;

namespace Template;

public partial class UIConsole : PanelContainer
{
    public event Action<bool> OnToggleVisibility;

    TextEdit _feed;
    LineEdit _input;
    Button _settingsBtn;
    PopupPanel _settingsPopup;
    CheckBox _settingsAutoScroll;
    readonly ConsoleHistory _history = new();
    bool _autoScroll = true;

    public List<ConsoleCommandInfo> Commands { get; } = [];

    public override void _Ready()
    {
        Global.Services.Add(this, persistent: true);
        LoadCommands();

        _feed          = GetNode<TextEdit>("%Output");
        _input         = GetNode<LineEdit>("%Input");
        _settingsBtn   = GetNode<Button>  ("%Settings");
        _settingsPopup = GetNode<PopupPanel>("PopupPanel");
        
        Node settingsVBox = GetNode("%PopupVBox");
        _settingsAutoScroll = GetNode<CheckBox>("%PopupAutoScroll");

        _input.TextSubmitted += OnConsoleInputEntered;
        _settingsBtn.Pressed += OnSettingsBtnPressed;
        _settingsAutoScroll.Toggled += v => _autoScroll = v;
        _settingsAutoScroll.ButtonPressed = _autoScroll;

        Hide();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("toggle_console"))
        {
            ToggleVisibility();
            return;
        }

        InputNavigateHistory();
    }

    public void AddMessage(object message)
    {
        double prevScroll = _feed.ScrollVertical;
        
        // Prevent text feed from becoming too large
        if (_feed.Text.Length > 1000)
            // If there are say 2353 characters then 2353 - 1000 = 1353 characters
            // which is how many characters we need to remove to get back down to
            // 1000 characters
            _feed.Text = _feed.Text.Remove(0, _feed.Text.Length - 1000);
        
        _feed.Text += $"\n{message}";

        // Removing text from the feed will mess up the scroll, this is why the
        // scroll value was stored previous, we set this to that value now to fix
        // this
        _feed.ScrollVertical = prevScroll;

        // Autoscroll if enabled
        ScrollDown();
    }

    public void ToggleVisibility()
    {
        Visible = !Visible;
        OnToggleVisibility?.Invoke(Visible);

        if (Visible)
        {
            _input.GrabFocus();
            CallDeferred(nameof(ScrollDown));
        }
    }

    void ScrollDown()
    {
        if (_autoScroll)
            _feed.ScrollVertical = (int)_feed.GetVScrollBar().MaxValue;
    }

    void OnSettingsBtnPressed()
    {
        if (!_settingsPopup.Visible)
            _settingsPopup.PopupCentered();
    }

    void LoadCommands()
    {
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();

        foreach (Type type in types)
        {
            // BindingFlags.Instance must be added or the methods will not
            // be seen
            MethodInfo[] methods = type.GetMethods(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic);

            foreach (MethodInfo method in methods)
            {
                object[] attributes =
                    method.GetCustomAttributes(
                        attributeType: typeof(ConsoleCommandAttribute),
                        inherit: false);

                foreach (object attribute in attributes)
                {
                    if (attribute is not ConsoleCommandAttribute cmd)
                        continue;

                    TryLoadCommand(cmd, method);
                }
            }
        }
    }

    void TryLoadCommand(ConsoleCommandAttribute cmd, MethodInfo method)
    {
        if (Commands.FirstOrDefault(x => x.Name == cmd.Name) != null)
        {
            throw new Exception($"Duplicate console command: {cmd.Name}");
        }

        Commands.Add(new ConsoleCommandInfo
        {
            Name = cmd.Name.ToLower(),
            Aliases = cmd.Aliases.Select(x => x.ToLower()).ToArray(),
            Method = method
        });
    }

    bool ProcessCommand(string text)
    {
        ConsoleCommandInfo cmd = TryGetCommand(text.Split()[0].ToLower());

        if (cmd == null)
        {
            Game.Log($"The command '{text.Split()[0].ToLower()}' does not exist");
            return false;
        }

        MethodInfo method = cmd.Method;

        object instance = GetMethodInstance(cmd.Method.DeclaringType);

        // Valk (Year 2023): Not really sure what this regex is doing. May rewrite
        // code in a more readable fassion.

        // Valk (Year 2024): What in the world

        // Split by spaces, unless in quotes
        string[] rawCommandSplit = Regex.Matches(text,
            @"[^\s""']+|""([^""]*)""|'([^']*)'").Select(m => m.Value)
            .ToArray();

        object[] parameters = ConvertMethodParams(method, rawCommandSplit);

        method.Invoke(instance, parameters);

        return true;
    }

    ConsoleCommandInfo TryGetCommand(string text)
    {
        ConsoleCommandInfo cmd =
            Commands.Find(cmd =>
            {
                // Does text match the command name?
                bool nameMatch = string.Equals(Name, text, StringComparison.OrdinalIgnoreCase);

                if (nameMatch)
                    return true;

                // Does text match an alias in this command?
                bool aliasMatch = cmd.Aliases
                    .Where(x => x == text)
                    .FirstOrDefault() != null;

                return aliasMatch;
            });

        return cmd;
    }

    void OnConsoleInputEntered(string text)
    {
        // case sensitivity and trailing spaces should not factor in here
        string inputToLowerTrimmed = text.Trim().ToLower();
        string[] inputArr = inputToLowerTrimmed.Split(' ');

        // extract command from input
        string cmd = inputArr[0];

        // do not do anything if cmd is just whitespace
        if (string.IsNullOrWhiteSpace(cmd))
            return;

        // keep track of input history
        _history.Add(inputToLowerTrimmed);

        // process the command
        ProcessCommand(text);

        // clear the input after the command is executed
        _input.Clear();
    }

    void InputNavigateHistory()
    {
        // If console is not visible or there is no history to navigate do nothing
        if (!Visible || _history.NoHistory())
            return;

        if (Input.IsActionJustPressed("ui_up"))
        {
            string historyText = _history.MoveUpOne();

            _input.Text = historyText;

            // if deferred is not used then something else will override these settings
            SetCaretColumn(historyText.Length);
        }

        if (Input.IsActionJustPressed("ui_down"))
        {
            string historyText = _history.MoveDownOne();

            _input.Text = historyText;

            // if deferred is not used then something else will override these settings
            SetCaretColumn(historyText.Length);
        }
    }

    #region Helper Functions
    void SetCaretColumn(int pos)
    {
        _input.CallDeferred(LineEdit.MethodName.GrabFocus);
        _input.CallDeferred(LineEdit.MethodName.Set, LineEdit.PropertyName.CaretColumn, pos);
    }

    object[] ConvertMethodParams(MethodInfo method, string[] rawCmdSplit)
    {
        ParameterInfo[] paramInfos = method.GetParameters();
        object[] parameters = new object[paramInfos.Length];
        for (int i = 0; i < paramInfos.Length; i++)
        {
            parameters[i] = rawCmdSplit.Length > i + 1 && rawCmdSplit[i + 1] != null
                ? ConvertStringToType(
                    input: rawCmdSplit[i + 1],
                    targetType: paramInfos[i].ParameterType)
                : null;
        }

        return parameters;
    }

    object GetMethodInstance(Type type)
    {
        object instance;

        if (type.IsSubclassOf(typeof(GodotObject)))
        {
            // This is a Godot Object, find it or create a new instance
            instance = FindNodeByType(GetTree().Root, type) ??
                Activator.CreateInstance(type);
        }
        else
        {
            // This is a generic class, create a new instance
            instance = Activator.CreateInstance(type);
        }

        return instance;
    }

    object ConvertStringToType(string input, Type targetType)
    {
        if (targetType == typeof(string))
            return input;

        try
        {
            if (targetType == typeof(int))
                return int.Parse(input);
        } catch (FormatException e)
        {
            Game.Log(e.Message);
            return 0;
        }

        try
        {
            if (targetType == typeof(bool))
                return bool.Parse(input);
        }
        catch (FormatException e)
        {
            Game.Log(e.Message);
            return false;
        }

        if (targetType == typeof(float))
        {
            // Valk: Not entirely sure what is happening here other than
            // convert the input to a float.
            float.TryParse(input.Replace(',', '.'),
                style: NumberStyles.Any,
                provider: CultureInfo.InvariantCulture,
                result: out float value);

            return value;
        }

        throw new ArgumentException($"Unsupported type: {targetType}");
    }

    // Valk: I have not tested this code to see if it works with 100%
    // no errors.
    Node FindNodeByType(Node root, Type targetType)
    {
        if (root.GetType() == targetType)
            return root;

        foreach (Node child in root.GetChildren())
        {
            Node foundNode = FindNodeByType(child, targetType);

            if (foundNode != null)
                return foundNode;
        }

        return null;
    }
    #endregion Utils
}

