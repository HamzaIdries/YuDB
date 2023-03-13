using YuDB;

internal class Program
{
    private static void Main(string[] args)
    {
        var controller = new BenchmarkController(true);
        controller.Start();
    }
}