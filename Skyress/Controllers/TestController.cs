using Microsoft.AspNetCore.Mvc;
using Skyress.API.Services;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase{

    private readonly ILogger<TestController> logger;
    private readonly ITestService testService;
    public TestController(ILogger<TestController> logger, ITestService testService) {
        this.logger = logger;
        this.testService = testService;
    }
    [HttpGet]
    public ActionResult<IEnumerable<string>> Get(){
        return  new ActionResult<IEnumerable<string>>(testService.GetData());
    }
}