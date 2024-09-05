using CSharpUtils;
using Godot;
using GodotUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Template;

public interface IBaseEntity
{
    public EntityComponent EntityComponent { get; set; }

    public void IdleState(State state);
}

