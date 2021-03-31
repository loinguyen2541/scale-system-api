using ImportExportManagement_API.Enum;
using ImportExportManagementAPI_scale.Models;
using ImportExportManagementAPI_scale.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI_scale.Controllers
{
    public class TransactionController : Controller
    {
        private readonly TransactionRepository _repo;
        public TransactionController()
        {
            _repo = new TransactionRepository();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            Transaction trans = await _repo.GetByIDIncludePartnerAsync(id);

            if (trans == null)
            {
                return NotFound();
            }

            return trans;
        }
        //add transaction
        [HttpPost("automatic")]
        public async Task<ActionResult<Transaction>> CreateTransactionByAutomatic(String cardId, float weightIn)
        {
            Transaction trans = new Transaction { CreatedDate = dateTime, IdentityCardId = cardId, WeightIn = weightIn, TimeIn = DateTime.Now, TransactionStatus = (int)TransactionStatus.Progessing };
            Transaction check = await _repo.CreateTransaction(trans, "Insert");
            if (check != null)
            {
                await _repo.SaveAsync();
                return Ok();
            }
            return BadRequest("Card is not exist");
        }

        /*
            Tìm giá trị id của transaction mới nhất của thẻ đó:
            + nếu trans ở trạng thái success => bỏ qua
            + nếu trans ở trạng thái processing => trả về giá trị id trans để update weight lần 2
         */

        //update
        [HttpPut("automatic/{cardId}")]
        public async Task<ActionResult<Transaction>> UpdateTransactionByAutomatic(String cardId, float weightOut)
        {
            Transaction transaction = await _repo.UpdateTransactionArduino(cardId, weightOut, "UpdateArduino");
            if (transaction != null)
            {
                Task<String> updateMiscellaneous = _repo.UpdateMiscellaneousAsync(transaction);

                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
