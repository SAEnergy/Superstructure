using Core.Interfaces.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Test.Mocks
{
    public class DataServiceMock : IDataService
    {
        #region Fields

        private const string _extension = ".xml";
        private const string _searchPattern = "*" + _extension;

        private Dictionary<Type, List<object>> _dataSource;
        private Dictionary<Type, List<object>> _dataSourceOriginal;

        private static int _keyCounter = 1;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an empty data source
        /// </summary>
        public DataServiceMock()
        {
            _dataSource = new Dictionary<Type, List<object>>();
            _dataSourceOriginal = new Dictionary<Type, List<object>>();
        }

        /// <summary>
        /// Prime DataServiceMock datasource with a folder of xml files, named after each table (e.g. Users.xml for Users)
        /// Not required to
        /// </summary>
        /// <param name="inputFolderPath"></param>
        public DataServiceMock(string inputFolderPath)
        {
            _dataSource = new Dictionary<Type, List<object>>();
            _dataSourceOriginal = new Dictionary<Type, List<object>>();

            PrimeData(inputFolderPath);
        }

        #endregion

        #region Public Methods

        public bool Delete<T>(Func<T, bool> where) where T : class
        {
            bool rc = false;

            var objs = FindOrCreate<T>();

            foreach(var obj in objs.Where(where))
            {
                rc = Delete(obj);

                if(!rc)
                {
                    break;
                }
            }

            return rc;
        }

        public bool Delete<T>(int key) where T : class
        {
            var objs = FindOrCreate<T>();

            T found = Find<T>(key);

            if (found != null)
            {
                objs.Remove(found);

                SetList(objs);
            }

            return found != null;
        }


        public bool Delete<T>(T obj) where T : class
        {
            var objs = FindOrCreate<T>();

            T found = Find<T>(GetKeyValue(obj));

            if (found != null)
            {
                objs.Remove(found);

                SetList(objs);
            }

            return found != null;
        }

        public T Find<T>(int key) where T : class
        {
            T result = default(T);

            var objs = FindOrCreate<T>();

            foreach(var obj in objs)
            {
                if(key == GetKeyValue(obj))
                {
                    result = obj;
                    break;
                }
            }

            return result;
        }

        public List<T> Find<T>(Func<T, bool> where) where T : class
        {
            var objs = FindOrCreate<T>();

            var retVal = objs.Where(where).ToList();

            return retVal.Count == 0 ? null : retVal;
        }

        public bool Insert<T>(T obj) where T : class
        {
            var objs = FindOrCreate<T>();

            SetKey(obj);

            objs.Add(obj);

            SetList(objs);

            return true;
        }

        public bool Update<T>(int key, T obj) where T : class
        {
            var objs = FindOrCreate<T>();

            var objFind = Find<T>(key);

            if (objFind != null)
            {
                objs.Remove(objFind);

                objs.Add(obj);

                SetList(objs);
            }

            return objFind != null;
        }

        /// <summary>
        /// Resets datasource back to what it was after the DataServiceMock was constructed, this includes primed data
        /// </summary>
        public void ResetDataSource()
        {
            _dataSource = new Dictionary<Type, List<object>>();

            if (_dataSourceOriginal.Count > 0)
            {
                foreach (var kvp in _dataSourceOriginal)
                {
                    var objs = new List<object>();

                    foreach (var obj in kvp.Value)
                    {
                        objs.Add(Clone(obj, kvp.Key));
                    }

                    _dataSource.Add(kvp.Key, objs);
                }
            }
        }

        public void SaveDataSource(string outputFolder)
        {
            Directory.CreateDirectory(outputFolder);

            foreach(var kvp in _dataSource)
            {
                SerializeToDisk(kvp.Key, kvp.Value, outputFolder);
            }
        }

        #endregion

        #region Private Methods

        private void PrimeData(string inputFolderPath)
        {
            if(Directory.Exists(inputFolderPath))
            {
                foreach (var file in Directory.GetFiles(inputFolderPath, _searchPattern, SearchOption.TopDirectoryOnly))
                {
                    var tuple = DeserializeFromDisk(file);

                    if(tuple != null)
                    {
                        if(tuple.Item1 != null && tuple.Item2 != null)
                        {
                            _dataSourceOriginal.Add(tuple.Item1, tuple.Item2);
                        }
                    }
                }

                ResetDataSource();
            }
            else
            {
                throw new DirectoryNotFoundException();
            }
        }

        public int GetKeyValue<T>(T obj)
        {
            var prop = GetKeyProperty<T>();
            return (int)prop.GetValue(obj);
        }

        public void SetKey<T>(T obj)
        {
            var prop = GetKeyProperty<T>();
            prop.SetValue(obj, _keyCounter++);
        }

        private PropertyInfo GetKeyProperty<T>()
        {
            Type type = typeof(T);

            var keyAtty = type.GetProperties().Where(p => p.CustomAttributes.Any(a => a.GetType() == typeof(KeyAttribute))).FirstOrDefault();

            if(keyAtty == null)
            {
                keyAtty = type.GetProperty(type.Name + "Id");
            }

            return keyAtty;
        }

        private void SerializeToDisk(Type key, List<object> value, string outputFolder)
        {
            string fileName = string.Format("{0}\\{1}{2}", outputFolder, key.FullName, _extension);

            var generic = typeof(List<>).MakeGenericType(key);
            var output = Activator.CreateInstance(generic);

            foreach (var obj in value)
            {
                ((IList)output).Add(Convert.ChangeType(obj, key));
            }

            DataContractSerializer s = new DataContractSerializer(generic);
            using (FileStream fs = File.Open(fileName, FileMode.Create))
            {
                s.WriteObject(fs, output);
            }
        }

        private Tuple<Type,List<object>> DeserializeFromDisk(string fileName)
        {
            Tuple<Type, List<object>> retVal = null;

            if (File.Exists(fileName))
            {
                var type = GetTypeFromFileName(fileName);

                if (type != null)
                {
                    var generic = typeof(List<>).MakeGenericType(type);

                    DataContractSerializer s = new DataContractSerializer(generic);
                    using (FileStream fs = File.Open(fileName, FileMode.Open))
                    {
                        var objs = s.ReadObject(fs);

                        var listObjs = new List<object>();

                        foreach(var obj in objs as IEnumerable)
                        {
                            listObjs.Add(obj);
                            _keyCounter++;
                        }

                        retVal = new Tuple<Type, List<object>>(type, listObjs);
                    }
                }
            }

            return retVal;
        }

        private Type GetTypeFromFileName(string fileName)
        {
            Type retVal = null;

            string fullName = Path.GetFileNameWithoutExtension(fileName);

            foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                retVal = assembly.GetType(fullName);

                if(retVal != null)
                {
                    break;
                }
            }

            return retVal;
        }

        private string GetNameFromType(Type type)
        {
            return string.Format("{0}{1}", type.FullName, _extension);
        }

        private List<T> FindOrCreate<T>()
        {
            Type type = typeof(T);
            List<object> objs;
            List<T> tList = new List<T>();

            if(!_dataSource.TryGetValue(type, out objs))
            {
                objs = new List<object>();

                _dataSource.Add(type, objs);
            }

            if(objs != null)
            {
                foreach(var obj in objs)
                {
                    tList.Add((T)obj);
                }
            }

            return tList;
        }

        private void SetList<T>(List<T> objs)
        {
            List<object> listObjs = null;
            if(!_dataSource.TryGetValue(typeof(T), out listObjs))
            {
                listObjs = new List<object>();
            }

            listObjs.Clear();

            foreach(var obj in objs)
            {
                listObjs.Add((object)obj);
            }
        }

        public object Clone(object source, Type type)
        {
            return Deserialize(Serialize(source), type);
        }

        public string Serialize(object obj)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (StreamReader reader = new StreamReader(memoryStream))
            {
                DataContractSerializer serializer = new DataContractSerializer(obj.GetType());
                serializer.WriteObject(memoryStream, obj);
                memoryStream.Position = 0;
                return reader.ReadToEnd();
            }
        }

        public object Deserialize(string xml, Type toType)
        {
            using (Stream stream = new MemoryStream())
            {
                byte[] data = Encoding.UTF8.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                DataContractSerializer deserializer = new DataContractSerializer(toType);
                return deserializer.ReadObject(stream);
            }
        }

        #endregion
    }
}
