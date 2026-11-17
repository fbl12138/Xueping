using Microsoft.AspNetCore.Mvc;
using XuePing.Application.DTOs;
using XuePing.Application.Services;
using XuePing.Application.Common;
using Microsoft.AspNetCore.Authorization;

namespace XuePing.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   
    public class CourseController :ControllerBase
    {
        private readonly IGetAllCoursesAsync _getAllCourses;
        private readonly IGetCourseByIdAsync _getCourseById;
        private readonly ICreateCourseAsync _createCourse; 
        private readonly IUpdateCourseAsync _updateCourse; 
        private readonly IDeleteCourseAsync _deleteCourse;
        private readonly IGetCourseUvAsync _getCourseUv;
        public CourseController (
            IGetAllCoursesAsync getAllCourses,
            IGetCourseByIdAsync getCourseById,
            ICreateCourseAsync createCourse,
            IUpdateCourseAsync updateCourse,
            IDeleteCourseAsync deleteCourse,
            IGetCourseUvAsync getCourseUv)
        {
            _getAllCourses = getAllCourses;
            _getCourseById = getCourseById;
            _createCourse = createCourse;
            _updateCourse = updateCourse;
            _deleteCourse = deleteCourse;
            _getCourseUv = getCourseUv;
        }
        //获取全部课程信息
        [HttpGet] 
        public async Task<ActionResult<Result<List<CourseDto>>>> GetAll(CancellationToken cancellationToken)
        { return Ok(await _getAllCourses.ExecuteAsync(cancellationToken)); }
        //获取特定课程信息
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Result<CourseDto>>> GetById(int id,CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();
            return Ok(await _getCourseById.ExecuteAsync(id,userId, cancellationToken));
        }
        //创建新课程信息
        [HttpPost]
        public async Task<ActionResult<Result<CourseDto>>> Create(CreateCourseRequest request,CancellationToken cancellationToken)
        {
            var result = await _createCourse.ExecuteAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
        }
        //更新课程信息
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Result<CourseDto>>> Update(int id,UpdateCourseRequest request,CancellationToken cancellationToken)
        {
            return Ok(await _updateCourse.ExecuteAsync(id, request, cancellationToken));
        }
        //删除课程信息
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Result<bool>>> Delete(int id,CancellationToken cancellationToken)
        {
           return Ok(await _deleteCourse.ExecuteAsync(id, cancellationToken));
        }
        //获取访客记录
        [HttpGet("{id:int}/uv")]
        public async Task<ActionResult<Result<CourseUvDto>>> GetUv(int id,CancellationToken cancellationToken)
        {
            return Ok(await _getCourseUv.ExecuteAsync(id, cancellationToken));
        }
    }
}
