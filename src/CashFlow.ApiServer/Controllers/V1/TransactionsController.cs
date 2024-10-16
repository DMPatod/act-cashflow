using CashFlow.ApiServer.Contracts.Transactions;
using CashFlow.Application.Transactions;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace CashFlow.ApiServer.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly TransactionService _transactionService;

        public TransactionsController(TransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public ActionResult GetTransactions()
        {
            return Ok();
        }

        [HttpGet("{id}")]
        public ActionResult GetTransaction(string id)
        {
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> AddTransaction([FromBody] AddTransactionRequest request)
        {
            var transaction = await _transactionService.AddAsync(
                request.Amount,
                request.Datetime is not null ? DateTime.ParseExact(request.Datetime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture) : null,
                request.Type);

            return Ok(transaction);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(string id, [FromBody] UpdatedTransactionRequest updatedTransaction)
        {
            await _transactionService.UpdateAsync(
                id,
                updatedTransaction.Amount,
                updatedTransaction.Datetime is not null ? DateTime.ParseExact(updatedTransaction.Datetime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture) : null,
                updatedTransaction.Type);

            return Ok();

        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTransaction(string id)
        {
            return Ok();
        }
    }
}
