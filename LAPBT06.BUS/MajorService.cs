using LAPBT06.DAL.Entities;
using System.Collections.Generic;
using System.Linq;

namespace LAPBT06.BUS
{
    public class MajorService
    {
        public List<Major> GetAllByFaculty(int facultyID)
        {
            ModelSinhvienDB sinhvienDB = new ModelSinhvienDB();
            return sinhvienDB.Major.Where(p => p.FacultyID == facultyID).ToList();
        }
    }
}
