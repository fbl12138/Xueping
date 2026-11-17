using System;
using System.Collections.Generic;
using System.Text;

namespace XuePing.Domain.Entities;
public class CourseCategory {
public int Id { get; set; }
public string Name { get; set; } = string.Empty; // 分类名：如"专业课"
public string Icon { get ; set ; } = string .Empty; // 图标
public int Sort { get ; set ; } // 排序权重
  }