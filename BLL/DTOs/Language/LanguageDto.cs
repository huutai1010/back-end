

namespace BLL.DTOs.Language
{
    public class LanguageDto : BaseDto
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public string FileLink { get; set; }
        public string LanguageCode { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int Status { get; set; }
    }
}
