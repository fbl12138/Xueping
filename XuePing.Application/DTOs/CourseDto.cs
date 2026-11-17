
namespace XuePing.Application.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Images { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double AvgScore { get; set; }
        public int CommentCount { get; set; }
        public string OpenHours { get; set; } = string.Empty;

    }
}
