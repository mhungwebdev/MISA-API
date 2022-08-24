using Dapper;
using MISA.core.Interfaces.RepositoryInterface;
using MISA.core.MISAAttribute;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.infrastructure.Repository
{
    public class BaseRepository<MISAEntity> : IBaseRepository<MISAEntity>
    {
        #region Declaration
        protected string connectString = "Server = localhost; " +
                                    "Port=3306; " +
                                    "Database = MISA.W06.LMHUNG;" +
                                    "User Id= mahhu;" +
                                    "Password=LMHung001201019898@";

        protected MySqlConnection sqlConnection;
        #endregion

        #region Check exist
        /// <summary>
        /// Kiểm tra sự tồn tại của bản ghi
        /// Autor : mhungwebdev (28/7/2022)
        /// </summary>
        /// <param name="id">id đối tượng muốn check</param>
        /// <returns>true nếu tồn tại, false nếu chưa tồn tại</returns>
        public bool CheckExist(Guid id)
        {
            using(sqlConnection = new MySqlConnection(connectString))
            {
                var tableName = typeof(MISAEntity).Name;
                DynamicParameters parameters = new DynamicParameters();
                var sqlCommand = $"Select * from {tableName} where {tableName}Id = @{tableName}Id";

                parameters.Add($"@{tableName}Id", id);

                var res = sqlConnection.QueryFirstOrDefault<MISAEntity>(sqlCommand,parameters);

                return res != null;
            }
        }
        #endregion

        #region Check unique
        /// <summary>
        /// Kiểm tra trùng lặp field unique
        /// Author : mhungwebdev (28/7/2022)
        /// </summary>
        /// <param name="value">value của field muốn check</param>
        /// <param name="fieldName">Tên field muốn check</param>
        /// <param name="id">id của đối tượng muốn check</param>
        /// <returns>true nếu trùng và false với trường hợp ngược lại</returns>
        public bool CheckUnique(object value, string fieldName, Guid id)
        {
            using(var sqlConnection = new MySqlConnection(connectString))
            {
                var tableName = typeof(MISAEntity).Name;
                DynamicParameters parameters = new DynamicParameters();

                var sqlCommand = $"select * from {tableName} where {fieldName} = @{fieldName} AND {tableName}Id != @{tableName}Id";

                parameters.Add($"@{fieldName}",value.ToString());
                parameters.Add($"@{tableName}Id", id);

                var res = sqlConnection.QueryFirstOrDefault(sqlCommand,parameters);

                return res != null;
            }
        }
        #endregion

        #region Delete
        /// <summary>
        /// Xóa 1 bản ghi
        /// Author : mhungwebdev (28/7/2022)
        /// </summary>
        /// <param name="id">id của đối tượng muốn xóa</param>
        /// <returns>1 nếu thành công</returns>
        public int Delete(Guid id)
        {
            using(sqlConnection = new MySqlConnection(connectString))
            {
                var tableName = typeof(MISAEntity).Name;
                DynamicParameters parameters = new DynamicParameters();

                var sqlCommand = $"Delete from {tableName} where {tableName}Id = @{tableName}Id";
                parameters.Add($"@{tableName}Id", id);
                var res = sqlConnection.Execute(sqlCommand, parameters);

                return res;
            }
        }
        #endregion

        #region Get
        /// <summary>
        /// Lấy tất cả dữ liệu
        /// Author : mhungwebdev (28/7/2022)
        /// </summary>
        /// <returns>Trả về tất cả dữ liệu của bảng</returns>
        public IEnumerable<MISAEntity> Get()
        {
            using(sqlConnection = new MySqlConnection(connectString))
            {
                var tableName = typeof(MISAEntity).Name;
                var sqlCommand = $"select * from {tableName}";
                var propFKs = typeof(MISAEntity).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(ForeignKey)));

                if(propFKs.Count() > 0)
                {
                    foreach(var propFK in propFKs)
                    {
                        var fkName = propFK.Name;
                        var fkTable = fkName.Substring(0,fkName.Length - 2);
                        sqlCommand += $" inner join {fkTable} on {tableName}.{fkName} = {fkTable}.{fkName}";
                    }
                }
                sqlCommand += $" order by {tableName}.ModifyDate desc";


                var res = sqlConnection.Query<MISAEntity>(sqlCommand);

                return res;
            }
        }
        #endregion

        #region Get by id
        /// <summary>
        /// Lấy bản ghi theo id
        /// Author : mhungwebdev (28/7/2022)
        /// </summary>
        /// <param name="id">id đối tượng muốn xóa</param>
        /// <returns>Trả về đối tượng trùng id</returns>
        public MISAEntity Get(Guid id)
        {
            using(sqlConnection=new MySqlConnection(connectString))
            {
                var tableName = typeof(MISAEntity).Name;
                DynamicParameters parameters = new DynamicParameters();

                var sqlCommand = $"select * from {tableName} where {tableName}Id = @{tableName}Id";

                parameters.Add($"@{tableName}Id", id);
                var res = sqlConnection.QueryFirstOrDefault<MISAEntity>(sqlCommand,param: parameters);

                return res;
            }
        }
        #endregion

        #region Insert
        /// <summary>
        /// Thêm mới 1 bản ghi
        /// Author : mhungwebdev (28/7/2022)
        /// </summary>
        /// <param name="entity">Đối tượng muốn thêm mới</param>
        /// <returns>1 nếu thành công</returns>
        public int Insert(MISAEntity entity)
        {
            sqlConnection = new MySqlConnection(connectString);
            sqlConnection.Open();
            using(var transaction = sqlConnection.BeginTransaction())
            {
                var tableName = typeof(MISAEntity).Name;
                var props = typeof(MISAEntity).GetProperties().Where(prop => !Attribute.IsDefined(prop,typeof(NotMap)));

                DynamicParameters parameters = new DynamicParameters();
                string listField = string.Empty;
                string listValue = string.Empty;

                foreach(var prop in props)
                {
                    var name = prop.Name;
                    var value = prop.GetValue(entity);
                    listField += $"{name},";
                    listValue += $"@{name},";

                    parameters.Add($"@{name}", value);
                }

                listField = listField.Substring(0, listField.Length - 1);
                listValue = listValue.Substring(0, listValue.Length - 1);

                var sqlCommand = $"insert into {tableName} ({listField}) values ({listValue})";

                var res = sqlConnection.Execute(sqlCommand,param: parameters,transaction: transaction);

                transaction.Commit();

                sqlConnection.Dispose();
                sqlConnection.Close();
                return res;
            }
        }
        #endregion

        #region Update
        /// <summary>
        /// Sửa một bản ghi
        /// Author : mhungwebdev (28/7/2022)
        /// </summary>
        /// <param name="entity">Thông tin mới của đối tượng được sửa </param>
        /// <returns>1 nếu thành công</returns>
        public int Update(MISAEntity entity, Guid id)
        {
            sqlConnection = new MySqlConnection(connectString);
            sqlConnection.Open();
            using(var transaction = sqlConnection.BeginTransaction())
            {
                var tableName = typeof(MISAEntity).Name;
                var propUpdate = typeof(MISAEntity).GetProperties().Where(prop => !Attribute.IsDefined(prop,typeof(NotUpdate)));

                DynamicParameters parameters = new DynamicParameters();
                var sqlCommand = $"Update {tableName} set ";
                foreach(var prop in propUpdate)
                {
                    var name = prop.Name;
                    var value = prop.GetValue(entity);
                    sqlCommand += $"{name} = @{name},";
                    parameters.Add($"@{name}", value);
                }
                sqlCommand = sqlCommand.Substring(0, sqlCommand.Length - 1);
                sqlCommand += $" where {tableName}Id = @{tableName}Id";
                parameters.Add($"@{tableName}Id", id);

                var res = sqlConnection.Execute(sqlCommand,param: parameters,transaction: transaction);

                transaction.Commit();

                sqlConnection.Dispose();
                sqlConnection.Close();
                return res;
            }
        }
        #endregion
    }
}
