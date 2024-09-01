namespace Template.Netcode;

using Godot;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;

public class PacketReader : IDisposable
{
    readonly MemoryStream stream;
    readonly BinaryReader reader;
    readonly byte[] readBuffer = new byte[GamePacket.MaxSize];

    public PacketReader(ENet.Packet packet)
    {
        stream = new MemoryStream(readBuffer);
        reader = new BinaryReader(stream);
        packet.CopyTo(readBuffer);
        packet.Dispose();
    }

    public byte ReadByte() => reader.ReadByte();
    public sbyte ReadSByte() => reader.ReadSByte();
    public char ReadChar() => reader.ReadChar();
    public string ReadString() => reader.ReadString();
    public bool ReadBool() => reader.ReadBoolean();
    public short ReadShort() => reader.ReadInt16();
    public ushort ReadUShort() => reader.ReadUInt16();
    public int ReadInt() => reader.ReadInt32();
    public uint ReadUInt() => reader.ReadUInt32();
    public float ReadFloat() => reader.ReadSingle();
    public double ReadDouble() => reader.ReadDouble();
    public long ReadLong() => reader.ReadInt64();
    public ulong ReadULong() => reader.ReadUInt64();
    public byte[] ReadBytes(int count) => reader.ReadBytes(count);
    public byte[] ReadBytes() => ReadBytes(ReadInt());
    public Vector2 ReadVector2() => new(ReadFloat(), ReadFloat());
    public Vector3 ReadVector3() => new(ReadFloat(), ReadFloat(), ReadFloat());

    public T Read<T>()
    {
        Type t = typeof(T);

        if (t.IsPrimitive || t == typeof(string))
        {
            return ReadPrimitive<T>(t);
        }

        if (t == typeof(Vector2))
        {
            return (T)(object)ReadVector2();
        }

        if (t == typeof(Vector3))
        {
            return (T)(object)ReadVector3();
        }

        if (t.IsGenericType)
        {
            return ReadGeneric<T>(t);
        }

        if (t.IsEnum)
        {
            return ReadEnum<T>();
        }

        if (t.IsValueType || t.IsClass)
        {
            return ReadStructOrClass<T>(t);
        }

        throw new NotImplementedException("PacketReader: " + t + " is not a supported type.");
    }

    private T ReadPrimitive<T>(Type t)
    {
        if (t == typeof(byte)) return (T)(object)ReadByte();
        if (t == typeof(sbyte)) return (T)(object)ReadSByte();
        if (t == typeof(char)) return (T)(object)ReadChar();
        if (t == typeof(string)) return (T)(object)ReadString();
        if (t == typeof(bool)) return (T)(object)ReadBool();
        if (t == typeof(short)) return (T)(object)ReadShort();
        if (t == typeof(ushort)) return (T)(object)ReadUShort();
        if (t == typeof(int)) return (T)(object)ReadInt();
        if (t == typeof(uint)) return (T)(object)ReadUInt();
        if (t == typeof(float)) return (T)(object)ReadFloat();
        if (t == typeof(double)) return (T)(object)ReadDouble();
        if (t == typeof(long)) return (T)(object)ReadLong();
        if (t == typeof(ulong)) return (T)(object)ReadULong();

        throw new NotImplementedException("PacketReader: " + t + " is not a supported primitive type.");
    }

    private T ReadEnum<T>()
    {
        // Read byte and convert to enum type T
        return (T)Enum.ToObject(typeof(T), ReadByte());
    }

    private T ReadGeneric<T>(Type t)
    {
        // Get generic type definition
        Type g = t.GetGenericTypeDefinition();

        // Check if type is list
        if (g == typeof(IList<>) || g == typeof(List<>))
        {
            // Get list element type
            Type vt = t.GetGenericArguments()[0];

            // Create list instance
            IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(vt));

            // Read list count
            int count = ReadInt();

            // Populate list
            for (int i = 0; i < count; i++)
                list.Add(Read(vt));

            // Return list as T
            return (T)(object)list;
        }

        // Check if type is dictionary
        if (g == typeof(IDictionary<,>) || g == typeof(Dictionary<,>))
        {
            // Get dictionary key and value types
            Type kt = t.GetGenericArguments()[0];
            Type vt = t.GetGenericArguments()[1];

            // Create dictionary instance
            IDictionary dict = (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(kt, vt));

            // Read dictionary count
            int count = ReadInt();

            // Populate dictionary
            for (int i = 0; i < count; i++)
                dict.Add(Read(kt), Read(vt));

            // Return dictionary as T
            return (T)(object)dict;
        }

        // Throw exception for unsupported generic type
        throw new NotImplementedException("PacketReader: " + t + " is not a supported generic type.");
    }

    private T ReadStructOrClass<T>(Type t)
    {
        // Create instance of T
        T v = Activator.CreateInstance<T>();

        // Get and order public instance fields
        IOrderedEnumerable<FieldInfo> fields = t
            .GetFields(BindingFlags.Public | BindingFlags.Instance)
            .OrderBy(field => field.MetadataToken);

        // Set field values
        foreach (FieldInfo f in fields)
            f.SetValue(v, Read(f.FieldType));

        // Get and order public instance properties with setters
        IOrderedEnumerable<PropertyInfo> properties = t
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite)
            .OrderBy(property => property.MetadataToken);

        // Set property values
        foreach (PropertyInfo p in properties)
            p.SetValue(v, Read(p.PropertyType));

        // Return populated instance
        return v;
    }

    private object Read(Type t)
    {
        // Get the Read method for compile-time type T and make it generic for runtime type t
        MethodInfo readMethod = typeof(PacketReader)
            .GetMethod(nameof(Read), BindingFlags.Instance | BindingFlags.Public)
            .MakeGenericMethod(t);

        // Invoke the generic Read method on this instance at runtime
        return readMethod.Invoke(this, null);
    }

    public void Dispose()
    {
        stream.Dispose();
        reader.Dispose();
    }
}
