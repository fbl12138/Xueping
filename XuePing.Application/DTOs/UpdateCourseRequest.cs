using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XuePing.Application.DTOs
{
    public class UpdateCourseRequest
    {
        [Required(ErrorMessage = "课程分类不能为空")]
        [Range(1, int.MaxValue, ErrorMessage = "请选择有效的课程分类")]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "课程名称不能为空")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "课程名称长度在2-50之间")]
        public string Name { get; set; } = string.Empty;

        public string Images { get; set; } = string.Empty;
        [Required(ErrorMessage = "请填写课程区域")]
        public string Area { get; set; } = string.Empty;
        [Required(ErrorMessage = "请填写开课地址")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "开课地址名称长度在2-50之间")]
        public string Address { get; set; } = string.Empty;
        [Required(ErrorMessage = "请填写开课时间")]

        public string OpenHours { get; set; } = string.Empty;
    }
}
