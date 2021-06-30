using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace LittleSerializer
{
    class Program
    {
        static void Main(string[] args)
        {
            MyStruct myStruct = new MyStruct();
            myStruct.PopulateData();

            MyStructSerializable msSerial = new MyStructSerializable();
            msSerial.PopulateData();

            Stopwatch sw = new Stopwatch();

            double serializeTime, deserializeTime;

            sw.Start();
            byte[] data = Serializer.Serialize(myStruct);
            sw.Stop();

            serializeTime = sw.Elapsed.TotalMilliseconds;

            sw.Restart();
            MyStruct aStruct = Serializer.Deserialize<MyStruct>(data);
            sw.Stop();

            deserializeTime = sw.Elapsed.TotalMilliseconds;

            Console.WriteLine("Data Size: " + data.Length + ", Serialize Time: " + serializeTime + "ms, De-Serialize Time: " + deserializeTime + "ms");

            sw.Restart();
            data = ManagedSerialize(msSerial);
            sw.Stop();

            serializeTime = sw.Elapsed.TotalMilliseconds;

            sw.Restart();
            MyStructSerializable ms = (MyStructSerializable)ManagedDeSerialize(data);
            sw.Stop();

            deserializeTime = sw.Elapsed.TotalMilliseconds;

            Console.WriteLine("Data Size: " + data.Length + ", Serialize Time: " + serializeTime + "ms, De-Serialize Time: " + deserializeTime + "ms");


            Console.ReadLine();
        }

        static byte[] ManagedSerialize(object input)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, input);
                return ms.ToArray();
            }
        }

        static object ManagedDeSerialize(byte[] data)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(data, 0, data.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct MyStruct
    {
        public float chunkX, chunkY, chunkZ; //12 bytes

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] //WARNING: Packing (base 2)
        public byte[] byteBuffer; //10 Items * 1 byte

        public int integerType; //4 bytes

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] //base 2 for packing
        public string nameString; //32 chars -> 1byte/char = 16 bytes
        public Vector3 posiiton; //12 bytes

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Vector3[] arrayStruct; //2 Items * 12 bytes

        public void PopulateData()
        {
            chunkX = 10; chunkY = 20; chunkZ = 30;

            byteBuffer = new byte[]{1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16};
            integerType = 12345;
            nameString = "Structure String";
            posiiton = new Vector3(123, 234, 345);
            arrayStruct = new Vector3[] { new Vector3(10,10,10), new Vector3(20, 30, 40)};
        }
    }


    [Serializable]
    struct MyStructSerializable
    {
        public float chunkX, chunkY, chunkZ; //12 bytes
        public byte[] byteBuffer; //10 Items * 1 byte

        public int integerType; //4 bytes
        public string nameString; //32 chars -> 1byte/char = 16 bytes
        public Vector3 posiiton; //12 bytes
        public Vector3[] arrayStruct; //2 Items * 12 bytes

        public void PopulateData()
        {
            chunkX = 10; chunkY = 20; chunkZ = 30;

            byteBuffer = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            integerType = 12345;
            nameString = "Structure String";
            posiiton = new Vector3(123, 234, 345);
            arrayStruct = new Vector3[] { new Vector3(10, 10, 10), new Vector3(20, 30, 40) };
        }
    }

    [Serializable]
    struct Vector3
    {
        public float x, y, z;

        public Vector3(float X, float Y, float Z)
        {
            x = X;
            y = Y;
            z = Z;
        }
    }
}
