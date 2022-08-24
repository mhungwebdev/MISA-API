using MISA.core.Entities;
using MISA.core.Interfaces.RepositoryInterface;
using MISA.core.Interfaces.ServiceInterface;
using MISA.core.MISAAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.core.Services
{
    public class EmployeeService:BaseService<Employee>, IEmployeeService
    {
        IBaseRepository<Employee> _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository):base(employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        #region Validate Custome
        public override Tuple<string, string, List<string>> ValidateCustome(Employee employee, Guid id)
        {
            List<string> errors = new List<string>();
            string userMsg = string.Empty;
            string typeError = string.Empty;

            if (employee.DateOfBirth > DateTime.Now)
            {
                var propDateOfBirth = typeof(Employee).GetProperties().Where(prop => prop.Name == "DateOfBirth").ToList();

                errors.Add(String.Format(Resources.Resources.DateEmployeeErrorMsg, "Ngày sinh"));
                typeError = Resources.Resources.VALID_INPUT;
            }

            if(employee.IdentityDate > DateTime.Now)
            {
                var propIdentityDate = typeof(Employee).GetProperties().Where(prop => prop.Name == "IdentityDate").ToList();

                errors.Add(String.Format(Resources.Resources.DateEmployeeErrorMsg, "Ngày cấp"));
                typeError = Resources.Resources.VALID_INPUT;
            }

            var employeeCodeEnd = employee.EmployeeCode[employee.EmployeeCode.Length - 1];

            if (!(int.TryParse(employeeCodeEnd.ToString(), out int n)))
            {
                errors.Add(Resources.Resources.EmployeeCodeErrorMsg);
                typeError = Resources.Resources.VALID_INPUT;
            }

            if (_employeeRepository.CheckUnique(employee.EmployeeCode,"EmployeeCode", id))
            {
                typeError = Resources.Resources.DUP_EMPLOYEECODE;
                userMsg = String.Format(Resources.Resources.DuplicateEmployeeCode,employee.EmployeeCode);
            }

            Tuple<string, string, List<string>> error = new Tuple<string, string, List<string>>(typeError,userMsg,errors);

            return error;
        }
        #endregion
    }
}
