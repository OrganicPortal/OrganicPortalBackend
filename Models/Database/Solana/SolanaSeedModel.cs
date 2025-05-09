using OrganicPortalBackend.Models.Database.Seed;
using System.ComponentModel.DataAnnotations;

namespace OrganicPortalBackend.Models.Database.Solana
{
    public class SolanaSeedModel
    {
        // Базова інформація
        /* */
        [Key]
        public long Id { get; set; }
        public DateTime CreatedDate { get; init; } = DateTime.UtcNow;

        // Ключ історії записів
        public string Key { get; set; } = string.Empty;

        // Ключ історії записів
        public string HistoryKey { get; set; } = string.Empty;
        /* */


        // Інформація про запис в Solana
        /* */
        public string AccountPrivateKey { get; set; } = string.Empty; // #shema
        public string AccountPublicKey { get; set; } = string.Empty;

        public ICollection<SignatureModel> SignatureList { get; set; } = new List<SignatureModel>();
        /* */


        // Інформація для фільтрації даних
        /* */
        [Required]
        // Узагальнене ім'я культури
        // #exemple::Кукурудза цукрова
        public string Name { get; set; } = string.Empty;

        [Required]
        // Сорт рослини
        // #exemple::Медунка F1
        public string Variety { get; set; } = string.Empty;

        [Required]
        // Тип насіння (органічне, гібридне тощо)
        // #exemple::Гібрид F1
        public string SeedType { get; set; } = string.Empty;

        [Required]
        // Оброблене насіння
        // #exemple::Untreated (Не оброблене)
        public EnumTreatmentType TreatmentType { get; set; } = EnumTreatmentType.Uncnown;

        [Required]
        // Ім'я установи (повна назва)
        // #exemple::ТМ Яскрава
        public string CompanyName { get; set; } = string.Empty;
        /* */
    }
}
