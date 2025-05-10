using Microsoft.EntityFrameworkCore;
using OrganicPortalBackend.Controllers;
using OrganicPortalBackend.Models;
using OrganicPortalBackend.Models.Database;
using OrganicPortalBackend.Models.Database.Seed;
using OrganicPortalBackend.Services.Response;

namespace OrganicPortalBackend.Services
{
    public interface ISeed
    {
        // Користувацькі методи
        /* */
        public Task<ResponseFormatter> NewSeedAsync(long companyId, SeedIncomingObj incomingObj);
        public Task<ResponseFormatter> EditSeedAsync(long seedId, long companyId, SeedIncomingObj incomingObj);
        public Task<ResponseFormatter> RemoveSeedAsync(long seedId, long companyId);
        public Task<ResponseFormatter> SendSeedToCertificationAsync(long seedId, long companyId);
        public Task<ResponseFormatter> SeedInfoAsync(long seedId, long companyId);
        public Task<ResponseFormatter> SeedList(long companyId, Paginator paginator);

        public Task<ResponseFormatter> AddCERTAsync(long seedId, long companyId, CERTIncomingObj incomingObj);
        public Task<ResponseFormatter> RemoveCERTAsync(long UseCERTId, long companyId);

        public Task<ResponseFormatter> CERTList();
        /* */


        // Адміністративні методи
        /* */
        /* */
    }
    public class SeedService : ISeed
    {
        public readonly OrganicContext _dbContext;
        public SeedService(OrganicContext dbContext)
        {
            _dbContext = dbContext;
        }


        // Користувацькі методи
        /* */
        public async Task<ResponseFormatter> NewSeedAsync(long companyId, SeedIncomingObj incomingObj)
        {
            if (await CompanyIsArchivated(companyId))
                return new ResponseFormatter(message: "Ця компанія заархівована. Додавання заблоковане.");

            SeedModel seed = new SeedModel
            {
                Name = incomingObj.Name,
                ScientificName = incomingObj.ScientificName,
                Variety = incomingObj.Variety,
                SeedType = incomingObj.SeedType,
                BatchNumber = incomingObj.BatchNumber,
                HarvestDate = incomingObj.HarvestDate.ToUniversalTime(),
                ExpiryDate = incomingObj.ExpiryDate.ToUniversalTime(),
                TreatmentType = incomingObj.TreatmentType,
                StorageConditions = incomingObj.StorageConditions,
                AverageWeightThousandSeeds = incomingObj.AverageWeightThousandSeeds,

                CompanyId = companyId,
            };

            _dbContext.SeedTable.Add(seed);
            await _dbContext.SaveChangesAsync();

            return new ResponseFormatter(
                type: System.Net.HttpStatusCode.OK,
                data: new
                {
                    SeedId = seed.Id
                });
        }
        public async Task<ResponseFormatter> EditSeedAsync(long seedId, long companyId, SeedIncomingObj incomingObj)
        {
            if (await CompanyIsArchivated(companyId))
                return new ResponseFormatter(message: "Ця компанія заархівована. Редагування заблоковане.");

            var seed = await _dbContext.SeedTable.FirstOrDefaultAsync(item => item.Id == seedId && item.CompanyId == companyId);
            if (seed == null)
                return new ResponseFormatter(message: "Такого насіння не існує.");

            if (seed.Status != EnumSeedStatus.New && seed.Status != EnumSeedStatus.Signed)
                return new ResponseFormatter(message: "Неможливо редагувати інформацію про насіння, що перебуває на етапі сертифікації.");

            seed.Name = incomingObj.Name;
            seed.ScientificName = incomingObj.ScientificName;
            seed.Variety = incomingObj.Variety;
            seed.SeedType = incomingObj.SeedType;
            seed.BatchNumber = incomingObj.BatchNumber;
            seed.HarvestDate = incomingObj.HarvestDate.ToUniversalTime();
            seed.ExpiryDate = incomingObj.ExpiryDate.ToUniversalTime();
            seed.TreatmentType = incomingObj.TreatmentType;
            seed.StorageConditions = incomingObj.StorageConditions;
            seed.AverageWeightThousandSeeds = incomingObj.AverageWeightThousandSeeds;

            _dbContext.SeedTable.Update(seed);
            await _dbContext.SaveChangesAsync();

            return new ResponseFormatter(type: System.Net.HttpStatusCode.OK);
        }
        public async Task<ResponseFormatter> SeedInfoAsync(long seedId, long companyId)
        {
            var item = await _dbContext.SeedTable
                .Include(item => item.CERTsList)
                .ThenInclude(item => item.CERT)

                .Where(item => item.CompanyId == companyId)
                .Where(item => item.Id == seedId)
                .Select(item => new
                {
                    item.Name,
                    item.ScientificName,
                    item.Variety,
                    item.SeedType,
                    item.BatchNumber,
                    item.HarvestDate,
                    item.ExpiryDate,
                    item.TreatmentType,
                    item.StorageConditions,
                    item.AverageWeightThousandSeeds,
                    item.Status,
                    item.CompanyId,

                    CERTsList = item.CERTsList.Select(el => new
                    {
                        el.Id,
                        el.IsVerified,
                        el.CERTId,
                        CERT = new
                        {
                            el.CERT!.Name,
                            el.CERT!.Number,
                            el.CERT!.IssuedBy,
                            el.CERT!.Description,
                            el.CERT!.IsAddlInfo,
                        },
                        el.CERTAdditionalId,
                    })
                    .ToList()
                })
                .FirstOrDefaultAsync();

            return new ResponseFormatter(
                type: System.Net.HttpStatusCode.OK,
                data: item);
        }
        public async Task<ResponseFormatter> RemoveSeedAsync(long seedId, long companyId)
        {
            if (await CompanyIsArchivated(companyId))
                return new ResponseFormatter(message: "Ця компанія заархівована. Видалення заблоковане.");

            var seed = await _dbContext.SeedTable.FirstOrDefaultAsync(item => item.Id == seedId && item.CompanyId == companyId);
            if (seed == null)
                return new ResponseFormatter(message: "Такого насіння не існує.");

            if (seed.Status != EnumSeedStatus.New && seed.Status != EnumSeedStatus.Signed)
                return new ResponseFormatter(message: "Неможливо видалити запис. Доступними для видалення є нові записи й підписані сервісом Solana.");

            _dbContext.SeedTable.Remove(seed);
            await _dbContext.SaveChangesAsync();

            return new ResponseFormatter(type: System.Net.HttpStatusCode.OK);
        }
        public async Task<ResponseFormatter> SendSeedToCertificationAsync(long seedId, long companyId)
        {
            if (await CompanyIsArchivated(companyId))
                return new ResponseFormatter(message: "Ця компанія заархівована. Насіння неможливо надіслати на сертифікацію.");

            var seed = await _dbContext.SeedTable
                .Include(item => item.CERTsList)
                .FirstOrDefaultAsync(item => item.Id == seedId && item.CompanyId == companyId);

            if (seed == null)
                return new ResponseFormatter(message: "Такого насіння не існує.");

            if (seed.Status != EnumSeedStatus.New && seed.Status != EnumSeedStatus.Signed)
                return new ResponseFormatter(message: "Це насіння вже перебуває на етапі сертифікації.");


            foreach (var cert in seed.CERTsList)
            {
                if (cert.CERTId == 1)
                {
                    cert.IsVerified = true;
                    _dbContext.UseCERTTable.Update(cert);
                }
            }

            seed.Status = seed.CERTsList.Any(item => item.IsVerified == false) ? EnumSeedStatus.AwaitCertificateConfirmation : EnumSeedStatus.AwaiSigning;

            _dbContext.SeedTable.Update(seed);
            await _dbContext.SaveChangesAsync();

            return new ResponseFormatter(type: System.Net.HttpStatusCode.OK, "Продукт відправлено на сертифікацію");
        }
        public async Task<ResponseFormatter> SeedList(long companyId, Paginator paginator)
        {
            var query = _dbContext.SeedTable
                .Where(item => item.CompanyId == companyId);

            var count = await query.CountAsync();
            var items = await query
                .OrderByDescending(item => item.CreatedDate)
                .Skip(paginator.Skip)
                .Take(paginator.PageSize)
                .ToListAsync();

            return new ResponseFormatter(
                type: System.Net.HttpStatusCode.OK,
                data: new
                {
                    Count = count,
                    Items = items
                });
        }

        public async Task<ResponseFormatter> AddCERTAsync(long seedId, long companyId, CERTIncomingObj incomingObj)
        {
            if (await CompanyIsArchivated(companyId))
                return new ResponseFormatter(message: "Ця компанія заархівована. Неможливо додати сертифікат.");

            var seed = await _dbContext.SeedTable.FirstOrDefaultAsync(item => item.Id == seedId && item.CompanyId == companyId);
            if (seed == null)
                return new ResponseFormatter(message: "Такого насіння не існує.");

            if (seed.Status != EnumSeedStatus.New && seed.Status != EnumSeedStatus.Signed)
                return new ResponseFormatter(message: "Неможливо додати сертифікат. Насіння перебуває на етапі сертифікації.");

            if (await _dbContext.UseCERTTable.AnyAsync(item => item.SeedId == seedId && item.CERTId == incomingObj.CERTId))
                return new ResponseFormatter(message: "такий сертифікат вже був доданий.");

            if (!(await _dbContext.CERTTable.AnyAsync(item => item.Id == incomingObj.CERTId)))
                return new ResponseFormatter(message: "Вказаного сертифікату не існує.");

            UseCERTModel cert = new UseCERTModel
            {
                CERTId = incomingObj.CERTId,
                SeedId = seed.Id,
            };

            _dbContext.UseCERTTable.Add(cert);
            await _dbContext.SaveChangesAsync();

            return new ResponseFormatter(
                type: System.Net.HttpStatusCode.OK,
                data: new
                {
                    UseCERTId = cert.Id
                });
        }
        public async Task<ResponseFormatter> RemoveCERTAsync(long UseCERTId, long companyId)
        {
            if (await CompanyIsArchivated(companyId))
                return new ResponseFormatter(message: "Ця компанія заархівована. Неможливо видалити сертифікат.");

            var cert = await _dbContext.UseCERTTable
                .Include(item => item.Seed)

                .Where(item => item.Id == UseCERTId)
                .Where(item => item.Seed!.CompanyId == companyId)
                .Where(item => item.Seed!.Status == EnumSeedStatus.New || item.Seed!.Status == EnumSeedStatus.Signed)
                .FirstOrDefaultAsync();

            if (cert == null)
                return new ResponseFormatter(message: "Неможливо видалити сертифікат.");

            _dbContext.UseCERTTable.Remove(cert);
            await _dbContext.SaveChangesAsync();

            return new ResponseFormatter(type: System.Net.HttpStatusCode.OK);
        }
        public async Task<ResponseFormatter> CERTList()
        {
            return new ResponseFormatter(
                type: System.Net.HttpStatusCode.OK,
                data: await _dbContext.CERTTable
                .Select(item => new
                {
                    item.Id,
                    item.Name,
                    item.Number,
                    item.IssuedBy,
                    item.Description,
                    item.IsAddlInfo,
                })
                .ToListAsync()
                );
        }
        /* */


        // Адміністративні методи
        /* */
        /* */

        private async Task<bool> CompanyIsArchivated(long companyId)
        {
            return await _dbContext.CompanyTable
                .Where(item => item.Id == companyId)
                .Select(item => item.isArchivated)
                .FirstOrDefaultAsync();
        }
    }
}
