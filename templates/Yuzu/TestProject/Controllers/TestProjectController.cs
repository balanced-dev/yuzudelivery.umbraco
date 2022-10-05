using Microsoft.AspNetCore.Mvc;
using Yuzu.TestProject.SetupSteps;
using YuzuDelivery.Umbraco.TestProject.Services;
using Umb = Umbraco.Cms.Core.Services;
using Mod = Umbraco.Cms.Core.Models;

namespace YuzuDelivery.Umbraco.TestProject.Controllers
{
    public class TestProjectController : Controller
    {
        private readonly IEnumerable<IYuzuTestSetupStep> _steps;
        private readonly IYuzuTestProjectState _state;

        public TestProjectController(IEnumerable<IYuzuTestSetupStep> steps, IYuzuTestProjectState state)
        {
            _steps = steps;
            _state = state;
        }

        [HttpGet("ready")]
        public IActionResult Ready()
        {
            if (_state.GetNextStep() > _steps.Max(x => x.Index))
            {
                return Ok("Ready");
            }

            return NotFound();
        }
    }
}
