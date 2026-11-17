using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Services;
using XuePing.Domain.Abstractions;

namespace XuePing.Application
{
        public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IGetAllCoursesAsync,GetAllCoursesService>();
            services.AddScoped<IGetCourseByIdAsync, GetCourseByIdService>();
            services.AddScoped<ICreateCourseAsync, CreateCourseService>();
            services.AddScoped<IUpdateCourseAsync, UpdateCourseService>();
            services.AddScoped<IDeleteCourseAsync, DeleteCourseService>();
            services.AddScoped<ISendCodeAsync, SendCodeService>();
            services.AddScoped<ILoginAsync, LoginService>();
            services.AddScoped<ISelectCourseAsync,SelectCourseService>();
            services.AddScoped<IGetSeckillQuotaAsync,GetSeckillQuotaService>();
            services.AddScoped<ICreateReviewAsync, CreateReviewService>();
            services.AddScoped<IDeleteReviewAsync, DeleteReviewService>();
            services.AddScoped<IGetReviewByCourseAsync, GetReviewsByCourseService>();
            services.AddScoped<IUnlikeReviewAsync, UnlikeReviewService>();
            services.AddScoped<ILikeReviewAsync, LikeReviewService>();
            services.AddScoped<IGetReviewLikeRankAsync, GetReviewLikeRankService>();
            services.AddScoped<IFollowUserAsync, FollowUserService>();
            services.AddScoped<IUnfollowUserAsync, UnfollowUserService>();
            services.AddScoped<IIsFollowingAsync, IsFollowingService>();
            services.AddScoped<IGetCommonFollowsAsync, GetCommonFollowsService>();
            services.AddScoped<IPublishPostAsync, PublishPostService>();
            services.AddScoped<IGetFeedAsync, GetFeedService>();
            services.AddScoped<ISignAsync, SignService>();
            services.AddScoped<IGetSignStatusAsync, GetSignStatusService>();
            services.AddScoped<IGetCourseUvAsync, GetCourseUvService>();
            services.AddScoped<IGetSeckillStatusAsync, GetSeckillStatusService>();
            services.AddScoped<ISeckillOrderHandler, SeckillOrderHandler>();
            return services;
        }
    }
}
