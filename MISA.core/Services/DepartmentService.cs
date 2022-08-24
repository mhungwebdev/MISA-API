using MISA.core.Entities;
using MISA.core.Interfaces.RepositoryInterface;
using MISA.core.Interfaces.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.core.Services
{
    public class DepartmentService:BaseService<Department>, IDepartmentService
    {
        public DepartmentService(IDepartmentRepository departmentRepository):base(departmentRepository)
        {

        }
    }
}
