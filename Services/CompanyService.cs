using Comfy.Enums;
using Microsoft.EntityFrameworkCore;
using OrganicPortalBackend.Controllers;
using OrganicPortalBackend.Models.Database;
using OrganicPortalBackend.Models.Database.Company;
using OrganicPortalBackend.Services.Response;
using PhoneNumbers;

namespace OrganicPortalBackend.Services
{
    public interface ICompany
    {
        public Task<ResponseFormatter> NewCompanyAsync(CompanyIncomingObj incomingObj, string token);
        public Task<ResponseFormatter> EditCompanyAsync(EditCompanyIncomingObj incomingObj);

        public Task<ResponseFormatter> MyCompanyAsync(string token);
        public Task<ResponseFormatter> CompanyInfoAsync(long companyId);
        public Task<ResponseFormatter> ArchivateCompanyAsync(long companyId);
    }
    public class CompanyService : ICompany
    {
        public readonly OrganicContext _dbContext;
        public readonly TokenService _tokenService;
        public CompanyService(OrganicContext dbContext, TokenService tokenService)
        {
            _dbContext = dbContext;
            _tokenService = tokenService;
        }


        public async Task<ResponseFormatter> NewCompanyAsync(CompanyIncomingObj incomingObj, string token)
        {
            // Перевірка на збіги
            bool isCompany = await _dbContext.CompanyTable.AnyAsync(item => item.RegistrationNumber == incomingObj.RegistrationNumber);

            if (isCompany)
                return new ResponseFormatter(message: string.Format("Компанія з кодом \"{0}\" вже існує в системі. Якщо ви її власник, та не реєстрували її в системі, зверність в службу технічної підтримки.", incomingObj.RegistrationNumber));

            // Формуємо запис
            CompanyModel company = new CompanyModel
            {
                Name = incomingObj.Name,
                Description = incomingObj.Description,
                RegistrationNumber = incomingObj.RegistrationNumber,
                Address = incomingObj.Address,
                LegalType = incomingObj.LegalType,
                EstablishmentDate = incomingObj.EstablishmentDate.ToUniversalTime(),
            };

            // Формуємо контактні дані
            foreach (var el in incomingObj.ContactList.Distinct())
            {
                string contact = "";

                switch (el.Type)
                {
                    case EnumCompanyContactType.Phone:
                        try
                        {
                            PhoneNumberUtil phoneUntil = PhoneNumberUtil.GetInstance();
                            PhoneNumber phoneNumber = phoneUntil.Parse(el.Contact, "UA");

                            contact = "+" + phoneNumber.CountryCode + phoneNumber.NationalNumber;
                            break;
                        }
                        catch { }
                        return new ResponseFormatter(message: string.Format("Контактні дані \"{0}\" не відповідають типу \"{1}\"", el.Contact, el.Type.GetDescription()));

                    default:
                        return new ResponseFormatter(message: string.Format("Контактні дані \"{0}\" містять не підтримуваний тип.", el.Contact));
                }

                company.ContactsList.Add(new CompanyContactModel
                {
                    Type = el.Type,
                    Contact = contact
                });
            }

            // Формуємо список тегів
            if (incomingObj.TypeOfInteractivityList.Count > 0)
            {
                company.TypeOfActivityList = incomingObj.TypeOfInteractivityList
                    .Select(item => new CompanyTypeOfActivityModel
                    {
                        Type = item
                    })
                    .Distinct()
                    .ToList();
            }

            // Додаємо власника
            company.EmployeesList.Add(new EmployeesModel
            {
                Role = EnumUserRole.Owner,
                UserId = _tokenService.GetUserIdFromLoginToken(token)
            });

            _dbContext.CompanyTable.Add(company);
            await _dbContext.SaveChangesAsync();


            // Повертаємо адресу компанії в системі
            return new ResponseFormatter(type: System.Net.HttpStatusCode.OK,
                data: new
                {
                    CompanyId = company.Id
                });
        }
        public async Task<ResponseFormatter> EditCompanyAsync(EditCompanyIncomingObj incomingObj)
        {
            CompanyModel? company = await _dbContext.CompanyTable
                .Where(item => item.Id == incomingObj.CompanyId)
                .FirstOrDefaultAsync();

            if (company == null)
                return new ResponseFormatter();

            company.Name = incomingObj.Name;
            company.Description = incomingObj.Description;
            company.Address = incomingObj.Address;
            company.LegalType = incomingObj.LegalType;
            company.EstablishmentDate = incomingObj.EstablishmentDate.ToUniversalTime();

            _dbContext.CompanyTable.Update(company);
            await _dbContext.SaveChangesAsync();

            return new ResponseFormatter(type: System.Net.HttpStatusCode.OK);
        }

        public async Task<ResponseFormatter> MyCompanyAsync(string token)
        {
            var companyList = await _dbContext.CompanyTable
                .Include(item => item.EmployeesList)

                .Where(item => item.EmployeesList.Any(item2 => item2.UserId == _tokenService.GetUserIdFromLoginToken(token)))
                .Select(item => new
                {
                    item.Id,
                    item.Name,
                    item.Description,

                    item.CreatedDate,
                    item.TrustStatus,
                    item.isArchivated
                })
                .ToListAsync();

            return new ResponseFormatter(
                type: System.Net.HttpStatusCode.OK,
                data: companyList);
        }
        public async Task<ResponseFormatter> CompanyInfoAsync(long companyId)
        {
            object? company = await _dbContext.CompanyTable
                .Include(item => item.ContactsList)
                .Include(item => item.TypeOfActivityList)
                .Include(item => item.EmployeesList)
                .ThenInclude(item => item.User)

                .Where(item => item.Id == companyId)
                .Select(item => new
                {
                    item.Id,
                    item.CreatedDate,

                    item.Name,
                    item.Description,
                    item.RegistrationNumber,
                    item.Address,
                    item.TrustStatus,
                    item.LegalType,
                    item.EstablishmentDate,
                    item.isArchivated,
                    item.ArchivationDate,

                    ContactList = item.ContactsList.Select(el => new
                    {
                        el.Id,
                        el.Type,
                        el.Contact
                    })
                    .ToList(),

                    TypeOfActivityList = item.TypeOfActivityList.Select(el => new
                    {
                        el.Id,
                        el.Type
                    })
                    .ToList(),

                    EmployeesList = item.EmployeesList.Select(el => new
                    {
                        el.Id,
                        el.Role,

                        el.UserId,
                        el.User!.FirstName,
                        el.User.MiddleName,
                        el.User.LastName,
                    })
                    .ToList()
                })
                .FirstOrDefaultAsync();


            return new ResponseFormatter(
                type: System.Net.HttpStatusCode.OK,
                data: company);
        }
        public async Task<ResponseFormatter> ArchivateCompanyAsync(long companyId)
        {
            CompanyModel? company = await _dbContext.CompanyTable
                .Where(item => item.Id == companyId)
                .FirstOrDefaultAsync();

            if (company == null)
                return new ResponseFormatter();

            if (company.isArchivated)
                return new ResponseFormatter(type: System.Net.HttpStatusCode.OK, message: "Компанію було заархівовано. Для розархівування, зверніться до служби підтримки.");

            company.isArchivated = true;
            company.ArchivationDate = DateTime.UtcNow;

            _dbContext.CompanyTable.Update(company);
            await _dbContext.SaveChangesAsync();

            return new ResponseFormatter(type: System.Net.HttpStatusCode.OK, message: "Компанію було заархівовано. Для розархівування, зверніться до служби підтримки.");
        }
    }
}
