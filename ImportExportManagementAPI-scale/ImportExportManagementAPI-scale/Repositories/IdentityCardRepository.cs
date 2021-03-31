using ImportExportManagement_API.Enum;
using ImportExportManagement_API.Models;
using ImportExportManagementAPI_scale.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI_scale.Repositories
{
    public class IdentityCardRepository : BaseRepository<IdentityCard>
    {
        //check scand card
        public async Task<IdentityCard> checkCard(String cardId)
        {
            if (cardId != null)
            {
                //card có nằm trong hệ thống không
                var identityCard = await GetByIDAsync(cardId);
                if (identityCard != null && identityCard.IdentityCardStatus.Equals((int)IdentityCardStatus.Active))
                {
                    return identityCard;
                }
            }
            return null;
        }
        //lay provider cua card
        public async Task<Partner> GetPartnerCard(int partnerId)
        {
            PartnerRepository partnerRepo = new PartnerRepository();
            var partner = await partnerRepo.GetByIDAsync(partnerId);
            if (partner == null || partner.PartnerStatus.Equals(PartnerStatus.Block))
            {
                return null;
            }
            return partner;
        }

    }
}
