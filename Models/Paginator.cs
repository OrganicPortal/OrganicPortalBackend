using System.ComponentModel.DataAnnotations;

namespace OrganicPortalBackend.Models
{
    public class Paginator
    {
        [Required]
        [Range(1, int.MaxValue)]
        // Встановлює номер сторінки
        // #example::1
        public int Page { get; set; }

        [Required]
        [Range(1, 500)]
        // Встановлює розмір сторінки
        // #example::50
        public int PageSize { get; set; }


        // Повертає значення для параметра Skip
        public int Skip { get { return PageSize * Page - PageSize; } }
    }
}
