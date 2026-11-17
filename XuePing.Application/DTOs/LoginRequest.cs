using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XuePing.Application.DTOs
{
    public class LoginRequest
    {
        [Required(ErrorMessage ="手机号不能为空")]
        [RegularExpression(@"1[3-9]\d{9}$",ErrorMessage ="手机号格式错误")]
        public string Phone { get; set; } = string.Empty;
        [Required(ErrorMessage ="验证码不能为空")]
        [StringLength(6,MinimumLength = 6,ErrorMessage ="验证码格式错误")]
        public string Code { get; set; } = string.Empty;
    }
}
