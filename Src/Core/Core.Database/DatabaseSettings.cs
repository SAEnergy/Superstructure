using System.Data.Entity;

namespace Core.Database
{
    public class DatabaseSettings
    {
        #region Fields

        private static object syncObject = new object();
        private static DatabaseSettings _instance;

        #endregion

        #region Properties

        public static DatabaseSettings Instance
        {
            get
            {
                if(_instance == null)
                {
                    lock(syncObject)
                    {
                        if(_instance == null)
                        {
                            _instance = new DatabaseSettings();
                        }
                    }
                }

                return _instance;
            }
        }

        public string ConnectionString { get; set; }

        #endregion

        #region Constructor

        private DatabaseSettings()
        {

        }

        #endregion
    }
}
