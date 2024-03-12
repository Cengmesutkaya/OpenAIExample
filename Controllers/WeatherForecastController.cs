using Microsoft.AspNetCore.Mvc;
using OpenAI_API.Completions;
using OpenAI_API;
using System.Diagnostics;

namespace OpenAIExample.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]/{id?}")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost()]
        public IActionResult save([FromBody] Metadata fitbit)
        {

            string json = System.Text.Json.JsonSerializer.Serialize(fitbit);

            //your OpenAI API key
            string apiKey = "sk-eZ8TH7Iuz6snRfLRmIxTT3Blbkxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
            string answer = string.Empty;
            var openai = new OpenAIAPI(apiKey);
            CompletionRequest completion = new CompletionRequest();
            completion.Prompt = fitbit.fitbit.ToString();
            completion.Model = OpenAI_API.Models.Model.ChatGPTTurbo_16k;
            completion.MaxTokens = 4000;
            var result = openai.Completions.CreateCompletionAsync(completion);
            if (result != null)
            {
                foreach (var item in result.Result.Completions)
                {
                    answer = item.Text;
                }
                return Ok(answer);
            }
            else
            {
                return BadRequest("Not found");
            }
        }

        public class Metadata
        {
            public object fitbit { get; set; }
        }
    }
}
