using Microsoft.AspNetCore.Mvc;
using MISA.core.Interfaces.RepositoryInterface;
using MISA.core.Interfaces.ServiceInterface;

namespace MISA.api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MISABaseController<MISAEntity> : ControllerBase
    {
        #region Contructor
        IBaseRepository<MISAEntity> _baseRepository;
        IBaseService<MISAEntity> _baseService;

        public MISABaseController(IBaseRepository<MISAEntity> baseRepository, IBaseService<MISAEntity> baseService)
        {
            _baseRepository = baseRepository;
            _baseService = baseService;
        }
        #endregion

        #region Get
        /// <summary>
        /// Lấy tất cả bản ghi
        /// Author : mhungwebdev (28/7/2022)
        /// </summary>
        /// <returns>Tất cả bản ghi</returns>
        [HttpGet]
        public IActionResult Get()
        {
            var res = _baseRepository.Get();

            var number = 1;
            return Ok(res);
        }
        #endregion

        #region Get by id
        /// <summary>
        /// Lấy theo id
        /// Author : mhungwebdev (28/7/2022)
        /// </summary>
        /// <param name="id">id của bản ghi</param>
        /// <returns>Bản ghi có id tương ứng</returns>
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var res = _baseRepository.Get(id);

            return Ok(res);
        }
        #endregion

        #region Delete
        /// <summary>
        /// Xóa theo id
        /// Author : mhungwebdev (28/7/2022)
        /// </summary>
        /// <param name="id">id bản ghi muốn xóa</param>
        /// <returns>1 nếu thành công</returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var res = _baseRepository.Delete(id);

            return Ok(res); 
        }
        #endregion

        #region Insert
        /// <summary>
        /// Thêm mới 1 bản ghi
        /// Author : mhungwebdev (28/7/2022)
        /// </summary>
        /// <param name="entity">bản ghi mới</param>
        /// <returns>1 nếu thành công</returns>
        [HttpPost]
        public IActionResult Insert(MISAEntity entity)
        {
            var res = _baseService.Insert(entity);

            return StatusCode(201, res);
        }
        #endregion
            
        #region Update
        /// <summary>
        /// Sửa bản ghi
        /// Author : mhungwebdev (28/7/2022)
        /// </summary>
        /// <param name="entity">bản ghi cập nhật</param>
        /// <param name="id">id bản ghi cập nhật</param>
        /// <returns>1 nếu update thành công</returns>
        [HttpPut("{id}")]
        public IActionResult Update(MISAEntity entity, Guid id)
        {
            var res = _baseService.Update(entity, id);

            return StatusCode(201, res);
        }
        #endregion
    }
}
