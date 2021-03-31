using ImportExportManagement_API.Enum;
using ImportExportManagement_API.Models;
using ImportExportManagementAPI_scale.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI_scale.Repositories
{
    public class TransactionRepository : BaseRepository<Transaction>
    {
        public async Task<Transaction> GetByIDIncludePartnerAsync(int id)
        {
            return await _dbSet.Include(t => t.Partner).Where(t => t.TransactionId == id).FirstOrDefaultAsync();
        }

        //kiểm tra xem các trans đang process có thẻ này không
        public async Task<bool> CheckProcessingCard(String cardId, String method)
        {
            bool check = false;
            List<Transaction> listProcessingTrans = new List<Transaction>();
            listProcessingTrans = _dbSet.Where(t => t.TransactionStatus.Equals((int)TransactionStatus.Progessing)).ToList();
            if (listProcessingTrans != null && listProcessingTrans.Count != 0)
            {
                if (method.Equals("Insert"))
                {
                    //insert
                    check = await DisableProcessingTransInInsert(listProcessingTrans, cardId);
                }
                else if (method.Equals("UpdateArduino"))
                {
                    //update by arduino
                    check = await DisableProcessingInUpdateByArduino(listProcessingTrans, cardId);
                }
            }
            return check;
        }
        public async Task<bool> UpdateTransScandCardAsync(String cardId, float weightOut, DateTime timeOut)
        {
            bool check = false;
            //disable những transaction của thẻ này trước đó đang ở trạng thái processing => trừ cái mới nhất để update
            bool checkProcessingCard = await CheckProcessingCard(cardId, "Update");
            if (!checkProcessingCard)
            {
                //tìm transaction gần nhất của thẻ ở trạng thái processing
                var trans = FindTransToWeightOut(cardId);
                if (trans != null)
                {
                    //tìm thấy trans nhưng weightIn = 0 => transaction ko hợp lệ
                    if (trans.WeightIn == 0)
                    {
                        check = false;
                    }
                    else
                    {
                        if (weightOut != 0)
                        {
                            //set type
                            SetTransactionType(trans, weightOut);
                            //set time out
                            trans.TimeOut = timeOut;
                            //set weight out
                            trans.WeightOut = weightOut;
                            //change status
                            trans.TransactionStatus = (int)TransactionStatus.Success;
                            //update transaction
                            try
                            {
                                Update(trans);
                                Task saveDB = SaveAsync();
                                //tạo inventory detail
                                Task updateDetail = UpdateInventoryDetail(trans);
                                check = true;
                            }
                            catch
                            {
                                check = false;
                            }
                        }
                    }
                }
            }
            return check;
        }

        //insert method => disable all transation processing in 
        public async Task<bool> DisableProcessingTransInInsert(List<Transaction> listDisable, String cardId)
        {
            bool check = false;
            for (int i = 0; i < listDisable.Count; i++)
            {
                if (listDisable[i].IdentityCardId != null)
                {
                    //insert => disable all processing transaction
                    if (listDisable[i].IdentityCardId.Equals(cardId))
                    {
                        check = await UpdateStatusProcessingTransactionAsync(listDisable[i]);
                        if (check)
                        {
                            check = false;
                        }
                    }
                }
            }
            return check;
        }
        //update method => disable transations processing in expect last
        public async Task<bool> DisableProcessingTransInUpdateManual(List<Transaction> listDisable, String cardId, int transId)
        {
            bool check = false;
            if (listDisable != null && listDisable.Count != 0)
            {
                for (int i = 0; i < listDisable.Count; i++)
                {
                    if (listDisable[i].IdentityCardId != null)
                    {
                        //update => disable all processing transaction except it
                        if (listDisable[i].IdentityCardId.Equals(cardId) && (listDisable[i].TransactionId != transId))
                        {
                            check = await UpdateStatusProcessingTransactionAsync(listDisable[i]);
                            if (check)
                            {
                                check = false;
                            }
                        }
                    }
                }
            }
            return check;
        }
        //update method => disable transations processing in expect last
        public async Task<bool> DisableProcessingInUpdateByArduino(List<Transaction> listDisable, String cardId)
        {
            bool check = false;
            if (listDisable != null && listDisable.Count != 0)
            {
                for (int i = 0; i < listDisable.Count; i++)
                {
                    if (listDisable[i].IdentityCardId != null)
                    {
                        //update => disable all processing transaction except it
                        if (listDisable[i].IdentityCardId.Equals(cardId) && (i != (listDisable.Count - 1)))
                        {
                            check = await UpdateStatusProcessingTransactionAsync(listDisable[i]);
                            if (check)
                            {
                                check = false;
                            }
                        }
                    }
                }
            }
            return check;
        }
        //disable status processing transaction
        private async Task<bool> UpdateStatusProcessingTransactionAsync(Transaction trans)
        {
            bool update = true;
            if (trans != null)
            {
                trans.TransactionStatus = (int) TransactionStatus.Disable;
                trans.TimeOut = DateTime.Now;
                Update(trans);
                try
                {
                    await SaveAsync();
                }
                catch
                {
                    update = false;
                }
            }
            else
            {
                update = false;
            }
            return update;
        }

        //tìm transaction mới nhất của thẻ ở trạng thái processing
        public Transaction FindTransToWeightOut(String cardId)
        {
            Transaction existed = _dbSet.OrderBy(t => t.TransactionId).Where(t => t.IdentityCardId.Equals(cardId) && t.TransactionStatus.Equals((int)TransactionStatus.Progessing)).LastOrDefault();
            return existed;
        }

        /*
         * update transaction when scand card secondth => truyền vào transaction cần update
         * check coi
         * check weight để insert datetype
         * update date weight in weight out
         */


        //identity transaction type
        public void SetTransactionType(Transaction trans, float weightOut)
        {
            float totalWeight = trans.WeightIn - weightOut;
            if (totalWeight > 0)
            {
                //nhập kho
                trans.TransactionType = (int)TransactionType.Import;
            }
            else
            {
                //xuất kho
                trans.TransactionType = (int)TransactionType.Export;
            }
        }
        public async Task<Transaction> UpdateTransactionByManual(Transaction trans, int id)
        {
            if (id != trans.TransactionId)
            {
                return null;
            }
            Update(trans);
            try
            {
                await SaveAsync();
            }
            catch (Exception)
            {
                return null;
            }
            return trans;
        }
        public async Task<Transaction> UpdateTransactionArduino(String cardId, float weightOut, String method)
        {
            Partner partner;
            if (cardId != null && cardId.Length > 0) //update by arduino
            {
                //find provider and check card status
                IdentityCardRepository cardRepo = new IdentityCardRepository();
                Task<IdentityCard> checkCard = cardRepo.checkCard(cardId);
                if (checkCard.Result == null)
                {
                    //card not available
                    return null;
                }
                partner = cardRepo.GetPartnerCard(checkCard.Result.PartnerId).Result;
            }
            else
            {
                return null;
            }
            //get partner failed
            if (partner == null)
            {
                return null;
            }


            bool checkProcessingCard = await CheckProcessingCard(cardId, method);

            if (checkProcessingCard)
            {
                return null;
            }
            else
            {
                if (weightOut <= 0) return null;
                var trans = FindTransToWeightOut(cardId);
                if (trans.WeightIn <= 0) return null;

                //set time out
                trans.TimeOut = DateTime.Now;
                //set weight out
                trans.WeightOut = weightOut;
                //change status
                trans.TransactionStatus = (int)TransactionStatus.Success;
                //update transaction
                Update(trans);
                try
                {
                    await SaveAsync();
                    //update transaction thành công => tạo inventory detail
                    await UpdateInventoryDetail(trans);
                    return trans;
                }
                catch (Exception)
                {
                    if (GetByID(trans.TransactionId) == null)
                    {
                        return null;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        //tao inventory detail
        private async Task UpdateInventoryDetail(Transaction trans)
        {
            InventoryDetailRepository detailRepo = new InventoryDetailRepository();
            await detailRepo.UpdateInventoryDetail(trans.CreatedDate, trans);
        }

        //tạo transaction
        public async Task<Transaction> CreateTransaction(Transaction trans, String method)
        {
            bool checkSchedule = await CheckTransactionScheduled(trans.IdentityCardId);
            trans.IsScheduled = checkSchedule;
            //check validate weight in weight out
            if (trans.WeightIn <= 0)
            {
                return null;
            }
            if (method.Equals("manual"))
            {
                if (trans.WeightOut <= 0)
                {
                    return null;
                }
            }

            //check card and provider
            Partner partner;
            if (trans.IdentityCardId != null) //insert by arduino
            {
                //find provider and check card status
                IdentityCardRepository cardRepo = new IdentityCardRepository();

                //check valid card
                Task<IdentityCard> checkCard = cardRepo.checkCard(trans.IdentityCardId);
                if (checkCard.Result == null)
                {
                    //card not available
                    return null;
                }
                partner = cardRepo.GetPartnerCard(checkCard.Result.PartnerId).Result;
            }
            else
            {
                partner = (Partner)_dbContext.Partners.Where(p => p.PartnerId == trans.PartnerId && p.PartnerStatus== (int)PartnerStatus.Active);
            }
            //get partner failed
            if (partner == null)
            {
                return null;
            }

            bool checkProceesingCard = await CheckProcessingCard(trans.IdentityCardId, "Insert");
            if (checkProceesingCard) return null;

            //check hợp lệ => tạo transaction
            trans.PartnerId = partner.PartnerId;
            trans.GoodsId = _dbContext.Goods.First().GoodsId;

            if (partner.PartnerTypeId == 1) trans.TransactionType = (int)TransactionType.Export;
            if (partner.PartnerTypeId == 2) trans.TransactionType = (int)TransactionType.Import;

            Insert(trans);
            if (method.Equals("manual"))
            {
                if (trans.TransactionStatus.Equals(TransactionStatus.Success))
                {
                    //tạo transaction thành công => tạo inventory detail
                    await UpdateInventoryDetail(trans);
                }
            }
            return trans;
        }

        //check transaction is scheduled or not
        public async Task<bool> CheckTransactionScheduled(String identityCardId)
        {
            bool check = false;
            IdentityCardRepository cardRepo = new IdentityCardRepository();
            Task<IdentityCard> checkCard = cardRepo.checkCard(identityCardId);
            if (checkCard.Result == null)
            {
                //card not available
                check = false;
            }
            else
            {
                var partner = cardRepo.GetPartnerCard(checkCard.Result.PartnerId).Result;
                if (partner == null)
                {
                    check = false;
                }
                else
                {
                    ScheduleRepository scheduleRepo = new ScheduleRepository();
                    //get list schedule that partner is booked in date
                    List<Schedule> listBookedSchedule = await scheduleRepo.GetBookedScheduleInDate(partner.PartnerId);
                    if (listBookedSchedule != null && listBookedSchedule.Count != 0)
                    {
                        //partner co datlich
                        check = true;
                    }
                    else
                    {
                        check = false;
                    }
                }
            }
            return check;
        }
        public async Task<string> UpdateMiscellaneousAsync(Transaction transaction)
        {
            GoodsRepository goodsRepository = new GoodsRepository();
            goodsRepository.UpdateQuantityOfGood(transaction.GoodsId, transaction.WeightIn - transaction.WeightOut, (TransactionType)transaction.TransactionType);

            String check = "";
            //update schedule
            if ((bool)transaction.IsScheduled)
            {
                ScheduleRepository scheduleRepo = new ScheduleRepository();
                bool checkUpdateSchedule = await scheduleRepo.UpdateActualWeight(transaction.PartnerId, transaction.WeightIn - transaction.WeightOut);
                if (!checkUpdateSchedule)
                {
                    check = "Weight is not valid with register weight";
                }
            }
            else
            {
                //transaction chưa đặt lịch
                TimeTemplateItemRepository timeTemplateItemRepository = new TimeTemplateItemRepository();
                List<TimeTemplateItem> listItem = await timeTemplateItemRepository.GetAppliedItem();
                TimeSpan timeOut = TimeSpan.Parse(transaction.TimeOut.ToString("HH:mm"));
                TimeTemplateItem timeItem = null;
                foreach (var item in listItem)
                {
                    if (timeOut > item.ScheduleTime)
                    {
                        timeItem = item;
                        break;
                    }
                }
                if (timeItem == null)
                    timeItem = listItem[0];
                float totalWeight = transaction.WeightIn - transaction.WeightOut;
                if (totalWeight < 0) totalWeight = totalWeight * -1;
                timeTemplateItemRepository.UpdateCurrent((TransactionType)transaction.TransactionType, totalWeight, timeItem.TimeTemplateItemId);
            }
            return check;
        }

        public async void CancelProcessing()
        {
            List<Transaction> transactions = await _dbSet.Where(t => t.TransactionStatus == (int)TransactionStatus.Progessing).ToListAsync();
            foreach (var item in transactions)
            {
                item.TransactionStatus = (int)TransactionStatus.Disable;
                if (item.TimeOut == null) item.TimeOut = DateTime.Now;
                item.Description = "Disable " + SystemName.System.ToString();
                _dbContext.Entry(item).State = EntityState.Modified;
            }
            await SaveAsync();
        }

        enum SystemName
        {
            System
        }
    }
}
