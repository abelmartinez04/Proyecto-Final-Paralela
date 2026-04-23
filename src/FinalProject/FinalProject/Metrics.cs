using System.Diagnostics;
using System.IO;

public class Metrics
{
    private static bool headerWritten = false;

    public static void Save(string nombreCaso, Stopwatch sw, Stopwatch sw2)
    {
        double seqTime = sw.Elapsed.TotalMilliseconds;
        double parTime = sw2.Elapsed.TotalMilliseconds;

        double speedup = parTime > 0
            ? seqTime / parTime
            : 0;

        string? current = Directory.GetCurrentDirectory();

        // While para subir las metricas y resultados a /metrics
        while (current != null && !Directory.Exists(Path.Combine(current, "src")))
        {
            current = Directory.GetParent(current)?.FullName;
        }
        if (current == null)
        {
            throw new Exception("No se encontró la carpeta src.");
        }
        string metricsDir = Path.Combine(current, "metrics");

        Directory.CreateDirectory(metricsDir);

        string txtPath = Path.Combine(metricsDir, "resultados.txt");
        string csvPath = Path.Combine(metricsDir, "resultados.csv");

        // -------- TXT --------
        string resultTxt = $@"
Caso: {nombreCaso}
Secuencial: {seqTime:F4} ms
Paralelo: {parTime:F4} ms
Speedup: {speedup:F2}x
-----------------------------------";

        File.AppendAllText(txtPath, resultTxt);

        // -------- CSV --------

        if (!headerWritten && !File.Exists(csvPath))
        {
            File.WriteAllText(csvPath, "Caso,Secuencial(ms),Paralelo(ms),Speedup\n");
            headerWritten = true;
        }

        string resultCsv = $"{nombreCaso},{seqTime:F4},{parTime:F4},{speedup:F2}\n";

        File.AppendAllText(csvPath, resultCsv);

        Console.WriteLine(resultTxt);
    }
}