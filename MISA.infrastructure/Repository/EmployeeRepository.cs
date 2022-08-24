using Dapper;
using MISA.core.Entities;
using MISA.core.Interfaces.RepositoryInterface;
using MISA.core.MISAAttribute;
using MySqlConnector;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.infrastructure.Repository
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        #region Delete Multi
        /// <summary>
        /// Xóa nhiều bản ghi
        /// Author : mhungwebdev (10/8/2022)
        /// </summary>
        /// <param name="listId">danh sách id các đối tượng muốn xóa</param>
        /// <returns>số bản ghi bị xóa</returns>
        public int DeleteMulti(List<Guid> listId)
        {
            sqlConnection = new MySqlConnection(connectString);
            sqlConnection.Open();
            DynamicParameters parameters = new DynamicParameters();

            using (var transaction = sqlConnection.BeginTransaction())
            {
                var sqlCommand = $"Delete from Employee where EmployeeId in (";

                int i = 0;
                foreach (var id in listId)
                {
                    sqlCommand += $"@Id{i},";
                    parameters.Add($"@Id{i}", id.ToString());
                    i++;
                }

                sqlCommand = sqlCommand[..^1] + ")";
                var res = 0;

                try
                {
                    res = sqlConnection.Execute(sqlCommand, param: parameters, transaction: transaction);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }

                sqlConnection.Dispose();
                sqlConnection.Close();

                return res;

            }
        }
        #endregion

        #region Filter
        /// <summary>
        /// Paging và tìm kiếm
        /// Author : mhungwebdev (28/7/2022)
        /// </summary>
        /// <param name="pageSize">Số bản ghi trên một trang</param>
        /// <param name="pageNumber">Trang hiện tại</param>
        /// <param name="keyword">Từ khóa tìm kiếm</param>
        /// <returns>Các bản ghi phù hợp</returns>
        public object Filter(int pageSize, int pageNumber, string keyword)
        {
            using (sqlConnection = new MySqlConnection(connectString))
            {
                //lay total record
                var sqlTotalCommand = $"select EmployeeId from Employee";
                DynamicParameters parameters = new DynamicParameters();

                if (!(String.IsNullOrWhiteSpace(keyword)))
                {
                    var propSearchs = typeof(Employee).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(Search)));

                    if (propSearchs.Count() > 0)
                        sqlTotalCommand += " where ";

                    foreach (var propSearch in propSearchs)
                    {
                        var fieldName = propSearch.Name;

                        sqlTotalCommand += $"{fieldName} like @Keyword or ";
                    }

                    parameters.Add($"@Keyword", '%' + keyword + '%');
                    sqlTotalCommand = sqlTotalCommand.Substring(0, sqlTotalCommand.Length - 3);
                }

                int totalRecord = sqlConnection.Query<Employee>(sqlTotalCommand, param: parameters).Count();
                if (pageSize <= 0 || pageNumber <= 0)
                {
                    pageSize = totalRecord;
                    pageNumber = 1;
                }
                //tính total page
                int totalPage = (int)Math.Ceiling((double)totalRecord / pageSize);
                var sqlFilter = $"SELECT * FROM Employee " +
                    $"inner join Department on Employee.DepartmentId = Department.DepartmentId " +
                    $"WHERE EmployeeId IN ({sqlTotalCommand}) ORDER BY Employee.ModifyDate DESC " +
                    $"LIMIT @pageSize OFFSET @offset";

                parameters.Add("@pageSize", pageSize);
                parameters.Add("@offset", pageSize * (pageNumber - 1));
                //lay data
                var data = sqlConnection.Query<Employee>(sqlFilter, parameters);

                var res = new
                {
                    TotalPage = totalPage,
                    TotalRecord = totalRecord,
                    currentPage = pageNumber,
                    Data = data
                };
                //tra ve du lieu

                return res;
            }
        }
        #endregion

        #region GenerateNewEmployeeCode
        /// <summary>
        /// Sinh ra một mã nhân viên mới
        /// Author : mhungwebdev (28/7/2022)
        /// </summary>
        /// <returns>Mã nhân viên với</returns>
        public async Task<string> GenerateNewEmployeeCode()
        {
            using (sqlConnection = new MySqlConnection(connectString))
            {
                await sqlConnection.OpenAsync();
                string newEmployeeCode = String.Empty;

                var sqlCommand = "Select EmployeeCode from Employee where EmployeeCode like '%NV-%' order by EmployeeCode desc limit 1 offset 0";
                string employeeCodeBiggest = await sqlConnection.QuerySingleOrDefaultAsync<string>(sqlCommand);

                for (int i = employeeCodeBiggest.Length - 1; i > 0; i--)
                {
                    int n;
                    bool isNumeric = int.TryParse(employeeCodeBiggest[i].ToString(), out n);
                    if (isNumeric)
                    {
                        newEmployeeCode = employeeCodeBiggest[i].ToString() + newEmployeeCode;
                    }
                    else
                        break;
                }

                return "NV-" + (int.Parse(newEmployeeCode) + 1);
            }
        }
        #endregion

        #region Export Data to Excel
        /// <summary>
        /// Xuất khẩu dữ liệu
        /// Author : mhungwebdev (10/8/2022)
        /// </summary>
        /// <returns>mảng byte</returns>
        async Task<Excel> IEmployeeRepository.ExportDataToExcel()
        {
            var employees = Get();
            var stream = new MemoryStream();
            using(var package = new ExcelPackage(stream))
            {
                Excel excel = new Excel();
                var workSheet = package.Workbook.Worksheets.Add("Danh sách nhân viên");
                // create title
                workSheet.Cells["A1:O1"].Merge = true;
                workSheet.Cells["A1"].Value = "DANH SÁCH NHÂN VIÊN";
                workSheet.Cells["A1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                workSheet.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                workSheet.Cells["A1"].Style.Font.Bold = true;
                workSheet.Cells["A1"].Style.Font.Size = 20;
                // fill header
                List<string> listHeader = new List<string>()
            {
                "A2","B2","C2","D2","E2","F2","G2","H2","I2","J2","K2","L2","M2","N2","O2"
            };
                listHeader.ForEach(c =>
                {
                    workSheet.Cells[c].Style.Font.Bold = true;
                    workSheet.Cells[c].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    workSheet.Cells[c].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    workSheet.Cells[c].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    workSheet.Cells[c].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    Color color = System.Drawing.ColorTranslator.FromHtml("#c9c9c9");
                    workSheet.Cells[c].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    workSheet.Cells[c].Style.Fill.BackgroundColor.SetColor(color);

                    if (c == "A2" || c == "E2" || c == "I2")
                        workSheet.Cells[c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    else
                        workSheet.Cells[c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                });
                workSheet.Cells[listHeader[0]].Value = "STT";
                workSheet.Cells[listHeader[1]].Value = "Mã nhân viên";
                workSheet.Cells[listHeader[2]].Value = "Tên nhân viên";
                workSheet.Cells[listHeader[3]].Value = "Giới tính";
                workSheet.Cells[listHeader[4]].Value = "Ngày sinh";
                workSheet.Cells[listHeader[5]].Value = "Địa chỉ";
                workSheet.Cells[listHeader[6]].Value = "Số điện thoại";
                workSheet.Cells[listHeader[7]].Value = "Số CMND";
                workSheet.Cells[listHeader[8]].Value = "Ngày cấp";
                workSheet.Cells[listHeader[9]].Value = "Nơi cấp";
                workSheet.Cells[listHeader[10]].Value = "Chức danh";
                workSheet.Cells[listHeader[11]].Value = "Tên đơn vị";
                workSheet.Cells[listHeader[12]].Value = "Số tài khoản";
                workSheet.Cells[listHeader[13]].Value = "Tên ngân hàng";
                workSheet.Cells[listHeader[14]].Value = "Chi nhánh";

                // fill data
                int i = 0;
                foreach (Employee employee in employees)
                {
                    String dateOfBirth = employee.DateOfBirth == null ? "" : ((DateTime)employee.DateOfBirth).ToString("dd/MM/yyyy");
                    String identityDate = employee.IdentityDate == null ? "" : ((DateTime)employee.IdentityDate).ToString("dd/MM/yyyy");

                    workSheet.Cells[i + 3, 1].Value = (i + 1).ToString();
                    workSheet.Cells[i + 3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[i + 3, 2].Value = employee.EmployeeCode;
                    workSheet.Cells[i + 3, 3].Value = employee.FullName;
                    workSheet.Cells[i + 3, 4].Value = employee.GenderName;
                    workSheet.Cells[i + 3, 5].Value = dateOfBirth;
                    workSheet.Cells[i + 3, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[i + 3, 6].Value = employee.Address;
                    workSheet.Cells[i + 3, 7].Value = employee.PhoneNumber;
                    workSheet.Cells[i + 3, 8].Value = employee.IdentityNumber;
                    workSheet.Cells[i + 3, 9].Value = identityDate;
                    workSheet.Cells[i + 3, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[i + 3, 10].Value = employee.IdentityPlace;
                    workSheet.Cells[i + 3, 11].Value = employee.PositionName;
                    workSheet.Cells[i + 3, 12].Value = employee.DepartmentName;
                    workSheet.Cells[i + 3, 13].Value = employee.BankAccount;
                    workSheet.Cells[i + 3, 14].Value = employee.BankName;
                    workSheet.Cells[i + 3, 15].Value = employee.BankBranch;
                    i++;
                }

                // format column width
                i = 1;
                while (i < 16)
                {
                    //workSheet.Column(i).Width = 10;
                    workSheet.Column(i).AutoFit();

                    i++;
                }
                workSheet.Column(2).Width = 15;
                workSheet.Column(3).Width = 25;
                workSheet.Column(7).Width = 15;
                workSheet.Column(8).Width = 15;
                workSheet.Column(11).Width = 15;
                workSheet.Column(12).Width = 15;
                workSheet.Column(13).Width = 15;
                workSheet.Column(14).Width = 15;

                // format cell border
                i = 0;
                while (i < employees.Count())
                {
                    for (int j = 1; j < 16; j++)
                    {
                        workSheet.Cells[i + 3, j].Style.Font.Size = 10;
                        workSheet.Cells[i + 3, j].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        workSheet.Cells[i + 3, j].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        workSheet.Cells[i + 3, j].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        workSheet.Cells[i + 3, j].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }
                    i++;
                }

                package.Save();
                stream.Position = 0;
                string excelName = $"Danh_sach_nhan_vien.xlsx";
                excel.FileContents = stream.ToArray();
                excel.FileName = excelName;
                excel.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return excel;
            }
        }
        #endregion
    }
}
