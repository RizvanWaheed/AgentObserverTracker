using System.ComponentModel.DataAnnotations;

namespace AgentObserver.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}