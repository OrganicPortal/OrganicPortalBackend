using OrganicPortalBackend.Models.Database.User;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OrganicPortalBackend.Models.Database.Company
{
    /*
     * Таблиця з інформацією про співробітників компанії, та їх ролей
     * Інформація вноситься при першій реєстрації компанії. Користувач є базовим менеджером
     * Можуть підтягуватися користувачі з системи і додаватися до компанії в якості співробітників
     * 
     * Вноситсья користувачем
     */
    public class EmployeesModel
    {
        // Базова інформація
        /* */
        public long Id { get; set; }
        public DateTime CreatedDate { get; init; } = DateTime.UtcNow;
        /* */


        // Дані співробітників
        /* */
        [Required]
        [AllowedValues(EnumUserRole.Owner, EnumUserRole.Manager)]
        // Роль користувача
        // #exemple::Owner (Власник)
        public EnumUserRole Role { get; set; } = EnumUserRole.Manager;
        /* */


        // Список зв'язків в таблицях бд
        /* */
        [Required]
        [Range(1, long.MaxValue)]
        // Ідентифікатор компанії
        public long CompanyId { get; set; }
        public CompanyModel? Company { get; set; } = null;


        [Required]
        [Range(1, long.MaxValue)]
        // Ідентифікатор користувача
        public long UserId { get; set; }
        public UserModel? User { get; set; } = null;
        /* */
    }

    // Список доступних ролей
    public enum EnumUserRole
    {
        [Description("Відвідувач")]
        Visitor = 0,
        [Description("Користувач")]
        User = 1,

        [Description("Власник")]
        Owner = 10,
        [Description("Менеджер")]
        Manager = 11,

        [Description("Системний адміністратор")]
        SysAdmin = 99,
        [Description("Системний менеджер")]
        SysManager = 100,
    }
}
