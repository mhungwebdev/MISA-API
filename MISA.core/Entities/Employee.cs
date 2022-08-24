using MISA.core.Enum;
using MISA.core.MISAAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.core.Entities
{
    public class Employee : InfoInteractive
    {
        /// <summary>
        /// Khoá chính
        /// </summary>
        [PrimaryKey]
        [NotUpdate]
        [Length(36)]
        public Guid? EmployeeId{ get; set; }

        /// <summary>
        /// Tên nhân viên
        /// </summary>
        [Require]
        [Length(100)]
        [PropertyName("Tên nhân viên"),Search]
        public String? FullName { get; set; }

        /// <summary>
        /// Id phòng ban
        /// </summary>
        [Length(36)]
        [Require]
        [PropertyName("Phòng ban")]
        [ForeignKey]
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Mã nhân viên
        /// </summary>
        [Require,Search,PropertyName("Mã nhân viên")]
        [Length(20)]
        public String? EmployeeCode { get; set; }

        /// <summary>
        /// Tên chức vụ
        /// </summary>
        [Length(255)]
        [PropertyName("Tên chức vụ")]
        public String? PositionName { get; set; }

        /// <summary>
        /// Ngày sinh nhân viên
        /// </summary>
        [DateField]
        [PropertyName("Ngày sinh")]
        public DateTime? DateOfBirth{ get; set; }

        /// <summary>
        /// Giới tính (0 - nam, 1 - nữ, 2 - khác)
        /// </summary>
        [PropertyName("Giới tính")]
        public Gender? Gender { get; set; }

        /// <summary>
        /// Số căn cước công dân
        /// </summary>
        [Length(25)]
        [PropertyName("Số căn cước công dân")]
        public String? IdentityNumber { get; set; }

        /// <summary>
        /// Nơi cấp căn cước công dân
        /// </summary>
        [Length(255)]
        public String? IdentityPlace { get; set; }

        /// <summary>
        /// Ngày cấp căn cước công dân
        /// </summary>
        [DateField]
        [PropertyName("Ngày cấp")]
        public DateTime? IdentityDate { get; set; }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        [Length(255)]
        [PropertyName("Địa chỉ")]
        public String? Address { get; set; }

        /// <summary>
        /// Số điện thoại di động
        /// </summary>
        [Length(50),Search]
        [PropertyName("Số điện thoại di động")]
        public String? PhoneNumber { get; set; }

        /// <summary>
        /// Số điện thoại cố định
        /// </summary>
        [Length(50)]
        [PropertyName("Số điện thoại cố định")]
        public String? LandlineNumber { get; set; }

        /// <summary>
        /// Email nhân viên
        /// </summary>
        [Email]
        [Length(100)]
        public String? Email { get; set; }

        /// <summary>
        /// Số tài khoản ngân hàng
        /// </summary>
        [Length(25)]
        [PropertyName("Số tài khoản ngân hàng")]
        public String? BankAccount { get; set; }

        /// <summary>
        /// Tên ngân hàng
        /// </summary>
        [Length(255)]
        [PropertyName("Tên ngân hàng")]
        public String? BankName { get; set; }

        /// <summary>
        /// Chi nhánh ngân hàng
        /// </summary>
        [Length(255)]
        [PropertyName("Tên chi nhánh")]
        public String? BankBranch { get; set; }

        /// <summary>
        /// Giới tính dạng text
        /// </summary>
        [NotMap]
        [NotUpdate]        
        public String? GenderName { get
            {
                switch (Gender)
                {
                    case Enum.Gender.MALE:
                        return Resources.Resources.GenderMale;
                    case Enum.Gender.FEMALE:
                        return Resources.Resources.GenderFeMale;
                    case Enum.Gender.OTHER:
                        return Resources.Resources.GenderOther;
                    default:
                        return null;
                };
            } }

        /// <summary>
        /// Tên phòng ban
        /// </summary>
        [NotMap]
        [NotUpdate]
        public String? DepartmentName { get;}
    }
}
