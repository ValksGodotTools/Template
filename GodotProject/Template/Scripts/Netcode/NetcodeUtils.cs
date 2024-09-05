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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class NetcodeUtils
{
    public static Dictionary<Type, PacketInfo<T>> MapPackets<T>()
    {
        List<Type> types = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(x => typeof(T).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .OrderBy(x => x.Name)
            .ToList();

        Dictionary<Type, PacketInfo<T>> dict = new();

        for (byte i = 0; i < types.Count; i++)
            dict.Add(types[i], new PacketInfo<T>
            {
                Opcode = i,
                Instance = (T)Activator.CreateInstance(types[i])
            });

        return dict;
    }
}

public class PacketInfo<T>
{
    public byte Opcode { get; set; }
    public T Instance { get; set; }
}

