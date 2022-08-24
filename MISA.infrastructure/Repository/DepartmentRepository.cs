using MISA.core.Entities;
using MISA.core.Interfaces.RepositoryInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.infrastructure.Repository
{
    public class DepartmentRepository:BaseRepository<Department>, IDepartmentRepository
    {
    }
}
