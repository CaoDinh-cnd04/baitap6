using LAPBT06.BUS;
using LAPBT06.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace LAPBT06

{
    public partial class Form1 : Form
    {
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();

        public Form1()
        {
            InitializeComponent();
            this.dgvStudent.CellContentClick += new DataGridViewCellEventHandler(this.dgvStudent_CellContentClick);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                setGridViewStyle(dgvStudent);
                var listFaculties = facultyService.GetAll();
                var listStudents = studentService.GetAll();
                FillFacultyCombobox(listFaculties);
                BindGrid(listStudents);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillFacultyCombobox(List<Faculty> listFaculties)
        {   
            listFaculties.Insert(0, new Faculty());
            this.cmbFaculty.DataSource = listFaculties;
            this.cmbFaculty.DisplayMember = "FacultyName";
            this.cmbFaculty.ValueMember = "FacultyID";
        }

        private void BindGrid(List<Student> listStudents)
        {
            dgvStudent.Rows.Clear();
            foreach (var student in listStudents)
            {
                int index = dgvStudent.Rows.Add();
                dgvStudent.Rows[index].Cells[0].Value = student.StudentID;
                dgvStudent.Rows[index].Cells[1].Value = student.FullName;
                dgvStudent.Rows[index].Cells[2].Value = student.Faculty?.FacultyName ?? "N/A"; 
                dgvStudent.Rows[index].Cells[3].Value = student.AverageScore.ToString("F2"); 
                dgvStudent.Rows[index].Cells[4].Value = student.Major?.Name ?? "Chưa đăng ký"; 
            }
        }


        private void ShowAvatar(string ImageName)
        {
            if (string.IsNullOrEmpty(ImageName))
            {
                pictureBox1.Image = null;
            }
            else
            {
                string parentDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                string imagePath = Path.Combine(parentDirectory, "Images", ImageName);
                pictureBox1.Image = Image.FromFile(imagePath);
                pictureBox1.Refresh();
            }
        }

        public void setGridViewStyle(DataGridView dgview)
        {
            dgview.BorderStyle = BorderStyle.None;
            dgview.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            dgview.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgview.BackgroundColor = Color.White;
            dgview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void btnThemSua_Click_1(object sender, EventArgs e)
        {
            string studentID = txtMSSV.Text.Trim();
            if (studentID.Length != 10)
            {
                MessageBox.Show("Mã sinh viên phải có 10 ký tự. Vui lòng nhập lại.");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                MessageBox.Show("Họ tên không được để trống.");
                return;
            }
            if (cmbFaculty.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn khoa.");
                return;
            }
            if (!double.TryParse(txtDTB.Text, out double avgScore))
            {
                MessageBox.Show("Điểm trung bình không hợp lệ. Vui lòng nhập lại.");
                return;
            }

            var student = new Student
            {
                StudentID = studentID,
                FullName = txtHoTen.Text,
                FacultyID = (int)cmbFaculty.SelectedValue,
                AverageScore = avgScore,
                Avatar = string.Empty // Avatar can be updated later if needed
            };

            try
            {
                if (studentService.FindByID(studentID) != null)
                {
                    // Edit existing student
                    studentService.InsertUpdate(student);
                    MessageBox.Show("Cập nhật sinh viên thành công!");
                }
                else
                {
                    // Add new student
                    studentService.InsertUpdate(student);
                    MessageBox.Show("Thêm sinh viên thành công!");
                }
                BindGrid(studentService.GetAll());
                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra khi thêm hoặc cập nhật sinh viên: {ex.Message}");
            }
        }

        private void ResetForm()
        {
            txtMSSV.Clear();
            txtHoTen.Clear();
            txtDTB.Clear();
            cmbFaculty.SelectedIndex = 0;
            pictureBox1.Image = null;
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvStudent.CurrentRow != null)
            {
                string studentID = dgvStudent.CurrentRow.Cells[0].Value.ToString();
                var studentToDelete = new Student { StudentID = studentID };
                studentService.DeleteUpdate(studentToDelete);
                MessageBox.Show("Xóa sinh viên thành công!");
                BindGrid(studentService.GetAll());
            }
            else
            {
                MessageBox.Show("Vui lòng chọn sinh viên để xóa.");
            }
        }

        private void dgvStudent_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var selectedRow = dgvStudent.Rows[e.RowIndex];
                string studentID = selectedRow.Cells[0].Value.ToString();
                var student = studentService.FindByID(studentID);
                if (student != null)
                {
                    txtMSSV.Text = student.StudentID.ToString();
                    txtHoTen.Text = student.FullName;
                    cmbFaculty.SelectedValue = student.FacultyID;
                    txtDTB.Text = student.AverageScore.ToString();
                    ShowAvatar(student.Avatar);
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            var listStudents = new List<Student>();
            if (this.checkBox1.Checked)
            {
                // Get students who haven't registered for a major
                listStudents = studentService.GetAllHasNoMajor();
            }
            else
            {
                // Get all students
                listStudents = studentService.GetAll();
            }
            BindGrid(listStudents);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All Files|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;
                    // Update PictureBox to display the selected image
                    pictureBox1.Image = Image.FromFile(selectedFilePath);
                    // Optionally save the path or file name if you need to store it in the database
                    // For example:
                    // currentStudent.Avatar = Path.GetFileName(selectedFilePath);
                    // SaveChanges();
                }
            }
        }

        private void btnAddPicture_Click(object sender, EventArgs e)
        {

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All Files|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;

                    // Update PictureBox to display the selected image
                    pictureBox1.Image = Image.FromFile(selectedFilePath);

                    // Optionally save the path or file name if you need to store it in the database
                    // For example:
                    // currentStudent.Avatar = Path.GetFileName(selectedFilePath);
                    // SaveChanges();
                }
            }
        }
    }
}
