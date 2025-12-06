using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ExamTwo.Interfaces;
using ExamTwo.Models;

namespace ExamTwo.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ITransactionService service_;

        public TransactionController(ITransactionService service)
        {
            this.service_ = service;
        }

        [HttpPost("Transction")]

        public ActionResult<TransactionResultModel> MakeTransaction([FromBody] OrderRequestModel request)
        {
            var transaction = service_.ProcessOrder(request);

            if (transaction == null)
            {
                return BadRequest("Ocurrió un error realizando la transacción");
            }

            return Ok(transaction);

        }
    }
}
