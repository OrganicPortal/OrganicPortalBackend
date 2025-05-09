using Comfy.Enums;
using CYberCryptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrganicPortalBackend.Models.Database;
using OrganicPortalBackend.Models.Database.Seed;
using OrganicPortalBackend.Models.Database.Solana;
using OrganicPortalBackend.Models.Options;
using OrganicPortalBackend.Services.Response;
using Solnet.Wallet;
using System.Text.Json;

namespace OrganicPortalBackend.Services
{
    public interface ISolanaCERT
    {
        public Task<ResponseFormatter> SendSeedToSolanaAsync(long seedId);
        public Task<ResponseFormatter> SolanaReadsAsync(string pubKey);
        public Task<int> CronSolana();
    }
    public class SolanaCERTService : ISolanaCERT
    {
        public readonly ISolana _solanaService;
        public readonly EncryptOptions _encryptOptions;
        public readonly OrganicContext _dbContext;

        public readonly string _programId;
        public readonly string _walletPatch;

        public readonly string _walletPubKey;
        public readonly string _walletPriKey;
        public SolanaCERTService(ISolana solanaService, IOptions<EncryptOptions> encryptOptions, IConfiguration configuration, OrganicContext dbContext)
        {
            _solanaService = solanaService;
            _encryptOptions = encryptOptions.Value;
            _dbContext = dbContext;

            _programId = configuration.GetSection("SolanaProgram").Get<string>()!;
            _walletPatch = configuration.GetSection("WalletPatch").Get<string>()!;

            _walletPubKey = configuration.GetSection("SolanaPub").Get<string>()!;
            _walletPriKey = configuration.GetSection("SolanaPrivate").Get<string>()!;
        }

        public async Task<ResponseFormatter> SolanaReadsAsync(string pubKey)
        {
            // Отримуємо інформацію з облікового запсиу
            var res = await _solanaService.ReadAccountInformationAsync(new PublicKey(pubKey));

            // Повертаємо помилку якщо Solana повернула помилку
            if (!res.Item1)
                return new ResponseFormatter(message: "Сталася помилка, не вдаєтсья отримати інформацію. Спробуйте пізніше");

            // Десеріалізовуємо дані
            var obj = JsonSerializer.Deserialize<SolanaObj>(res.Item2);

            // Повертаємо відповідь з об'єктом даних
            return new ResponseFormatter(
                type: System.Net.HttpStatusCode.OK,
                data: obj);
        }
        public async Task<ResponseFormatter> SendSeedToSolanaAsync(long seedId)
        {
            // Отримуємо насіння
            var seed = await _dbContext.SeedTable
                .Include(item => item.Company)
                .ThenInclude(item => item.ContactsList)

                .Include(item => item.CERTsList)
                .ThenInclude(item => item.CERT)

                .Where(item => item.Id == seedId)
                .FirstOrDefaultAsync();

            // Повертаємо помилку якщо насіння немає
            if (seed == null)
                return new ResponseFormatter();

            // Отримуємо ключ в історії записів
            CYberFormatter cyberFormatter = new CYberFormatter();
            if (string.IsNullOrWhiteSpace(seed.HistoryKey))
                seed.HistoryKey = cyberFormatter.EncryptMethod(seed.Id.ToString(), _encryptOptions.SolanaKey);

            // Формуємо об'єкт
            SolanaObj solObj = new SolanaObj
            {
                Key = cyberFormatter.EncryptMethod(seed.Id.ToString(), _encryptOptions.SolanaKey),
                HistoryKey = seed.HistoryKey,
                PublisherInfo = new SolanaPublisherInfoObj(),

                SeedInfo = new SolanaSeedInfoObj
                {
                    Name = seed.Name,
                    ScientificName = seed.ScientificName,
                    Variety = seed.Variety,
                    SeedType = seed.SeedType,
                    BatchNumber = seed.BatchNumber,
                    HarvestDate = seed.HarvestDate,
                    ExpiryDate = seed.ExpiryDate,
                    TreatmentType = seed.TreatmentType,
                    TreatmentDescription = seed.TreatmentType.GetDescription(),
                    StorageConditions = seed.StorageConditions,
                    AverageWeightThousandSeeds = seed.AverageWeightThousandSeeds,
                },

                CompanyInfo = new SolanaCompanyInfoObj
                {
                    Name = seed.Company!.Name,
                    Description = seed.Company.Description,
                    Address = seed.Company.Address,
                },

                CERTList = seed.CERTsList.Select(item => new SolanaCERTInfoObj
                {
                    Name = item.CERT!.Name,
                    Number = item.CERT.Number,
                    IssuedBy = item.CERT.IssuedBy,
                    Description = item.CERT.Description,
                    IsAddlInfo = item.CERT.IsAddlInfo,
                    CreatedDate = item.VerifiedDate,
                })
                .ToList()
            };

            // Отримуємо стрінгу
            string solJson = JsonSerializer.Serialize(solObj);

            // Отримуємо акаунт платника
            //Account walletAccount = await _solanaService.GetAccountFromFileAsync(_walletPatch);
            Account walletAccount = new Account(_walletPriKey, _walletPubKey);
            PublicKey programPubKey = new PublicKey(_programId);

            // Отримуємо інформацію про запис в соляні
            var result = await _solanaService.CallProgramAndWriteInformationAsync(walletAccount, "J3Am7TPpRHqGZSHNpzoUgQWiBBmNi1Zi41qMHptMGToJ", solJson);

            // Повертаємо помилку, якщо запис не вдалося
            if (!result.Item1)
                return new ResponseFormatter(message: "Не вдалося сформувати запис. Спробуйте пізніше.");

            // Читаємо інформацію про запис
            var resultInfo = result.Item2;

            // Формуємо бд представлення
            SolanaSeedModel solanadb = new SolanaSeedModel
            {
                HistoryKey = solObj.HistoryKey,
                Key = solObj.Key,

                Name = solObj.SeedInfo.Name,
                Variety = solObj.SeedInfo.Variety,
                SeedType = solObj.SeedInfo.SeedType,
                TreatmentType = solObj.SeedInfo.TreatmentType,

                CompanyName = solObj.CompanyInfo.Name,

                AccountPrivateKey = cyberFormatter.EncryptMethod(resultInfo.Account.PrivateKey.Key, _encryptOptions.SolanaPrivate),
                AccountPublicKey = resultInfo.Account.PublicKey.Key,

                SignatureList = resultInfo.SignatureList
                .Select(item => new SignatureModel
                {
                    Signature = item
                })
                .ToList()
            };

            // Зберігаємо зміни та оновлені статуси
            _dbContext.SolanaSeedTable.Add(solanadb);

            seed.Status = EnumSeedStatus.Signed;
            _dbContext.SeedTable.Update(seed);

            await _dbContext.SaveChangesAsync();

            // Повертиаємо відповідь з посиланням
            return new ResponseFormatter(
                type: System.Net.HttpStatusCode.OK,
                data: new
                {
                    Id = solanadb.Id,
                    Key = solanadb.Key
                });
        }
        public async Task<int> CronSolana()
        {
            var seeds = await _dbContext.SeedTable
                .Where(item => item.Status == EnumSeedStatus.AwaiSigning)
                .Select(item => item.Id)
                .Take(10)
                .ToListAsync();

            if (seeds.Count > 0)
            {
                foreach (long id in seeds)
                {
                    var res = await SendSeedToSolanaAsync(id);

                    if (res.Type != System.Net.HttpStatusCode.OK)
                    {
                        Console.WriteLine("Sert Error: " + res.Message);
                    }
                }
            }

            return seeds.Count();
        }
    }

    public class SolanaObj
    {
        // Ключ елемента в базі даних
        public string Key { get; set; } = string.Empty;

        // Ключ історії записів
        public string HistoryKey { get; set; } = string.Empty;

        // Дата створення запису
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Інформація про насіння
        public SolanaSeedInfoObj SeedInfo { get; set; } = new SolanaSeedInfoObj();

        // Інформація про компанію
        public SolanaCompanyInfoObj CompanyInfo { get; set; } = new SolanaCompanyInfoObj();

        // Інформація про видавця
        public SolanaPublisherInfoObj PublisherInfo { get; set; } = new SolanaPublisherInfoObj();

        // Інформація про сертифікацію насінн
        public ICollection<SolanaCERTInfoObj> CERTList { get; set; } = new List<SolanaCERTInfoObj>();
    }
    public class SolanaPublisherInfoObj
    {
        // Ім'я сервісу
        public string Name { get; set; } = "Organic Portal";

        // Посилання на сайт
        public string Href { get; set; } = "http://organicportal.in.ua/";
    }
    public class SolanaSeedInfoObj
    {
        // Узагальнене ім'я культури
        // #exemple::Кукурудза цукрова
        public string Name { get; set; } = string.Empty;

        // Узагальнене ім'я культури на латині
        // #exemple::Zea mays saccharata Sturt
        public string ScientificName { get; set; } = string.Empty;

        // Сорт рослини
        // #exemple::Медунка F1
        public string Variety { get; set; } = string.Empty;

        // Тип насіння (органічне, гібридне тощо)
        // #exemple::Гібрид F1
        public string SeedType { get; set; } = string.Empty;

        // Номер партії
        // #exemple::4823069912888
        public string BatchNumber { get; set; } = string.Empty;

        // Дата виготовлення (збирання)
        // #exemple::10.03.2025
        public DateTime HarvestDate { get; set; } = DateTime.UtcNow;

        // Термін придатності
        // #exemple::10.10.2029
        public DateTime ExpiryDate { get; set; } = DateTime.UtcNow;

        // Оброблене насіння
        // #exemple::Untreated (Не оброблене)
        public EnumTreatmentType TreatmentType { get; set; } = EnumTreatmentType.Uncnown;

        // Оброблене насіння
        // #exemple::Не оброблене
        public string TreatmentDescription { get; set; } = string.Empty;

        // Умови зберігання насіння
        // #exemple::В сухому місці, при температурах від 6°-40° градусів
        public string StorageConditions { get; set; } = string.Empty;

        // Вага 1 тисячі насіннин в грамах
        // #exemple::~1176,47 г
        public double AverageWeightThousandSeeds { get; set; } = 0;
    }
    public class SolanaCompanyInfoObj
    {
        // Ім'я установи (повна назва)
        // #exemple::ТМ Яскрава
        public string Name { get; set; } = string.Empty;

        // Повний опис компанії (весь опис компанії для відображення в пошуку)
        // #exemple::Виробник якісного насіння, що захоплує всю Україну
        public string Description { get; set; } = string.Empty;

        //// Номер в державному реєстрі
        //// #exemple::2115818089
        //public string RegistrationNumber { get; set; } = string.Empty;

        // Повна адреса до установи
        // #exemple::Україна, **0, Хмельницька обл., місто Хмельницький, ВУЛИЦЯ ПРИБУЗЬКА, будинок **, квартира **
        public string Address { get; set; } = string.Empty;

        //// Контактна інформація пр окомпанію
        //public List<string> ContactInfo { get; set; } = new List<string>();
    }
    public class SolanaCERTInfoObj
    {
        // Назва сертифікату
        // #exemple::Organic, Base EU/UA from 2025 year, issue I
        public string Name { get; set; } = string.Empty;

        // Номер сертифікату
        // #exemple::OB-EU/UA-2025-I (Organic, Base)
        public string Number { get; set; } = string.Empty;

        // Надавач послуг сертифікації
        // #exemple::Портал органічної рослинності (Organic Portal)
        public string IssuedBy { get; set; } = string.Empty;

        // Опис сертифікату
        // #exemple::Базовий сертифікат якості насіння в системі OrganicPortal. Насіння не є перевіреним лабораторією "Organic Portal"
        public string Description { get; set; } = string.Empty;

        // Чи повинен містити додаткову інформацію
        // #exemple::false
        public bool IsAddlInfo { get; set; } = false;

        // Дата верифікації сертифікату
        public DateTime? CreatedDate { get; init; } = null;
    }
}
