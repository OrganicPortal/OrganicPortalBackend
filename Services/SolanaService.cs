using Org.BouncyCastle.Utilities;
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
        public Task<(bool, CallProgramResponse)> CallProgramAndWriteInformationAsync(Account walletAccount, string programId, string jsonData);
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
        public TransactionInstruction GenerateProgramCallInstruction(PublicKey programId, PublicKey accountKey, PublicKey walletKey, byte[] dataBytes)
        {
            // Створення інструкції виклику програми
            var programIx = new TransactionInstruction
            {
                ProgramId = programId,
                Keys = new List<AccountMeta>
                {
                    AccountMeta.Writable(accountKey, false),
                    AccountMeta.ReadOnly(walletKey, true),
                },
                Data = dataBytes
            };

            // повертаємо інструкцію
            return programIx;
        }

        // Формування нового запису про насіння
        public async Task<(bool, CallProgramResponse)> CallProgramAndWriteInformationAsync(Account walletAccount, string programId, string jsonData)
        {
            // Список сигнатур 
            List<string> SignatureList = new();

            // Формуємо масив байтів
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

            // Визначаємо розмір хранилища (4 байти довжини)
            ulong space = (ulong)bytes.Length + 4;
            RequestResult<ulong> requestLamport = await _rpcClient.GetMinimumBalanceForRentExemptionAsync((int)space);
            if (!requestLamport.WasSuccessful)
                // Повертаємо повідомлення про проблему
                return (false, null);

            // Отримуємо вартість
            ulong lamports = requestLamport.Result;

            // Генеруємо інструкцію створення облікового запису
            var accountTuple = GenerateNewAccountProgramInstruction(walletAccount, new PublicKey(programId), lamports, space);
            TransactionInstruction createAccountIx = accountTuple.Item1;
            Account account = accountTuple.Item2;

            var recentHash = _rpcClient.GetLatestBlockHash().Result.Value;

            var txUser = new TransactionBuilder()
                // Передаємо акаунт платника
                .SetFeePayer(walletAccount)
                // Вказуємо Blockhash
                .SetRecentBlockHash(recentHash.Blockhash)
                // Додаємо інструкцію на додавання користувача
                .AddInstruction(createAccountIx)
                // Передаємо інформацію про облікові записи
                .Build(new List<Account> { walletAccount, account });
            var txSigUser = await _rpcClient.SendTransactionAsync(txUser);

            if (!txSigUser.WasSuccessful)
                // Повертаємо повідомлення про проблему
                return (false, null);

            // Записуємо сигнатуру
            string signature = "";
            signature = txSigUser.Result;
            SignatureList.Add(txSigUser.Result);

            // Проводимо очікування на додавання акаунту
            if (await TransactionSuccesed(signature) == false)
                return (false, null);

            // На випадок, якщо запис занадто довгий
            int i = 0;
            int chunkSize = 950;
            byte[][] arrays = bytes.GroupBy(s => i++ / chunkSize).Select(item => item.ToArray()).ToArray();
            foreach (byte[] el in arrays)
            {
                // Генеруємо інструкцію використання програми
                var programIx = GenerateProgramCallInstruction(new PublicKey(programId), account.PublicKey, walletAccount.PublicKey, el);

                var recentHash2 = _rpcClient.GetLatestBlockHash().Result.Value;

                // Створюємо транзакцію
                var tx = new TransactionBuilder()
                    // Передаємо акаунт платника
                    .SetFeePayer(walletAccount)
                    // Вказуємо Blockhash
                    .SetRecentBlockHash(recentHash2.Blockhash)
                    // Додаємо інструкцію виклику програми
                    .AddInstruction(programIx)
                    // Передаємо інформацію про облікові записи
                    .Build(new List<Account> { walletAccount });

                // Виконуємо транзакцію
                var txSig = await _rpcClient.SendTransactionAsync(tx);

                if (!txSig.WasSuccessful)
                    return (false, null);

                signature = txSig.Result;
                SignatureList.Add(txSig.Result);

                // Проводимо очікування на виконання транзакції
                if (await TransactionSuccesed(signature) == false)
                    return (false, null);
            }

            // Повертаємо успішну відповідь
            return (true, new CallProgramResponse
            {
                Account = account,
                ProgramId = programId,
                SignatureList = SignatureList
            });
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
                string jsonString = Encoding.UTF8.GetString(dataBytes[4..]).TrimEnd('\0');

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

        // Очікування транзакції в межах 35 с
        public async Task<bool> TransactionSuccesed(string signature)
        {
            for (int i = 0; i < 5; i++)
            {
                var transactionResult = await _rpcClient.GetTransactionAsync(signature);

                if (transactionResult.Result != null)
                {
                    if (transactionResult.Result.Meta.Error == null)
                        return true;

                    return false;
                }

                Thread.Sleep(7000);
            }

            return false;
        }
    }


    // Модель з інформацією про виклик програми
    public class CallProgramResponse
    {
        // Акаунт запису
        public Account Account { get; set; } = new Account();
        // Адреса програми
        public string ProgramId { get; set; } = "";
        // Список сигнатур
        public List<string> SignatureList { get; set; } = new List<string>();
    }
}
