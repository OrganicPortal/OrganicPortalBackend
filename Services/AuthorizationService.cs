using Comfy.SMS;
using CYberCryptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrganicPortalBackend.Controllers;
using OrganicPortalBackend.Models.Database;
using OrganicPortalBackend.Models.Database.RegUser;
using OrganicPortalBackend.Models.Database.User;
using OrganicPortalBackend.Models.Database.User.Recovery;
using OrganicPortalBackend.Models.Database.User.Session;
using OrganicPortalBackend.Models.Options;
using OrganicPortalBackend.Services.Response;
using PhoneNumbers;
using System.Net;
using System.Text.Json;

namespace OrganicPortalBackend.Services
{
    public interface IAuthorization
    {
        public Task<ResponseFormatter> SignInAsync(SignInIncomingObj incomingObj);
        public Task<ResponseFormatter> SignOutAsync(string token);
        public Task<ResponseFormatter> SignUpAsync(SignUpIncomingObj incomingObj, string ip);
        public Task<ResponseFormatter> VerifySignUpAsync(string code, string token, string ip);
        public Task<ResponseFormatter> RetryVerifSMSAsync(string token, string ip);

        public Task<ResponseFormatter> InitRecoveryAsync(InitRecoveryIncomingObj incomingObj);
        public Task<ResponseFormatter> RecoveryAsync(RecoveryIncomingObj incomingObj, string token);

        public Task<ResponseFormatter> UserRoles(string token);
    }
    public class AuthorizationService : IAuthorization
    {
        public readonly OrganicContext _dbContext;
        public readonly EncryptOptions _encryptOptions;
        public readonly SMSOptions _smsOptions;
        public AuthorizationService(OrganicContext dbContext, IOptions<EncryptOptions> encryptOptions, IOptions<SMSOptions> smsOptions)
        {
            _dbContext = dbContext;
            _encryptOptions = encryptOptions.Value;
            _smsOptions = smsOptions.Value;
        }


        public async Task<ResponseFormatter> SignInAsync(SignInIncomingObj incomingObj)
        {
            // Initialize the current phone variable
            string phone = string.Empty;

            // Try to disassemble the phone in UA format
            try
            {
                PhoneNumberUtil phoneUntil = PhoneNumberUtil.GetInstance();
                PhoneNumber phoneNumber = phoneUntil.Parse(incomingObj.Login, "UA");

                phone = "+" + phoneNumber.CountryCode + phoneNumber.NationalNumber;
            }
            catch
            {
                // Response error
                return new ResponseFormatter(message: "Вказаний номер не підтримується.");
            }


            // Generate a phone and password scheme
            CYberFormatter cyberFormatter = new CYberFormatter();
            string phoneShema = cyberFormatter.EncryptMethod(phone, _encryptOptions.PhoneKey);
            string passwordShema = cyberFormatter.EncryptMethod(incomingObj.Password, _encryptOptions.PasswordKey);


            // Check if password exists
            string? key = await _dbContext.PhoneTable
                .Where(item => item.IsActive)
                .Where(item => EF.Functions.Collate(item.Phone, "SQL_Latin1_General_CP1_CS_AS") == phoneShema)

                .Select(item => item.Key)
                .FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(key))
            {
                // Check if password exists
                bool isPassword = await _dbContext.PasswordTable
                    .Where(item => EF.Functions.Collate(item.Key, "SQL_Latin1_General_CP1_CS_AS") == key)
                    .Where(item => EF.Functions.Collate(item.Password, "SQL_Latin1_General_CP1_CS_AS") == passwordShema)
                    .Where(item => item.IsActive)
                    .AnyAsync();

                if (isPassword)
                {
                    // Check information about the active user session
                    SessionModel? session = await _dbContext.SessionTable
                        .Where(item => EF.Functions.Collate(item.Key, "SQL_Latin1_General_CP1_CS_AS") == key)
                        .Where(item => item.ExpiredDate >= DateTime.UtcNow)
                        .FirstOrDefaultAsync();

                    if (session != null)
                    {
                        // Update session information
                        session.ExpiredDate = DateTime.UtcNow.AddMinutes(ProgramSettings.TokenExpiredMinuts);
                        session.LastActivityDate = DateTime.UtcNow;

                        _dbContext.SessionTable.Update(session);
                        await _dbContext.SaveChangesAsync();


                        // Response token
                        return new ResponseFormatter(type: HttpStatusCode.OK, data: new
                        {
                            Token = session.Token
                        });
                    }


                    // Create new user session
                    TokenInformation tokenInfo = new TokenInformation
                    {
                        UserId = long.Parse(cyberFormatter.DecryptMethod(key, _encryptOptions.Key)),
                        CreatedDate = DateTime.UtcNow,
                    };

                    session = new SessionModel
                    {
                        Key = key,
                        CreatedDate = tokenInfo.CreatedDate,
                        Token = cyberFormatter.EncryptMethod(JsonSerializer.Serialize(tokenInfo), _encryptOptions.TokenKey)
                    };

                    _dbContext.SessionTable.Add(session);
                    await _dbContext.SaveChangesAsync();


                    // Response token
                    return new ResponseFormatter(type: HttpStatusCode.OK, data: new
                    {
                        Token = session.Token
                    });
                }
            }

            // Response error
            return new ResponseFormatter(message: "Перевірте правильність введених даних.");
        }
        public async Task<ResponseFormatter> SignOutAsync(string token)
        {
            var account = await _dbContext.SessionTable.FirstOrDefaultAsync(item => EF.Functions.Collate(item.Token, "SQL_Latin1_General_CP1_CS_AS") == token);

            if (account != null)
            {
                account.ExpiredDate = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }

            return new ResponseFormatter(type: HttpStatusCode.OK);
        }
        public async Task<ResponseFormatter> SignUpAsync(SignUpIncomingObj incomingObj, string ip)
        {
            // Initialize the current phone variable
            string phone = string.Empty;
            string countryCode = string.Empty;

            // Try to disassemble the phone in UA format
            try
            {
                PhoneNumberUtil phoneUntil = PhoneNumberUtil.GetInstance();
                PhoneNumber phoneNumber = phoneUntil.Parse(incomingObj.Phone, "UA");

                // Update phone information
                phone = "+" + phoneNumber.CountryCode + phoneNumber.NationalNumber;
                countryCode = "+" + phoneNumber.CountryCode;
            }
            catch
            {
                // Response error
                return new ResponseFormatter(message: "Вказаний номер не підтримується.");
            }


            // Generate scheme
            CYberFormatter cyberFormatter = new CYberFormatter();
            string phoneShema = cyberFormatter.EncryptMethod(phone, _encryptOptions.PhoneKey);
            string countryShema = cyberFormatter.EncryptMethod(countryCode, _encryptOptions.PhoneKey);
            string passwordShema = cyberFormatter.EncryptMethod(incomingObj.Password, _encryptOptions.PasswordKey);
            string ipShema = cyberFormatter.EncryptMethod(ip, _encryptOptions.IPKey);

            // Check if phone number existing in DB
            if (await _dbContext.PhoneTable.AnyAsync(item => item.IsActive == true && EF.Functions.Collate(item.Phone, "SQL_Latin1_General_CP1_CS_AS") == phoneShema))
            {
                // Response error
                return new ResponseFormatter(message: "Схоже ви вже маєте обліковий запис. Скористайтеся механізмом відновлення паролю.");
            }

            // Check if the user has previously specified a registration process
            #region Check registration process
            RegModel? dbReg = await _dbContext.RegTable
                .Where(item => EF.Functions.Collate(item.Ip, "SQL_Latin1_General_CP1_CS_AS") == ipShema)
                .FirstOrDefaultAsync();

            if (dbReg != null)
            {
                RegTokenInformation info = JsonSerializer.Deserialize<RegTokenInformation>(cyberFormatter.DecryptMethod(dbReg.Token, _encryptOptions.TokenKey))!;

                // Check if token expired
                if (info.ExpiredDate <= DateTime.UtcNow)
                {
                    _dbContext.RegTable.Remove(dbReg);
                    await _dbContext.SaveChangesAsync();

                    goto Next;
                }

                // Check if user use to many SMS
                if (dbReg.CodeCount >= ProgramSettings.MaxCodeCount)
                {
                    // Response error
                    return new ResponseFormatter(message: "Сталася невідома помилка. Спробуйте трішки пізніше.");
                }

                _dbContext.RegTable.Remove(dbReg);
            }
            Next:
            #endregion


            // Add registration token
            RegTokenInformation regToken = new RegTokenInformation
            {
                FirstName = incomingObj.FirstName,
                MiddleName = incomingObj.MiddleName,
                LastName = incomingObj.LastName,
            };
            RegModel regModel = new RegModel
            {
                Token = cyberFormatter.EncryptMethod(JsonSerializer.Serialize(regToken), _encryptOptions.TokenKey),
                Ip = ipShema,
                Phone = phoneShema,
                CountryCode = countryShema,
                Password = passwordShema,
                Code = GenerateCode(),
                CodeCount = dbReg != null ? dbReg.CodeCount : 1,
            };

            _dbContext.RegTable.Add(regModel);
            await _dbContext.SaveChangesAsync();

            await SendSMSCode("Код підтвердження реєстрації, на сайті organicportal.in.ua: " + regModel.Code, phone);

            // Response token
            return new ResponseFormatter(type: HttpStatusCode.OK,
                message: "На вказаний номер було надіслано СМС повідомлення.",
                data: new
                {
                    Token = regModel.Token
                });
        }
        public async Task<ResponseFormatter> VerifySignUpAsync(string code, string token, string ip)
        {
            // Generate scheme
            CYberFormatter cyberFormatter = new CYberFormatter();
            string ipShema = cyberFormatter.EncryptMethod(ip, _encryptOptions.IPKey);

            RegModel? dbToken = await _dbContext.RegTable
                .Where(item => EF.Functions.Collate(item.Token, "SQL_Latin1_General_CP1_CS_AS") == token)
                .Where(item => EF.Functions.Collate(item.Ip, "SQL_Latin1_General_CP1_CS_AS") == ipShema)
                .Where(item => item.Code == code && item.ExpiredDate >= DateTime.UtcNow)
                .FirstOrDefaultAsync();

            if (dbToken == null)
                return new ResponseFormatter(message: "Код доступу не правильний!");


            RegTokenInformation info = JsonSerializer.Deserialize<RegTokenInformation>(cyberFormatter.DecryptMethod(dbToken.Token, _encryptOptions.TokenKey))!;

            // Registration new user
            UserModel newUser = new UserModel
            {
                FirstName = info.FirstName,
                MiddleName = info.MiddleName,
                LastName = info.LastName
            };

            // Generate user id
            _dbContext.UserTable.Add(newUser);
            await _dbContext.SaveChangesAsync();

            // Generate user key
            string key = cyberFormatter.EncryptMethod(newUser.Id.ToString(), _encryptOptions.Key);

            PhoneModel newPhone = new PhoneModel
            {
                Key = key,
                Phone = dbToken.Phone,
                CountryCode = dbToken.CountryCode,

                IsActive = true
            };

            PasswordModel newPassword = new PasswordModel
            {
                Key = key,
                Password = dbToken.Password,

                IsActive = true
            };

            _dbContext.PhoneTable.Add(newPhone);
            _dbContext.PasswordTable.Add(newPassword);

            _dbContext.RegTable.Remove(dbToken);
            await _dbContext.SaveChangesAsync();

            // Response OK message
            return new ResponseFormatter(type: HttpStatusCode.OK, message: "Код підтверджено. Скористайтеся формою входу для прожовження.");
        }
        public async Task<ResponseFormatter> RetryVerifSMSAsync(string token, string ip)
        {
            // Generate scheme
            CYberFormatter cyberFormatter = new CYberFormatter();
            string ipShema = cyberFormatter.EncryptMethod(ip, _encryptOptions.IPKey);

            RegModel? dbToken = await _dbContext.RegTable
                .Where(item => EF.Functions.Collate(item.Token, "SQL_Latin1_General_CP1_CS_AS") == token)
                .Where(item => EF.Functions.Collate(item.Ip, "SQL_Latin1_General_CP1_CS_AS") == ipShema)
                .FirstOrDefaultAsync();

            if (dbToken == null)
                return new ResponseFormatter(message: "Схоже час підтвердження сплив. Спробуйте пізніше.");


            RegTokenInformation info = JsonSerializer.Deserialize<RegTokenInformation>(cyberFormatter.DecryptMethod(dbToken.Token, _encryptOptions.TokenKey))!;

            if (info.ExpiredDate < DateTime.UtcNow || dbToken.CodeCount >= ProgramSettings.MaxCodeCount)
                return new ResponseFormatter(message: "Схоже час підтвердження сплив. Спробуйте пізніше.");

            if (dbToken.ExpiredDate.AddMinutes(-2) >= DateTime.UtcNow)
                return new ResponseFormatter(type: HttpStatusCode.OK, message: "Код ще дійсний. Зачекайте ще хвилину.");

            string code = GenerateCode();

            dbToken.Code = code;
            dbToken.ExpiredDate = DateTime.UtcNow.AddMinutes(5);
            dbToken.CodeCount++;

            _dbContext.RegTable.Update(dbToken);
            await _dbContext.SaveChangesAsync();

            await SendSMSCode(code, cyberFormatter.DecryptMethod(dbToken.Phone, _encryptOptions.PhoneKey));

            return new ResponseFormatter(type: HttpStatusCode.OK);
        }


        public async Task<ResponseFormatter> InitRecoveryAsync(InitRecoveryIncomingObj incomingObj)
        {
            // Initialize the current phone variable
            string phone = string.Empty;

            // Try to disassemble the phone in UA format
            try
            {
                PhoneNumberUtil phoneUntil = PhoneNumberUtil.GetInstance();
                PhoneNumber phoneNumber = phoneUntil.Parse(incomingObj.Phone, "UA");

                phone = "+" + phoneNumber.CountryCode + phoneNumber.NationalNumber;
            }
            catch
            {
                // Response error
                return new ResponseFormatter(message: "Вказаний номер не підтримується.");
            }


            // Generate scheme
            CYberFormatter cyberFormatter = new CYberFormatter();
            string phoneShema = cyberFormatter.EncryptMethod(phone, _encryptOptions.PhoneKey);

            string code = GenerateCode();
            DateTime dateTime = DateTime.UtcNow;

            RecoveryModel recovery = new RecoveryModel
            {
                CreatedDate = dateTime,
                Token = cyberFormatter.EncryptMethod(phone, _encryptOptions.TokenKey),
                Code = code
            };


            var dbPhone = await _dbContext.PhoneTable
                .Where(item => EF.Functions.Collate(item.Phone, "SQL_Latin1_General_CP1_CS_AS") == phoneShema)
                .Where(item => item.IsActive)
                .FirstOrDefaultAsync();

            if (dbPhone != null)
            {
                var lastRecovery = await _dbContext.RecoveryTable
                    .Where(item => EF.Functions.Collate(item.Token, "SQL_Latin1_General_CP1_CS_AS") == recovery.Token)
                    .OrderByDescending(item => item.CreatedDate)
                    .FirstOrDefaultAsync();

                if (lastRecovery != null && lastRecovery.ExpiredDate.AddMinutes(-2) >= dateTime)
                    // Response token
                    return new ResponseFormatter(type: HttpStatusCode.OK,
                        message: "Зачекайте ще хвилинку. Можливо СМС затримуєтсья.",
                        data: new
                        {
                            Token = lastRecovery.Token
                        });


                var dayRecoveryCount = await _dbContext.RecoveryTable
                    .Where(item => EF.Functions.Collate(item.Token, "SQL_Latin1_General_CP1_CS_AS") == recovery.Token)
                    .Where(item => item.ExpiredDate >= new DateTime(dateTime.Year, dateTime.Month, dateTime.Day))
                    .CountAsync();


                if (dayRecoveryCount >= ProgramSettings.MaxCodeCount)
                    // Response error
                    return new ResponseFormatter(message: "Сталася невідома помилка. Спробуйте відновити доступ пізніше.");


                _dbContext.RecoveryTable.Add(recovery);
                await _dbContext.SaveChangesAsync();


                await SendSMSCode("Код скидання паролю: " + code, phone);
            }

            // Response token
            return new ResponseFormatter(type: HttpStatusCode.OK,
                message: "На вказаний номер було надіслано СМС повідомлення з кодом.",
                data: new
                {
                    Token = recovery.Token
                });
        }
        public async Task<ResponseFormatter> RecoveryAsync(RecoveryIncomingObj incomingObj, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return new ResponseFormatter(message: "Код не дійсний!");

            var recovery = await _dbContext.RecoveryTable
                .Where(item => EF.Functions.Collate(item.Token, "SQL_Latin1_General_CP1_CS_AS") == token)
                .Where(item => item.Code == incomingObj.Code)
                .Where(item => item.ExpiredDate >= DateTime.UtcNow)
                .FirstOrDefaultAsync();

            if (recovery != null)
            {
                // Generate scheme
                CYberFormatter cyberFormatter = new CYberFormatter();
                string phone = cyberFormatter.DecryptMethod(token, _encryptOptions.TokenKey);
                string phoneShema = cyberFormatter.EncryptMethod(phone, _encryptOptions.PhoneKey);

                var dbPhone = await _dbContext.PhoneTable
                    .Where(item => EF.Functions.Collate(item.Phone, "SQL_Latin1_General_CP1_CS_AS") == phoneShema)
                    .FirstOrDefaultAsync();

                if (dbPhone != null)
                {
                    var password = await _dbContext.PasswordTable
                        .Where(item => item.Key == dbPhone.Key)
                        .Where(item => item.IsActive == true)
                        .FirstOrDefaultAsync();

                    if (password != null)
                    {
                        recovery.ExpiredDate = DateTime.UtcNow;

                        password.DeactivateDate = DateTime.UtcNow;
                        password.IsActive = false;

                        PasswordModel newPassword = new PasswordModel
                        {
                            Key = dbPhone.Key,
                            Password = cyberFormatter.EncryptMethod(incomingObj.Password, _encryptOptions.PasswordKey),
                            IsActive = true
                        };

                        _dbContext.RecoveryTable.Update(recovery);
                        _dbContext.PasswordTable.Add(newPassword);
                        _dbContext.PasswordTable.Update(password);
                        await _dbContext.SaveChangesAsync();


                        return new ResponseFormatter(type: HttpStatusCode.OK, message: "Пароль змінено успішно. Скористайтеся формою входу для продовження.");
                    }
                }
            }

            return new ResponseFormatter(message: "Код не дійсний!");
        }

        public async Task<ResponseFormatter> UserRoles(string token)
        {
            CYberFormatter cyberFormatter = new CYberFormatter();
            long userId = JsonSerializer.Deserialize<TokenInformation>(cyberFormatter.DecryptMethod(token, _encryptOptions.TokenKey))!.UserId;

            var roles = await _dbContext.EmployeesTable
                .Where(item => item.UserId == userId)
                .Select(item => new
                {
                    item.Role,
                    item.CompanyId
                })
                .Distinct()
                .ToListAsync();

            return new ResponseFormatter(
                type: HttpStatusCode.OK,
                data: roles
                );
        }

        private async Task SendSMSCode(string message, string phone)
        {
            SMSClub smsClub = new SMSClub(_smsOptions.Key);
            smsClub.AddPhone(phone);

#if DEBUG
            Console.WriteLine(message);
#else
            var result = await smsClub.PostSMSAsync(_smsOptions.AlphaName, message);
#endif
        }
        private string GenerateCode(int len = ProgramSettings.CodeLength)
        {
            string _code = "";

            Random random = new Random();
            for (int i = 0; i < len; i++)
                _code += random.Next(9).ToString();

            return _code;
        }
    }
}
