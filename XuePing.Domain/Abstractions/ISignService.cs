namespace XuePing.Domain.Abstractions
{
    /// <summary>
    /// 签到（Bitmap）：每个用户每月一个位图，第 N 位代表当月第 N 天是否签到
    /// </summary>
    public interface ISignService
    {
        /// <summary>
        /// 今天签到。已经签过返回 false（幂等，不报错）
        /// </summary>
        Task<bool> SignAsync(long userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// 本月已签到的日期列表（如 [1, 2, 5]），供前端画日历
        /// </summary>
        Task<List<int>> GetSignedDaysAsync(long userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// 截至今天的连续签到天数；今天没签则为 0（不跨月累计）
        /// </summary>
        Task<int> GetConsecutiveDaysAsync(long userId, CancellationToken cancellationToken = default);
    }
}
