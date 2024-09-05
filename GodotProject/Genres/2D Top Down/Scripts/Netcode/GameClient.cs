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

using Template.Netcode.Client;

public partial class GameClient : ENetClient
{
    protected override void Stopped()
    {
        
    }
}

