using System;
using System.Collections.Generic;
using System.Text;

namespace XuePing.Domain.Entities
{
    public class Course
    {
    public int Id { get; set; }
    public int CategoryId { get; set; } // 课程分类外键
    public string Name { get ; set ; } = string .Empty; // 课程名称
    public string Images { get ; set ; } = string .Empty; // 封面图URL（逗号分隔）
    public string Area { get ; set ; } = string .Empty; // 开课地点
    public string Address { get ; set ; } = string .Empty; // 详细地址
    public double AvgScore { get ; set ; } // 平均评分
    public int CommentCount { get ; set ; } // 评论数
    public string OpenHours { get ; set ; } = string .Empty; // 开课时间
    public DateTime CreatedAt { get ; set ; } // 创建时间（审计）
    public DateTime UpdatedAt { get ; set ; } // 更新时间（审计）
    }
}
