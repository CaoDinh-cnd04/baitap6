using LAPBT06.DAL.Entities;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;

namespace LAPBT06.BUS
{
    public class StudentService
    {
        private ModelSinhvienDB sinhvienDB = new ModelSinhvienDB();

        public List<Student> GetAll()
        {
            return sinhvienDB.Student.Include("Faculty").Include("Major").ToList();
        }

        public List<Student> GetAllHasNoMajor()
        {
            return sinhvienDB.Student.Include("Faculty").Where(p => p.MajorID == null).ToList();
        }

        public Student FindByID(string studentID)
        {
            return sinhvienDB.Student.Include("Faculty").Include("Major").FirstOrDefault(p => p.StudentID == studentID);
        }

        public void InsertUpdate(Student s)
        {
            sinhvienDB.Student.AddOrUpdate(s);
            sinhvienDB.SaveChanges();
        }

        public void DeleteUpdate(Student s)
        {
            var student = sinhvienDB.Student.FirstOrDefault(st => st.StudentID == s.StudentID);
            if (student != null)
            {
                if (student.AverageScore < 2.0)
                {
                    sinhvienDB.Student.Remove(student);
                }
                else
                {
                    student.FullName = s.FullName;
                    student.FacultyID = s.FacultyID;
                    student.MajorID = s.MajorID;
                    student.AverageScore = s.AverageScore;
                    student.Avatar = s.Avatar;
                }
                sinhvienDB.SaveChanges();
            }
        }
    }
}
