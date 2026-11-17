using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XuePing.Application.DTOs
{
    public class CreateReviewRequest
    {
        [Range(1,int.MaxValue,ErrorMessage = "请选择有效课程")]
        public int  CourseId { get; set; }
        [Required(ErrorMessage = "评论内容不能为空")]
        [StringLength(500)]
        public string Content { get; set; } = string.Empty;
        [Required(ErrorMessage = "评分不能为空")]
        [Range(1, 5)]
        public int Score { get; set; }
    }
}
