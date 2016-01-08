using Core.Database;
using Core.Interfaces.Logging;
using Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Services
{
    public class DataService : IDataService
    {
        #region Fields

        private readonly ILogger _logger;

        #endregion

        #region Constructor

        public DataService(ILogger logger)
        {
            _logger = logger;
        }

        #endregion

        #region Public Methods

        public bool Delete<T>(Func<T, bool> where) where T : class
        {
            bool retVal = false;

            var list = Find<T>(where);

            foreach (T obj in list)
            {
                retVal = Delete<T>(obj); //not efficent

                if (!retVal)
                {
                    break;
                }
            }

            return retVal;
        }

        public bool Delete<T>(T obj) where T : class
        {
            bool retVal = false;

            if (ValidateObject(obj))
            {
                using (ServerContext db = new ServerContext())
                {
                    var set = db.Set<T>();
                    set.Attach(obj);
                    set.Remove(obj);

                    //indicates at least one object was removed, if you have cascading deletes it may be greater than 1.
                    retVal = db.SaveChanges() > 0;
                }
            }

            return retVal;
        }

        public T Find<T>(int key) where T : class
        {
            T result = null;

            using (ServerContext db = new ServerContext())
            {
                var set = db.Set<T>();
                result = set.Find(key);
            }

            return result;
        }

        public List<T> Find<T>(Func<T, bool> where) where T : class
        {
            List<T> results;

            using (ServerContext db = new ServerContext())
            {
                var set = db.Set<T>();
                results = set.Where(where).ToList();
            }

            return results;
        }

        public bool Insert<T>(T obj) where T : class
        {
            bool retVal = false;

            if (ValidateObject(obj))
            {
                using (ServerContext db = new ServerContext())
                {
                    var set = db.Set<T>();
                    set.Add(obj);
                    db.SaveChanges();
                }
            }

            return retVal;
        }

        public bool Update<T>(int key, T obj) where T : class
        {
            bool result = false;

            using (ServerContext db = new ServerContext())
            {
                T dbObj = null;

                var set = db.Set<T>();
                dbObj = set.Find(key);

                if (dbObj != null)
                {
                    dbObj = obj;
                    result = db.SaveChanges() > 0;
                }
            }

            return result;
        }

        #endregion

        #region Private Methods

        private bool ValidateObject(object obj)
        {
            bool retVal = true;

            if (obj == null)
            {
                retVal = false;
                _logger.Log(LogMessageSeverity.Error, "Null value detected sent to DataService...");
            }

            return retVal;
        }

        #endregion
    }
}
