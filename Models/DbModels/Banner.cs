using System.ComponentModel.DataAnnotations.Schema;

namespace sarw_rp.Models.DbModels
{
    public class Banner
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Place { get; set; }
        public string ArticleUrl { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}
