using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ExamTwo.Interfaces;

namespace ExamTwo.Controllers
{
    public class MachineMoneyController : Controller
    {
        private readonly IMachineMoneyService _machineMoneyService;

        public MachineMoneyController(IMachineMoneyService machineMoneyService)
        {
            this._machineMoneyService = machineMoneyService;
        }

        [HttpGet("CoinsChange")]
        public ActionResult<Dictionary<int, int>> GetQuantity()
        {
            var data = this._machineMoneyService.GetCoinInventory();

            if (data == null)
            {
                return BadRequest("No se obtuvieron los precios");
            }

            return data;
        }

        [HttpPut("CoinsUpdate")]
        public ActionResult UpdateCoinsChange([FromQuery] int coin, int count)
        {
            var result = this._machineMoneyService.UpdateCoins(coin, count);

            if(result == false)
            {
                return BadRequest("No se pudo actualizar el estado de las monedas");
            }

            return Ok(true);
        }
    }
}
