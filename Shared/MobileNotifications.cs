using System.ComponentModel.DataAnnotations;

namespace HVMDash.Shared
{
    public class MobileNotifications
    {
        [Required]
        [StringLength(30, ErrorMessage = "Topic length at least 3 characters long but not more 30.", MinimumLength = 3)]
        public string Topic { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Title must be at least 3 characters long.", MinimumLength = 3)]
        public string Title { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Body must be at least 5 characters long.", MinimumLength = 5)]
        public string Body { get; set; }

        //[Required]
        //[Compare(nameof(Body))]
        //public string Password2 { get; set; }
    }
}
