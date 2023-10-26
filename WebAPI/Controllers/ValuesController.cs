using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

public class ValuesController : BaseAPIController
{
    // GET api/values
    [HttpGet]
    public async Task<ActionResult<IEnumerable<string>>> Get() {
        await Task.Delay(1000);

        return new string[] { "value1", "value2" };
    }
}