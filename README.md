# Serializer
Fast and Small Serializer Wrapper for C# using Marshal

This is just a simple wrapper code that allows conversion from a struct to byte array. It has no overhead, however it does come with array size limitations.

Without any summaries or try/catch/finally blocks, the code simplifies to this:
```c#
public static class Serializer
{
    public static byte[] Serialize(object InputObject)
    {
        byte[] outputArray = new byte[Marshal.SizeOf(InputObject)];
        GCHandle handle = GCHandle.Alloc(outputArray, GCHandleType.Pinned);
        Marshal.StructureToPtr(InputObject, handle.AddrOfPinnedObject(), false);

        handle.Free();
        return outputArray;
    }

    public static T Deserialize<T>(byte[] InputBytes) where T : struct
    {
        if (Marshal.SizeOf(default(T)) != InputBytes.Length) throw new Exception();

        GCHandle handle = GCHandle.Alloc(InputBytes, GCHandleType.Pinned);
        T value = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));

        handle.Free();
        return value;
    }
}
```

### Notes
Make sure you include the proper ```[MarshalAs(..., ...)]``` lines to ensure no exception will be thrown.
