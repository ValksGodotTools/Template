namespace Template.Netcode;

using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

public class PacketWriter : IDisposable
{
    public MemoryStream Stream { get; } = new();

    readonly BinaryWriter writer;

    public PacketWriter() => writer = new BinaryWriter(Stream);

    public void Write(byte v) => writer.Write(v);
    public void Write(sbyte v) => writer.Write(v);
    public void Write(char v) => writer.Write(v);
    public void Write(string v) => writer.Write(v);
    public void Write(bool v) => writer.Write(v);
    public void Write(short v) => writer.Write(v);
    public void Write(ushort v) => writer.Write(v);
    public void Write(int v) => writer.Write(v);
    public void Write(uint v) => writer.Write(v);
    public void Write(float v) => writer.Write(v);
    public void Write(double v) => writer.Write(v);
    public void Write(long v) => writer.Write(v);
    public void Write(ulong v) => writer.Write(v);

    public void Write(byte[] v, bool header = true)
    {
        if (header)
            Write(v.Length);

        writer.Write(v);
    }

    public void Write(Vector2 v)
    {
        writer.Write(v.X);
        writer.Write(v.Y);
    }

    public void Write(Vector3 v)
    {
        writer.Write(v.X);
        writer.Write(v.Y);
        writer.Write(v.Z);
    }

    public void Write<T>(T v)
    {
        switch (v)
        {
            case byte k: Write(k); return;
            case sbyte k: Write(k); return;
            case char k: Write(k); return;
            case string k: Write(k); return;
            case bool k: Write(k); return;
            case short k: Write(k); return;
            case ushort k: Write(k); return;
            case int k: Write(k); return;
            case uint k: Write(k); return;
            case float k: Write(k); return;
            case double k: Write(k); return;
            case long k: Write(k); return;
            case ulong k: Write(k); return;
            case byte[] k: Write(k, true); return;
            case Vector2 k: Write(k); return;
        }

        Type t = v.GetType();
        dynamic d = v;

        if (t.IsGenericType)
        {
            Type g = t.GetGenericTypeDefinition();

            if (g == typeof(IList<>) || g == typeof(List<>))
            {
                Write(d.Count);

                foreach (dynamic item in d)
                    Write<dynamic>(item);

                return;
            }

            if (g == typeof(IDictionary<,>) || g == typeof(Dictionary<,>))
            {
                Write(d.Count);

                foreach (dynamic item in d)
                {
                    Write<dynamic>(item.Key);
                    Write<dynamic>(item.Value);
                }

                return;
            }
        }

        if (t.IsEnum)
        {
            Write((byte)d);
            return;
        }

        if (t.IsValueType)
        {
            IOrderedEnumerable<FieldInfo> fields = t
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(field => field.MetadataToken);

            foreach (FieldInfo field in fields)
                Write<dynamic>(field.GetValue(d));

            return;
        }

        throw new NotImplementedException("PacketWriter: " + t + " is not a supported type.");
    }

    public void WriteAll(params dynamic[] values)
    {
        foreach (dynamic value in values)
            Write<dynamic>(value);
    }

    public void Dispose()
    {
        Stream.Dispose();
        writer.Dispose();
    }
}
