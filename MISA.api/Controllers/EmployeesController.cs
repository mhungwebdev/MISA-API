using Microsoft.AspNetCore.Mvc;
using MISA.core.Entities;
using MISA.core.Interfaces.RepositoryInterface;
using MISA.core.Interfaces.ServiceInterface;

namespace MISA.api.Controllers
{
    public class EmployeesController : MISABaseController<Employee>
    {
        #region Contructor
        IEmployeeRepository _employeeRepository;
        public EmployeesController(IEmployeeRepository employeeRepository,IEmployeeService employeeService):base(employeeRepository,employeeService)
        {
            _employeeRepository = employeeRepository;
        }
        #endregion

        #region Filter
        /// <summary>
        /// Api phân trang + tìm kiếm
        /// Author : mhungwebdev (28/7/2022)
        /// </summary>
        /// <param name="pageSize">Số bản ghi trên một trang</param>
        /// <param name="pageNumber">Trang hiện tại</param>
        /// <param name="keyword">Từ khóa tìm kiếm</param>
        /// <returns>total record, total page, current page, data (ds nhân viên)</returns>
        [HttpGet("filter")]
        public IActionResult Filter(int pageSize, int pageNumber, string? keyword)
        {
            var res = _employeeRepository.Filter(pageSize, pageNumber, keyword);

            return Ok(res);
        }
        #endregion

        #region NewEmployeeCode
        /// <summary>
        /// Api lấy mã nhân viên mới
        /// Author : mhungwebdev (28/7/2022)
        /// </summary>
        /// <returns>mã nhân viên mới</returns>
        [HttpGet("NewEmployeeCode")]
        public async Task<IActionResult> GenerateNewEmployeeCode()
        {
            var res = await _employeeRepository.GenerateNewEmployeeCode();

            return Ok(res);
        }
        #endregion
        
        #region DeleteMulti
        /// <summary>
        /// Api xóa nhiều
        /// Author : mhungwebdev (10/8/2022)
        /// </summary>
        /// <param name="listId">danh sách id những thằng muốn xóa</param>
        /// <returns>số bản ghi đã xóa</returns>
        [HttpDelete("multi")]
        public IActionResult DeleteMulti(List<Guid> listId)
        {
            var res = _employeeRepository.DeleteMulti(listId);

            return Ok(res);
        }
        #endregion

        #region Export Data
        /// <summary>
        /// Func : Xuất khẩu dữ liệu
        /// Author : mhungwebdev (13/8/2022)
        /// </summary>
        /// <returns>Đường path tới file</returns>
        [HttpGet("ExportData")]
        public async Task<IActionResult> ExportData()
        {
            Excel excel = await _employeeRepository.ExportDataToExcel();

            return File(excel.FileContents,excel.ContentType,excel.FileName);
        }
        #endregion
    }
}
