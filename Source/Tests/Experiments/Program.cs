using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace Experiments
{
    class Program
    {
        static void Main(string[] args)
        {
            MethodInfo[] mInfos = typeof(Vector2).GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

            File.WriteAllText(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Vector2Methods.txt"),
                string.Join(Environment.NewLine, mInfos.Select(mInfo => mInfo.Name).Where(name => !name.StartsWith("op_")).OrderBy(name => name))
            );

            mInfos = typeof(Vector3).GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

            File.WriteAllText(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Vector3Methods.txt"),
                string.Join(Environment.NewLine, mInfos.Select(mInfo => mInfo.Name).Where(name => !name.StartsWith("op_")).OrderBy(name => name))
            );

            mInfos = typeof(Vector4).GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

            File.WriteAllText(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Vector4Methods.txt"),
                string.Join(Environment.NewLine, mInfos.Select(mInfo => mInfo.Name).Where(name => !name.StartsWith("op_")).OrderBy(name => name))
            );
        }
    }
}
