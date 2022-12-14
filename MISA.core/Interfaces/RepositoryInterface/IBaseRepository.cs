using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.core.Interfaces.RepositoryInterface
{
    public interface IBaseRepository<MISAEntity>
    {
        /// <summary>
        /// Lấy tất cả dữ liệu
        /// Author : mhungwebdev (28/7/2022)
        /// </summary>
        /// <returns>Trả về tất cả dữ liệu của bảng</returns>
        IEnumerable<MISAEntity> Get();

        /// <summary>
        /// Lấy bản ghi theo id
        /// Author : mhungwebdev (28/7/2022)
        /// </summary>
        /// <param name="id">id đối tượng muốn xóa</param>
        /// <returns>Trả về đối tượng trùng id</returns>
        MISAEntity Get(Guid id);

        /// <summary>
        /// Thêm mới 1 bản ghi
        /// Author : mhungwebdev (28/7/2022)
        /// </summary>
        /// <param name="entity">Đối tượng muốn thêm mới</param>
        /// <returns>1 nếu thành công</returns>
        int Insert(MISAEntity entity);

        /// <summary>
        /// Sửa một bản ghi
        /// Author : mhungwebdev (28/7/2022)
        /// </summary>
        /// <param name="entity">Thông tin mới của đối tượng được sửa </param>
        /// <returns>1 nếu thành công</returns>
        int Update(MISAEntity entity, Guid id);

        /// <summary>
        /// Xóa 1 bản ghi
        /// Author : mhungwebdev (28/7/2022)
        /// </summary>
        /// <param name="id">id của đối tượng muốn xóa</param>
        /// <returns>1 nếu thành công</returns>
        int Delete(Guid id);

        /// <summary>
        /// Kiểm tra sự tồn tại của bản ghi
        /// Autor : mhungwebdev (28/7/2022)
        /// </summary>
        /// <param name="id">id đối tượng muốn check</param>
        /// <returns>true nếu tồn tại, false nếu chưa tồn tại</returns>
        bool CheckExist(Guid id);

        /// <summary>
        /// Kiểm tra trùng lặp field unique
        /// Author : mhungwebdev (28/7/2022)
        /// </summary>
        /// <param name="value">value của field muốn check</param>
        /// <param name="fieldName">Tên field muốn check</param>
        /// <param name="id">id của đối tượng muốn check</param>
        /// <returns>true nếu trùng và false với trường hợp ngược lại</returns>
        bool CheckUnique(object value,string fieldName,Guid id);
    }
}
