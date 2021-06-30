using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace LittleSerializer
{
    public static class Serializer
    {
        /// <summary>
        /// Serializes A Struct
        /// </summary>
        /// <param name="InputObject">The input struct.</param>
        /// <returns>Bytes that represent the serialized data.</returns>
        public static byte[] Serialize(object InputObject)
        {
            GCHandle handle = new GCHandle();
            byte[] outputArray;

            try
            {
                outputArray = new byte[Marshal.SizeOf(InputObject)];
                handle = GCHandle.Alloc(outputArray, GCHandleType.Pinned);
                Marshal.StructureToPtr(InputObject, handle.AddrOfPinnedObject(), false);
            }
            catch (Exception e)
            {
                throw new Exception("Failed To Serialize The Data. Please make sure no managed members are present, arrays have the correct Marshal Attributes and arrays are fully initialized: " + e.Message);
            }
            finally
            {
                handle.Free();
            }

            return outputArray;
        }

        /// <summary>
        /// De-Serializes A Struct
        /// </summary>
        /// <typeparam name="T">The Type Struct being De-Serialized</typeparam>
        /// <param name="InputBytes">Input Serialized Data</param>
        /// <returns>De-Serialized Struct Returned</returns>
        public static T Deserialize<T>(byte[] InputBytes) where T : struct
        {
            if (InputBytes == null)
                throw new System.ArgumentNullException("InputBytes");

            GCHandle handle = new GCHandle();
            T value;

            try
            {
                if (Marshal.SizeOf(default(T)) != InputBytes.Length) throw new InvalidCastException("Cannot Byte Cast To Struct of not same Size!");
                handle = GCHandle.Alloc(InputBytes, GCHandleType.Pinned);
                value = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            catch (InvalidCastException iCast)
            {
                throw iCast;
            }
            catch (Exception e)
            {
                throw new Exception("Failed To De-Serialize The Data: " + e.Message);
            }
            finally
            {
                handle.Free();
            }

            return value;
        }
    }

    public static class SerializerNTCF
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
            if (Marshal.SizeOf(default(T)) != InputBytes.Length) throw new InvalidCastException("Cannot Byte Cast To Struct of not same Size!");

            GCHandle handle = GCHandle.Alloc(InputBytes, GCHandleType.Pinned);
            T value = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));

            handle.Free();
            return value;
        }
    }
}
