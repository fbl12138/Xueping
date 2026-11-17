using System;
using System.Collections.Generic;
using System.Text;

namespace XuePing.Application.DTOs
{
    public class LoginResponseDto
    {
        public long UserId {  get; set; }
        public string Token { get; set; } = string.Empty;
        public string NickName {  get; set; } = string.Empty;
        public string Icon {  get; set; } = string.Empty;
    }
}
