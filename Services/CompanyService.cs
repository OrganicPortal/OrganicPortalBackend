using Comfy.Enums;
using Microsoft.EntityFrameworkCore;
using OrganicPortalBackend.Controllers;
using OrganicPortalBackend.Models;
using OrganicPortalBackend.Models.Database;
using OrganicPortalBackend.Models.Database.Company;
using OrganicPortalBackend.Services.Response;
using PhoneNumbers;

namespace OrganicPortalBackend.Services
{
    public interface ICompany
    {
        // Користувацькі методи
        /* */
        public Task<ResponseFormatter> NewCompanyAsync(CompanyIncomingObj incomingObj, string token);
        public Task<ResponseFormatter> EditCompanyAsync(long companyId, EditCompanyIncomingObj incomingObj);

        public Task<ResponseFormatter> MyCompanyAsync(string token);
        public Task<ResponseFormatter> CompanyInfoAsync(long companyId);
        public Task<ResponseFormatter> ArchivateCompanyAsync(long companyId);
        public Task<ResponseFormatter> CheckArchivatedCompanyAsync(long companyId);
        /* */


        // Адміністративні методи
        /* */
        public Task<ResponseFormatter> CompanyListAsync(Paginator paginator);
        public Task<ResponseFormatter> CompanyAsync(long companyId);
        public Task<ResponseFormatter> ChangeTrustCompanyAsync(long companyId, EnumTrustStatus trustStatus);
        /* */
    }
    public class CompanyService : ICompany
    {
        public readonly OrganicContext _dbContext;
        public CompanyService(OrganicContext dbContext)
        {
            _dbContext = dbContext;
        }


        // Користувацькі методи
        /* */
        public async Task<ResponseFormatter> NewCompanyAsync(CompanyIncomingObj incomingObj, string token)
        {
            // Перевірка на збіги
            bool isCompany = await _dbContext.CompanyTable.AnyAsync(item => item.RegistrationNumber == incomingObj.RegistrationNumber);

            if (isCompany)
                //return new ResponseFormatter(message: string.Format("Компанія з кодом \"{0}\" вже існує в системі. Якщо ви її власник, та не реєстрували її в системі, зверність в службу технічної підтримки.", incomingObj.RegistrationNumber));
                return new ResponseFormatter(message: string.Format("Company with code \"{0}\" already exists in the system. If you are its owner and have not registered it in the system, please contact the technical support service.", incomingObj.RegistrationNumber));

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
                var contactValue = CheckContact(el.Contact, el.Type);

                if (contactValue.Item1 == false)
                    return new ResponseFormatter(message: contactValue.Item2);

                company.ContactsList.Add(new CompanyContactModel
                {
                    Type = el.Type,
                    Contact = contactValue.Item2
                });
            }

            // Формуємо список тегів
            if (incomingObj.TypeOfActivityList.Count > 0)
            {
                company.TypeOfActivityList = incomingObj.TypeOfActivityList
                    .Select(item => new CompanyTypeOfActivityModel
                    {
                        Type = item.ToEnum<EnumTypeOfInteractivity>()
                    })
                    .Distinct()
                    .ToList();
            }

            // Додаємо власника
            company.EmployeesList.Add(new EmployeesModel
            {
                Role = EnumUserRole.Owner,
                UserId = TokenService.GetUserIdFromLoginToken(token)
            });

            _dbContext.CompanyTable.Add(company);
            await _dbContext.SaveChangesAsync();


            // Повертаємо адресу компанії в системі
            return new ResponseFormatter(type: System.Net.HttpStatusCode.OK,
                data: new
                {
                    CompanyId = company.Id
                    //}, message: "Компанію успішно створено");
                }, message: "The company was successfully established");
        }
        public async Task<ResponseFormatter> EditCompanyAsync(long companyId, EditCompanyIncomingObj incomingObj)
        {
            CompanyModel? company = await _dbContext.CompanyTable
                .Include(item => item.ContactsList)
                .Include(item => item.TypeOfActivityList)
                .Include(item => item.EmployeesList)
                .ThenInclude(item => item.User)

                .Where(item => item.Id == companyId)
                .FirstOrDefaultAsync();

            if (company == null)
                return new ResponseFormatter();

            if (company.isArchivated)
                //return new ResponseFormatter(message: "Ця компанія заархівована. Редагування заблоковане.");
                return new ResponseFormatter(message: "This company is archived. Editing is locked.");

            company.Name = incomingObj.Name;
            company.Description = incomingObj.Description;
            company.Address = incomingObj.Address;
            company.LegalType = incomingObj.LegalType;
            company.EstablishmentDate = incomingObj.EstablishmentDate.ToUniversalTime();


            // Форматуємо контактні дані
            foreach (var cnt in incomingObj.ContactList)
            {
                var contactValue = CheckContact(cnt.Contact, cnt.Type);

                if (contactValue.Item1 == false)
                    return new ResponseFormatter(message: contactValue.Item2);

                cnt.Contact = contactValue.Item2;
            }

            // Перевіряємо на відповідність контактних даних (тимчасова)
            incomingObj.ContactList = incomingObj.ContactList
                .DistinctBy(item => item.Contact)
                .ToList();

            {
                int i = 0;
                while (company.ContactsList.ElementAtOrDefault(i) != null)
                {
                    var cnt = company.ContactsList.ElementAt(i);

                    var el = incomingObj.ContactList.FirstOrDefault(item => item.Contact == cnt.Contact && item.Type == cnt.Type);
                    if (el != null)
                    {
                        incomingObj.ContactList.Remove(el);
                        i++;
                    }
                    else
                    {
                        _dbContext.CompanyContactTable.Remove(cnt);
                        company.ContactsList.Remove(cnt);
                    }
                }

                if (incomingObj.ContactList.Count() > 0)
                {
                    foreach (var el in incomingObj.ContactList)
                    {
                        company.ContactsList.Add(new CompanyContactModel
                        {
                            Type = el.Type,
                            Contact = el.Contact,

                            CompanyId = companyId
                        });
                    }
                }
            }
            {
                // Перевіряємо на відповідність тегів (тимчасова)
                incomingObj.TypeOfActivityList = incomingObj.TypeOfActivityList
                    .Distinct()
                    .ToList();

                try
                {
                    int i = 0;
                    while (company.TypeOfActivityList.ElementAtOrDefault(i) != null)
                    {
                        var toa = company.TypeOfActivityList.ElementAt(i);

                        var isEl = incomingObj.TypeOfActivityList.Any(item => item.ToEnum<EnumTypeOfInteractivity>() == toa.Type);
                        if (isEl)
                        {
                            incomingObj.TypeOfActivityList.Remove((int)toa.Type);
                            i++;
                        }
                        else
                        {
                            _dbContext.CompanyTypeOfActivityTable.Remove(toa);
                            company.TypeOfActivityList.Remove(toa);
                        }
                    }

                    if (incomingObj.TypeOfActivityList.Count() > 0)
                    {
                        foreach (var el in incomingObj.TypeOfActivityList)
                        {
                            company.TypeOfActivityList.Add(new CompanyTypeOfActivityModel
                            {
                                Type = el.ToEnum<EnumTypeOfInteractivity>(),

                                CompanyId = companyId
                            });
                        }
                    }
                }
                catch
                {
                    //return new ResponseFormatter(message: "Один з видів діяльності вказаний не корректно.");
                    return new ResponseFormatter(message: "One of the activities is specified incorrectly.");
                }
            }


            _dbContext.CompanyTable.Update(company);
            await _dbContext.SaveChangesAsync();


            object? responseCompany = new
            {
                company.Id,
                company.CreatedDate,

                company.Name,
                company.Description,
                company.RegistrationNumber,
                company.Address,
                company.TrustStatus,
                TrustDescription = "",
                company.LegalType,
                company = "",
                company.EstablishmentDate,
                company.isArchivated,
                company.ArchivationDate,

                ContactList = company.ContactsList.Select(el => new
                {
                    el.Id,
                    el.Type,
                    TypeDescription = "",
                    el.Contact
                })
                    .ToList(),

                TypeOfActivityList = company.TypeOfActivityList.Select(el => new
                {
                    el.Id,
                    el.Type,
                    TypeDescription = "",
                })
                    .ToList(),

                EmployeesList = company.EmployeesList.Select(el => new
                {
                    el.Id,
                    el.Role,

                    el.UserId,
                    el.User!.FirstName,
                    el.User.MiddleName,
                    el.User.LastName,
                })
                    .ToList()
            };


            return new ResponseFormatter(
                type: System.Net.HttpStatusCode.OK,
                //data: responseCompany, message: "Успішно відредаговано");
                data: responseCompany, message: "Successfully edited");
        }

        public async Task<ResponseFormatter> MyCompanyAsync(string token)
        {
            var companyList = await _dbContext.CompanyTable
                .Include(item => item.EmployeesList)

                .Where(item => item.EmployeesList.Any(item2 => item2.UserId == TokenService.GetUserIdFromLoginToken(token)))
                .Select(item => new
                {
                    item.Id,
                    item.Name,
                    item.Description,
                    item.TrustStatus,
                    TrustDescription = "",

                    item.CreatedDate,
                    item.isArchivated,
                    item.ArchivationDate,
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
                    TrustDescription = "",
                    item.LegalType,
                    LegalDescription = "",
                    item.EstablishmentDate,
                    item.isArchivated,
                    item.ArchivationDate,

                    ContactList = item.ContactsList.Select(el => new
                    {
                        el.Id,
                        el.Type,
                        TypeDescription = "",
                        el.Contact
                    })
                    .ToList(),

                    TypeOfActivityList = item.TypeOfActivityList.Select(el => new
                    {
                        el.Id,
                        el.Type,
                        TypeDescription = "",
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
                //return new ResponseFormatter(type: System.Net.HttpStatusCode.OK, message: "Компанію було заархівовано. Для розархівування, зверніться до служби підтримки.");
                return new ResponseFormatter(type: System.Net.HttpStatusCode.OK, message: "The company has been archived. To unarchive it, please contact support.");

            company.isArchivated = true;
            company.ArchivationDate = DateTime.UtcNow;

            _dbContext.CompanyTable.Update(company);
            await _dbContext.SaveChangesAsync();

            //return new ResponseFormatter(type: System.Net.HttpStatusCode.OK, message: "Компанію було заархівовано. Для розархівування, зверніться до служби підтримки.");
            return new ResponseFormatter(type: System.Net.HttpStatusCode.OK, message: "The company has been archived. To unarchive it, please contact support.");
        }
        public async Task<ResponseFormatter> CheckArchivatedCompanyAsync(long companyId)
        {
            bool? company = await _dbContext.CompanyTable
                .Where(item => item.Id == companyId)
                .Select(item => item.isArchivated)
                .FirstOrDefaultAsync();

            if (company == null)
                return new ResponseFormatter();

            return new ResponseFormatter(
                type: System.Net.HttpStatusCode.OK,
                data: new
                {
                    isArchivated = company
                });
        }

        public async Task<ResponseFormatter> RemoveContactAsync(long companyId, long contactId)
        {
            var contact = await _dbContext.CompanyContactTable.FirstOrDefaultAsync(item => item.Id == contactId && item.CompanyId == companyId);

            if (contact != null)
            {
                _dbContext.CompanyContactTable.Remove(contact);
                await _dbContext.SaveChangesAsync();

                return new ResponseFormatter(type: System.Net.HttpStatusCode.OK);
            }

            return new ResponseFormatter();
        }
        public async Task<ResponseFormatter> AddContactAsync(long companyId, CompanyContactIncomingObj incomingObj)
        {

            var contactValue = CheckContact(incomingObj.Contact, incomingObj.Type);

            if (contactValue.Item1 == false)
                return new ResponseFormatter(message: contactValue.Item2);

            // Перевіряємо чи такий контакт вже є в компанії
            var dbContact = await _dbContext.CompanyContactTable.FirstOrDefaultAsync(item => item.Contact == contactValue.Item2 && item.CompanyId == companyId);
            if (dbContact != null)
                return new ResponseFormatter(
                    type: System.Net.HttpStatusCode.OK,
                    //message: "Надані контактні дані вже були вказані.");
                    message: "The contact details provided have already been provided.");

            CompanyContactModel contactObj = new CompanyContactModel
            {
                Type = incomingObj.Type,
                Contact = contactValue.Item2,

                CompanyId = companyId
            };

            _dbContext.CompanyContactTable.Add(contactObj);
            await _dbContext.SaveChangesAsync();

            return new ResponseFormatter(
                type: System.Net.HttpStatusCode.OK,
                //message: "Контактні дані додано.",
                message: "Contact details added.",
                data: new
                {
                    contactObj.Id,
                    contactObj.Type,
                    TypeDescription = "",
                    contactObj.Contact
                });
        }

        public async Task<ResponseFormatter> RemoveTypeOfActivityAsync(long companyId, long typeOfActivityId)
        {
            var typeOfActivity = await _dbContext.CompanyTypeOfActivityTable.FirstOrDefaultAsync(item => item.Id == typeOfActivityId && item.CompanyId == companyId);

            if (typeOfActivity != null)
            {
                _dbContext.CompanyTypeOfActivityTable.Remove(typeOfActivity);
                await _dbContext.SaveChangesAsync();

                return new ResponseFormatter(type: System.Net.HttpStatusCode.OK);
            }

            return new ResponseFormatter();
        }
        public async Task<ResponseFormatter> AddTypeOfActivityAsync(long companyId, EnumTypeOfInteractivity typeOfActivity)
        {
            // Перевіряємо чи такий контакт вже є в компанії
            var dbTypeOfActivity = await _dbContext.CompanyTypeOfActivityTable.FirstOrDefaultAsync(item => item.Type == typeOfActivity && item.CompanyId == companyId);
            if (dbTypeOfActivity != null)
                return new ResponseFormatter(
                    type: System.Net.HttpStatusCode.OK,
                    //message: "Вказаний вид діяльності вже був вказаний.");
                    message: "The specified type of activity has already been specified.");

            CompanyTypeOfActivityModel typeOfActivityObj = new CompanyTypeOfActivityModel
            {
                Type = typeOfActivity,
                CompanyId = companyId
            };

            _dbContext.CompanyTypeOfActivityTable.Add(typeOfActivityObj);
            await _dbContext.SaveChangesAsync();

            return new ResponseFormatter(
                type: System.Net.HttpStatusCode.OK,
                //message: "Вид діяльності додано.",
                message: "Activity type added.",
                data: new
                {
                    typeOfActivityObj.Id,
                    typeOfActivityObj.Type,
                    TypeDescription = "",
                });
        }
        /* */


        // Адміністративні методи
        /* */
        public async Task<ResponseFormatter> CompanyListAsync(Paginator paginator)
        {
            var query = _dbContext.CompanyTable;

            long count = await query.CountAsync();
            var items = await query
                .Select(item => new
                {
                    item.Id,
                    item.Name,
                    item.Description,
                    item.TrustStatus,
                    TrustDescription = "",

                    item.CreatedDate,
                    item.isArchivated,
                    item.ArchivationDate,
                })
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
        public async Task<ResponseFormatter> CompanyAsync(long companyId)
        {
            if (companyId > 0)
                return new ResponseFormatter();

            return await CompanyInfoAsync(companyId);
        }
        public async Task<ResponseFormatter> ChangeTrustCompanyAsync(long companyId, EnumTrustStatus trustStatus)
        {
            if (companyId > 0)
                return new ResponseFormatter();

            var company = await _dbContext.CompanyTable.FirstOrDefaultAsync(item => item.Id == companyId);

            if (company != null)
            {
                company.TrustStatus = trustStatus;

                _dbContext.CompanyTable.Update(company);
                await _dbContext.SaveChangesAsync();

                return new ResponseFormatter(type: System.Net.HttpStatusCode.OK);
            }

            return new ResponseFormatter();
        }
        /* */


        // Перевіряє контакт на відповідність типу
        private Tuple<bool, string> CheckContact(string contact, EnumCompanyContactType type)
        {
            string value = "";

            switch (type)
            {
                case EnumCompanyContactType.Phone:
                    try
                    {
                        PhoneNumberUtil phoneUntil = PhoneNumberUtil.GetInstance();
                        PhoneNumber phoneNumber = phoneUntil.Parse(contact, "UA");

                        value = "+" + phoneNumber.CountryCode + phoneNumber.NationalNumber;
                        break;
                    }
                    catch { }
                    //return new Tuple<bool, string>(false, string.Format("Контактні дані \"{0}\" не відповідають типу \"{1}\"", contact, type.GetDescription()));
                    return new Tuple<bool, string>(false, string.Format("Contact data \"{0}\" does not match type \"{1}\"", contact, type.GetDescription()));

                default:
                    //return new Tuple<bool, string>(false, string.Format("Контактні дані \"{0}\" містять не підтримуваний тип.", contact));
                    return new Tuple<bool, string>(false, string.Format("The contact data \"{0}\" contains an unsupported type.", contact));
            }

            return new Tuple<bool, string>(true, value);
        }
    }
}
