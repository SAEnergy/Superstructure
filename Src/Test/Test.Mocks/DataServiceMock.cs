using Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Mocks
{
    public class DataServiceMock : IDataService
    {
        public bool Delete<T>(Func<T, bool> where) where T : class
        {
            throw new NotImplementedException();
        }

        public bool Delete<T>(T obj) where T : class
        {
            throw new NotImplementedException();
        }

        public T Find<T>(int key) where T : class
        {
            throw new NotImplementedException();
        }

        public List<T> Find<T>(Func<T, bool> where) where T : class
        {
            throw new NotImplementedException();
        }

        public bool Insert<T>(T obj) where T : class
        {
            throw new NotImplementedException();
        }

        public bool Update<T>(int key, T obj) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
