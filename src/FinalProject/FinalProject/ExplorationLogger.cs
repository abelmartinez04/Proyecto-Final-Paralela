using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class ExplorationLogger
{
    private static ConcurrentBag<string> logs = new ConcurrentBag<string>();

    public static void Log(string message)
    {
        logs.Add(message);
    }

    public static void Save(string path)
    {
        File.WriteAllLines(path, logs);
    }

    public static void Clear()
    {
        logs = new ConcurrentBag<string>();
    }
}
