using MISA.core.MISAAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.core.Entities
{
    public class InfoInteractive
    {
        /// <summary>
        /// Ngày tạo
        /// </summary>
        [DateField]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// Ngày sửa
        /// </summary>
        [DateField]
        public DateTime? ModifyDate { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        [Length(100)]
        public String? CreateBy { get; set; }

        /// <summary>
        /// Người sửa
        /// </summary>
        [Length(100)]
        public String? ModifyBy { get; set; }
    }
}
