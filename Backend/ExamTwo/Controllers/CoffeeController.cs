using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ExamTwo.Interfaces;
namespace ExamTwo.Controllers
{
    public class CoffeeController : Controller
    {
        private readonly ICoffeeService _coffeeService;

        public CoffeeController(ICoffeeService service)
        {
            this._coffeeService = service;
        }


        [HttpGet("CoffeeData")]

        public ActionResult<Dictionary<string, int>> GetCoffees()
        {
            var data = this._coffeeService.GetCoffeeStock();

            if (data == null)
            {
                return BadRequest("No se encontraron cafés para mostrarte");
            }

            return Ok(data);
        }

        [HttpGet("CoffeePrices")]

        public ActionResult<Dictionary<string, int>> GetCoffeePrices()
        {
            var data = this._coffeeService.GetCoffeePrices();

            if (data == null)
            {
                return BadRequest("No se encontraron los precios del café");
            }

            return Ok(data);
        }

        [HttpPut("UpdateCoffeeData")]
        public ActionResult UpdateCoffeStatus([FromQuery] string coffeName, int count)
        {
            var result = this._coffeeService.UpdateCoffePrices(coffeName, count);

            if(result == null)
            {
                return BadRequest("Ocurrió un error actualizando las cantidades de los caafés");
            }

            return Ok(true);
        }
    }
}
