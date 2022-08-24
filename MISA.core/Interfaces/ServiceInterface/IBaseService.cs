using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.core.Interfaces.ServiceInterface
{
    public interface IBaseService<MISAEntity>
    {
        #region Insert Service
        /// <summary>
        /// Thêm mới bản ghi có thực hiện nghiệp vụ
        /// Author : mhungwebdev (28/7/2022)
        /// </summary>
        /// <param name="entity">đối tượng muốn thêm mới</param>
        /// <returns>1 nếu thành công, 0 nếu thất bại</returns>
        int Insert(MISAEntity entity);
        #endregion

        #region Update service
        /// <summary>
        /// Sửa một bản ghi có thực hiện nghiệp vụ
        /// Author : mhungwebdev (28/7/2022)
        /// </summary>
        /// <param name="entity">Đối tượng muốn update</param>
        /// <returns>1 nếu thành công, 0 nếu thất bại</returns>
        int Update(MISAEntity entity, Guid id);
        #endregion
    }
}
