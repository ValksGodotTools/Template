using CSharpUtils;
using Godot;
using GodotUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Template.Platformer2D;

public abstract partial class Entity : CharacterBody2D
{
    protected AnimatedSprite2D sprite;

    Label stateLabel;
    State curState;

    public override void _Ready()
    {
        sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        stateLabel = new Label();
        AddChild(stateLabel);

        Init();

        curState = InitialState();
        UpdateStateLabel(curState);

        curState.Enter();
    }

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();

        Update();
        curState.Update((float)delta);
        curState.Transitions();
    }

    protected abstract State InitialState();

    public void SwitchState(State newState)
    {
        curState.Exit();
        newState.Enter();
        curState = newState;

        UpdateStateLabel(newState);
    }

    public virtual void Init() { }
    public virtual void Update() { }

    void UpdateStateLabel(State state)
    {
        stateLabel.Text = state.ToString();
        stateLabel.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.CenterBottom);
        stateLabel.Position -= new Vector2(0, stateLabel.Size.Y / 2);
    }
}

