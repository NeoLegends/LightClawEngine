using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Configuration;
using LightClaw.Engine.Core;
using LightClaw.Engine.Coroutines;
using LightClaw.Engine.Graphics;
using LightClaw.Engine.IO;
using LightClaw.Extensions;
using log4net;
using Munq;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using ProtoBuf;

namespace Experiments
{
    [ProtoContract]
    public class TestCollection : List<int> { }

    class Program
    {
        static void Main(string[] args)
        {
            ObservableCollection<int> test = new ObservableCollection<int>();
            test.Add(10);
            test.Add(12);
            test.Add(14);

            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.Serialize(ms, test);
                Console.WriteLine(ms.ToArray().Length);
            }

            Console.WriteLine("Finished.");
            Console.ReadLine();
        }
    }
}
