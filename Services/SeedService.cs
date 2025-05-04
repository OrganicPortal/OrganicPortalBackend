using Microsoft.EntityFrameworkCore;
using OrganicPortalBackend.Controllers;
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

        public Task<ResponseFormatter> AddCERTAsync(long seedId, long companyId, CERTIncomingObj incomingObj);
        public Task<ResponseFormatter> RemoveCERTAsync(long UseCERTId, long companyId);
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
            SeedModel seed = new SeedModel
            {
                Name = incomingObj.Name,
                ScientificName = incomingObj.ScientificName,
                Variety = incomingObj.Variety,
                SeedType = incomingObj.SeedType,
                BatchNumber = incomingObj.BatchNumber,
                HarvestDate = incomingObj.HarvestDate,
                ExpiryDate = incomingObj.ExpiryDate,
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
            var seed = await _dbContext.SeedTable.FirstOrDefaultAsync(item => item.Id == seedId && item.CompanyId == companyId);
            if (seed == null)
                return new ResponseFormatter(message: "Такого насіння не існує.");

            if (seed.Status != EnumSeedStatus.New || seed.Status != EnumSeedStatus.Signed)
                return new ResponseFormatter(message: "Неможливо редагувати інформацію про насіння, що перебуває на етапі сертифікації.");

            seed.Name = incomingObj.Name;
            seed.ScientificName = incomingObj.ScientificName;
            seed.Variety = incomingObj.Variety;
            seed.SeedType = incomingObj.SeedType;
            seed.BatchNumber = incomingObj.BatchNumber;
            seed.HarvestDate = incomingObj.HarvestDate;
            seed.ExpiryDate = incomingObj.ExpiryDate;
            seed.TreatmentType = incomingObj.TreatmentType;
            seed.StorageConditions = incomingObj.StorageConditions;
            seed.AverageWeightThousandSeeds = incomingObj.AverageWeightThousandSeeds;

            _dbContext.SeedTable.Update(seed);
            await _dbContext.SaveChangesAsync();

            return new ResponseFormatter(type: System.Net.HttpStatusCode.OK);
        }
        public async Task<ResponseFormatter> RemoveSeedAsync(long seedId, long companyId)
        {
            var seed = await _dbContext.SeedTable.FirstOrDefaultAsync(item => item.Id == seedId && item.CompanyId == companyId);
            if (seed == null)
                return new ResponseFormatter(message: "Такого насіння не існує.");

            if (seed.Status != EnumSeedStatus.New || seed.Status != EnumSeedStatus.Signed)
                return new ResponseFormatter(message: "Неможливо видалити запис. Доступними для видалення є нові записи й підписані сервісом Solana.");

            _dbContext.SeedTable.Remove(seed);
            await _dbContext.SaveChangesAsync();

            return new ResponseFormatter(type: System.Net.HttpStatusCode.OK);
        }
        public async Task<ResponseFormatter> SendSeedToCertificationAsync(long seedId, long companyId)
        {
            var seed = await _dbContext.SeedTable.FirstOrDefaultAsync(item => item.Id == seedId && item.CompanyId == companyId);
            if (seed == null)
                return new ResponseFormatter(message: "Такого насіння не існує.");

            if (seed.Status != EnumSeedStatus.New || seed.Status != EnumSeedStatus.Signed)
                return new ResponseFormatter(message: "Це насіння вже перебуває на етапі сертифікації.");

            seed.Status = EnumSeedStatus.AwaitCertificateConfirmation;

            _dbContext.SeedTable.Update(seed);
            await _dbContext.SaveChangesAsync();

            return new ResponseFormatter(type: System.Net.HttpStatusCode.OK);
        }


        public async Task<ResponseFormatter> AddCERTAsync(long seedId, long companyId, CERTIncomingObj incomingObj)
        {
            var seed = await _dbContext.SeedTable.FirstOrDefaultAsync(item => item.Id == seedId && item.CompanyId == companyId);
            if (seed == null)
                return new ResponseFormatter(message: "Такого насіння не існує.");

            if (seed.Status != EnumSeedStatus.New || seed.Status != EnumSeedStatus.Signed)
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
        /* */


        // Адміністративні методи
        /* */
        /* */
    }
}
