using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XuePing.Application.DTOs
{
    public class SendCodeRequest
    {
        [Required(ErrorMessage ="手机号不能为空")]
        [RegularExpression(@"1[3-9]\d{9}$",ErrorMessage = "手机号格式不正确")]
        public string Phone { get; set;  } = string.Empty;
    }
}
