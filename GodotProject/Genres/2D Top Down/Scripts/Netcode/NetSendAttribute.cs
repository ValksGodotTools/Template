using CSharpUtils;
using Godot;
using GodotUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Template.Netcode;

[AttributeUsage(AttributeTargets.Property)]
public class NetSendAttribute : Attribute
{
    public int Order { get; }

    public NetSendAttribute(int order)
    {
        Order = order;
    }
}

