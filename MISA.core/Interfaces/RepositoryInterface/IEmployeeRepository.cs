using MISA.core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.core.Interfaces.RepositoryInterface
{
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        /// <summary>
        /// Paging và tìm kiếm
        /// Author : mhungwebdev (28/7/2022)
        /// </summary>
        /// <param name="pageSize">Số bản ghi trên một trang</param>
        /// <param name="pageNumber">Trang hiện tại</param>
        /// <param name="keyword">Từ khóa tìm kiếm</param>
        /// <returns>Các bản ghi phù hợp</returns>
        object Filter(int pageSize, int pageNumber, string keyword);

        /// <summary>
        /// Sinh ra một mã nhân viên mới
        /// Author : mhungwebdev (28/7/2022)
        /// </summary>
        /// <returns>Mã nhân viên với</returns>
        Task<string> GenerateNewEmployeeCode();

        /// <summary>
        /// Xóa nhiều bản ghi
        /// Author : mhungwebdev (10/8/2022)
        /// </summary>
        /// <param name="listId">danh sách id các đối tượng muốn xóa</param>
        /// <returns>số bản ghi bị xóa</returns>
        int DeleteMulti(List<Guid> listId);

        /// <summary>
        /// Xuất khẩu dữ liệu
        /// Author : mhungwebdev (10/8/2022)
        /// </summary>
        /// <returns>mảng byte</returns>
        Task<Excel> ExportDataToExcel();
    }
}
