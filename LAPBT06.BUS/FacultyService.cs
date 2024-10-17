using LAPBT06.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAPBT06.BUS
{
    public class FacultyService
    {
        public List<Faculty> GetAll()
        {
            ModelSinhvienDB sinhvienDB = new ModelSinhvienDB();
            return sinhvienDB.Faculty.ToList();
        }
    }
}
