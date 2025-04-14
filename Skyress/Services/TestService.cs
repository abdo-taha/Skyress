namespace Skyress.API.Services;

public class TestService : ITestService
{
    private static readonly List<string> data = new List<string>{
        "1",
        "2",
        "3",
    };

    public IEnumerable<string> GetData()
    {
        return data;
    }
}