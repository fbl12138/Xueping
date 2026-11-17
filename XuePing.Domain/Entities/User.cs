using System;
using System.Collections.Generic;
using System.Text;

namespace XuePing.Domain.Entities;
public class User
{
public long Id { get; set; } // 用 long，为雪花ID做准备
public string Phone { get ; set ; } = string .Empty; // 手机号
public string Password { get ; set ; } = string .Empty; // 密码（哈希）
public string NickName { get ; set ; } = string .Empty; // 昵称
public string Icon { get ; set ; } = string .Empty; // 头像
public DateTime CreatedAt { get ; set ; }
public DateTime UpdatedAt { get ; set ; }
}