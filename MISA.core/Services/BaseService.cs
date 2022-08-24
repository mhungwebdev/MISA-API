using MISA.core.Interfaces.RepositoryInterface;
using MISA.core.Interfaces.ServiceInterface;
using MISA.core.MISAAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MISA.core.Exceptions;

namespace MISA.core.Services
{
    public class BaseService<MISAEntity> : IBaseService<MISAEntity>
    {
        #region contructor
        IBaseRepository<MISAEntity> _baseRepository;

        public BaseService(IBaseRepository<MISAEntity> baseRepository)
        {
            _baseRepository = baseRepository;
        }
        #endregion

        #region Insert Service
        /// <summary>
        /// Thêm mới bản ghi có thực hiện nghiệp vụ
        /// Author : mhungwebdev (28/7/2022)
        /// </summary>
        /// <param name="entity">đối tượng muốn thêm mới</param>
        /// <returns>1 nếu thành công, 0 nếu thất bại</returns>
        public int Insert(MISAEntity entity)
        {
            var props = typeof(MISAEntity).GetProperties();
            Guid id = Guid.NewGuid();
            foreach (var prop in props)
            {
                if (Attribute.IsDefined(prop, typeof(PrimaryKey)))
                {
                    prop.SetValue(entity, id);
                }

            }

            Tuple<string, string, List<string>> error = ValidateGeneral(entity,id);

            if (!string.IsNullOrEmpty(error.Item1))
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                result.Add("devMsg", "");
                if (!string.IsNullOrEmpty(error.Item2))
                    result.Add("userMsg", error.Item2);
                else
                    result.Add("userMsg", Resources.Resources.ExceptionMessage);

                var errorMsg = new
                {
                    typeError = error.Item1,
                    errors = error.Item3
                };
                result.Add("errorMsg", errorMsg);

                throw new MISAException(result);
            }

            var res = _baseRepository.Insert(entity);

            return res;
        }
        #endregion

        #region Update Service
        /// <summary>
        /// Sửa một bản ghi có thực hiện nghiệp vụ
        /// Author : mhungwebdev (28/7/2022)
        /// </summary>
        /// <param name="entity">Đối tượng muốn update</param>
        /// <returns>1 nếu thành công, 0 nếu thất bại</returns>
        public int Update(MISAEntity entity, Guid id)
        {

            Tuple<string, string, List<string>> error = ValidateGeneral(entity, id);

            if (!string.IsNullOrEmpty(error.Item1))
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                result.Add("devMsg", "");
                if (!string.IsNullOrEmpty(error.Item2))
                    result.Add("userMsg", error.Item2);
                else
                    result.Add("userMsg", Resources.Resources.ExceptionMessage);

                var errorMsg = new
                {
                    typeError = error.Item1,
                    errors = error.Item3
                };
                result.Add("errorMsg", errorMsg);

                throw new MISAException(result);
            }

            var res = _baseRepository.Update(entity, id);

            return res;
        }
        #endregion

        #region Validate General
        /// <summary>
        /// Kiểm tra validate chung (require, unique, email)
        /// Author : mhungwebdev (28/07/2022)
        /// </summary>
        /// <param name="entity">Bản ghi cần kiểm tra</param>
        /// <param name="id">Khóa chính của bản ghi</param>
        /// <returns>một tuple, Item 1 là typeError, Item 2 là userMsg, Item 3 là list error</returns>
        protected Tuple<string,string,List<string>> ValidateGeneral(MISAEntity entity, Guid id)
        {
            List<string> errors = new List<string>();
            string userMsg = string.Empty;
            string typeError = string.Empty;

            //kiểm tra field require
            var propRequires = typeof(MISAEntity).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(Require)));
            foreach(var propRequire in propRequires)
            {
                var value = propRequire.GetValue(entity);

                if (value == null || String.IsNullOrEmpty(value.ToString()))
                {
                    string nameDisplay = propRequire.Name;

                    var propNames = propRequire.GetCustomAttributes(typeof(PropertyName), true);

                    if(propNames.Length > 0)
                    {
                        nameDisplay = (propNames[0] as PropertyName).Name;
                    }

                    errors.Add(string.Format(Resources.Resources.EmptyFieldRequireMsg,nameDisplay));
                }
            }

            //kiểm tra field unique
            var propUniques = typeof(MISAEntity).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(Unique)));
            foreach(var propUnique in propUniques)
            {
                var value = propUnique.GetValue(entity);
                if (value != null || String.IsNullOrEmpty(value.ToString())){
                    if (_baseRepository.CheckUnique(value, propUnique.Name, id)){
                        string nameDisplay = propUnique.Name;

                        var propNames = propUnique.GetCustomAttributes(typeof(PropertyName), true);
                        
                        if (propNames.Length > 0)
                            nameDisplay = (propNames[0] as PropertyName).Name;

                        errors.Add(string.Format(Resources.Resources.UniqueErrorMsg, nameDisplay));
                    }
                }
            }

            //kiểm tra field email
            var propEmails = typeof(MISAEntity).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(Email)));
            foreach(var propEmail in propEmails)
            {
                var value = propEmail.GetValue(entity);

                if(!(String.IsNullOrEmpty(value.ToString())))
                {
                    Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                    Match match = regex.Match(value.ToString());

                    if (!(match.Success))
                    {
                        var nameDisplay = propEmail.Name;

                        var propNames = propEmail.GetCustomAttributes(typeof(PropertyName), true);
                        if (propNames.Length > 0)
                            nameDisplay = (propNames[0] as PropertyName).Name;

                        errors.Add(String.Format(Resources.Resources.EmailErrorMsg,nameDisplay));
                    }
                }
            }

            //kiểm tra độ dài
            var propLengths = typeof(MISAEntity).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(Length)));
            foreach(var propLength in propLengths)
            {
                var name = propLength.Name;
                var value = propLength.GetValue(entity);
                var lengths = propLength.GetCustomAttributes(typeof(Length), true);
                int fieldLength = 255;

                if(lengths.Length > 0)
                {
                    fieldLength = (lengths[0] as Length).length;
                }

                if(value != null && !string.IsNullOrEmpty(value.ToString()) && value.ToString().Length > fieldLength)
                {
                    var propNames = propLength.GetCustomAttributes(typeof(PropertyName), true);
                    string nameDisplay = propLength.Name;
                    if(propNames.Length > 0)
                    {
                        nameDisplay = (propNames[0] as PropertyName).Name;
                    }

                    errors.Add(String.Format(Resources.Resources.TooLongValid,nameDisplay,fieldLength));
                }
            }

            if (errors.Count > 0)
                typeError = Resources.Resources.VALID_INPUT;

            //kiểm tra custome
            Tuple<string, string, List<string>> errorCustomers = ValidateCustome(entity,id);

            if (!string.IsNullOrEmpty(errorCustomers.Item1))
                typeError = errorCustomers.Item1;

            if (!string.IsNullOrEmpty(errorCustomers.Item2))
                userMsg = errorCustomers.Item2;

            errors = errors.Concat(errorCustomers.Item3).ToList();

            Tuple<string, string, List<string>> error = new Tuple<string, string, List<string>>(typeError,userMsg,errors);

            return error;
        }
        #endregion

        #region Validate Cumtome
        /// <summary>
        /// Kiểm tra validate custome cho từng entity
        /// Author : mhungwebdev (28/7/2022)
        /// </summary>
        /// <param name="entity">đối tượng validate</param>
        /// <param name="id">id của đối tượng validate</param>
        /// <returns>một tuple, Item 1 là typeError, Item 2 là userMsg, Item 3 là list error</returns>
        public virtual Tuple<string, string, List<string>> ValidateCustome(MISAEntity entity, Guid id)
        {
            return new Tuple<string, string, List<string>>(null, null, new List<string>());
        }
        #endregion
    }
}
