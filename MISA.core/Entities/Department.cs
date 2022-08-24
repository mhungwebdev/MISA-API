using MISA.core.MISAAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.core.Entities
{
    public class Department : InfoInteractive
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [PrimaryKey]
        [NotUpdate]
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Mã phòng ban
        /// </summary>
        [Require]
        [PropertyName("Mã phòng ban")]
        [Length(20)]
        public String? DepartmentCode { get; set; }

        /// <summary>
        /// Tên phòng ban
        /// </summary>
        [Require]
        [PropertyName("Tên phòng ban")]
        [Length(100)]
        public String? DepartmentName { get; set; }
    }
}
