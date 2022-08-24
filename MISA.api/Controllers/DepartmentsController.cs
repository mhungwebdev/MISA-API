using Microsoft.AspNetCore.Mvc;
using MISA.core.Entities;
using MISA.core.Interfaces.RepositoryInterface;
using MISA.core.Interfaces.ServiceInterface;

namespace MISA.api.Controllers
{
    public class DepartmentsController :MISABaseController<Department>
    {
        public DepartmentsController(IDepartmentRepository departmentRepository,IDepartmentService departmentService):base(departmentRepository,departmentService)  
        {

        }
    }
}
