using Solnet.Programs;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System.Text;
using System.Text.Json;

namespace OrganicPortalBackend.Services
{
    public interface ISolana
    {
        // Отримання акаунту програми та запис даних в акаунт
        public Task<(bool, CallProgramResponse)> CallProgramAndWriteInformationAsync(Account walletAccount, PublicKey programId, string jsonData);
        // Читання інформації з облікового запису програми
        public Task<(bool, string)> ReadAccountInformationAsync(PublicKey pubKey);
        // Отримання акаунту з файлу
        public Task<Account> GetAccountFromFileAsync(string patch);
    }
    public class SolanaService : ISolana
    {
        public readonly IRpcClient _rpcClient = ClientFactory.GetClient(Cluster.DevNet);
        public SolanaService() { }


        // Генерація інструкції по створенню акаунта за допомогою програми
        public (TransactionInstruction, Account) GenerateNewAccountProgramInstruction(Account walletAccount, PublicKey programId, ulong lamports, ulong space)
        {
            // Генеруємо обліковий запис
            Account account = new Account();

            // Створюємо інструкцію для додавання акаунту
            var accounInstruction = SystemProgram.CreateAccount(
                // Акаунт з якого здійснюється генерація (власник)
                fromAccount: walletAccount.PublicKey,
                // Акаунт який додаєтсья
                newAccountPublicKey: account.PublicKey,
                // Вартість транзакції
                lamports: lamports,
                // Розмір хранилища
                space: space,
                // Адреса програми
                programId: programId
                );

            // повертаємо інструкцію
            return (accounInstruction, account);
        }

        // Інструкція виклику програми (внесення опису в обліковий запис)
        public TransactionInstruction GenerateProgramCallInstruction(PublicKey programId, PublicKey accountKey, byte[] dataBytes)
        {
            // Створення інструкції виклику програми
            var programIx = new TransactionInstruction
            {
                ProgramId = programId,
                Keys = new List<AccountMeta>
                {
                    AccountMeta.Writable(accountKey, false)
                },
                Data = dataBytes
            };

            // повертаємо інструкцію
            return programIx;
        }

        // Формування нового запису про насіння
        public async Task<(bool, CallProgramResponse)> CallProgramAndWriteInformationAsync(Account walletAccount, PublicKey programId, string jsonData)
        {
            // Формуємо масив байтів
            byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

            // Визначаємо розмір хранилища
            ulong space = (ulong)jsonBytes.Length;
            RequestResult<ulong> requestLamport = await _rpcClient.GetMinimumBalanceForRentExemptionAsync((int)space);

            // Якщо все впорядку
            if (requestLamport.WasSuccessful)
            {
                // Отримуємо вартість
                ulong lamports = requestLamport.Result;

                // Генеруємо інструкцію створення облікового запису
                var accountTuple = GenerateNewAccountProgramInstruction(walletAccount, programId, lamports, space);
                TransactionInstruction createAccountIx = accountTuple.Item1;
                Account account = accountTuple.Item2;

                // Генеруємо інструкцію використання програми
                var programIx = GenerateProgramCallInstruction(programId, account.PublicKey, jsonBytes);

                // Отримуємо hash
                var recentHash = _rpcClient.GetLatestBlockHash().Result.Value;

                // Створюємо транзакцію
                var tx = new TransactionBuilder()
                    // Передаємо акаунт платника
                    .SetFeePayer(walletAccount)
                    // Вказуємо Blockhash
                    .SetRecentBlockHash(recentHash.Blockhash)
                    // Додаємо інструкцію на додавання користувача
                    .AddInstruction(createAccountIx)
                    // Додаємо інструкцію виклику програми
                    .AddInstruction(programIx)
                    // Передаємо інформацію про облікові записи
                    .Build(new List<Account> { walletAccount, account });


                // Виконуємо транзакцію
                var txSig = await _rpcClient.SendTransactionAsync(tx);
                if (txSig.WasSuccessful)
                {
                    // Отримуємо сигнатуру транзакції
                    var sig = txSig.Result;

                    // Повертаємо успішну відповідь
                    return (true, new CallProgramResponse
                    {
                        Account = account,
                        ProgramId = programId,
                        Signature = sig
                    });
                }

                // Повертаємо повідомлення про проблему
                return (false, null);
            }

            return (false, null);
        }

        // Отримання інформації з облікового запису насіння
        public async Task<(bool, string)> ReadAccountInformationAsync(PublicKey pubKey)
        {
            // Кидаємо запис на отримання інформації про акаунт
            var accountInfo = await _rpcClient.GetAccountInfoAsync(pubKey);
            if (accountInfo.WasSuccessful && accountInfo.Result?.Value != null)
            {
                // Отримуємо записані дані
                var dataBase64 = accountInfo.Result.Value.Data[0];

                // Формуємо масив байті
                var dataBytes = Convert.FromBase64String(dataBase64);

                // Декодуємо з UTF8 та очищаємо пусті байти
                string jsonString = Encoding.UTF8.GetString(dataBytes).TrimEnd('\0');

                // Повертаємо успішну відповідь
                return (true, jsonString);
            }

            // Повертаємо помилку
            return (false, "");
        }

        // Отримання акаунту з файлу
        public async Task<Account> GetAccountFromFileAsync(string patch)
        {
            // Читаємо файлу
            string fileContent = await System.IO.File.ReadAllTextAsync(patch);

            // Серіалізуємо json
            byte[] keyByte = Array.ConvertAll(JsonSerializer.Deserialize<int[]>(fileContent)!, i => (byte)i);

            // Створюємо акаунт з масиву байтів
            Account account = new Account(keyByte, keyByte[32..]);

            return account;
        }
    }


    // Модель з інформацією про виклик програми
    public class CallProgramResponse
    {
        // Акаунт запису
        public Account Account { get; set; } = new Account();
        // Адреса програми
        public string ProgramId { get; set; } = "";
        // Сигнатура транзакції
        public string Signature { get; set; } = "";
    }
}
