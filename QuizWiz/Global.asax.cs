namespace QuizWiz
{
    using System;
    using System.Configuration;
    using System.Data.SQLite;
    using System.IO;
    using System.Threading;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    /// <summary>
    /// 
    /// </summary>
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly object lockObj = new object();
        private Timer timer;

        /// <summary>
        /// 
        /// </summary>
        public MvcApplication()
            : base()
        { //WARNING: might be called multiple times per App Domain. Application_Start called only once.
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        private void OnTimerElapsed(object sender)
        {
            try
            {
                var bytesSent = Interlocked.Read(ref Global.bytesSent);

                using (var conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "UPDATE Stats SET Value = " + bytesSent + " WHERE Name = 'BytesSent'";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void Application_Start()
        {
            this.SetupDatabase();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            using (var conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Value FROM Stats WHERE Name = 'BytesSent'";
                    var res = cmd.ExecuteScalar();
                    Global.bytesSent = res != null ? (long)res : 0L;
                }

                if (Global.bytesSent == 0) {
                    using(var cmd = conn.CreateCommand()){
                        cmd.CommandText = "INSERT INTO Stats (Name, Value) VALUES ('BytesSent', 0)";
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            this.timer = new Timer(OnTimerElapsed);
            this.timer.Change(TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        /// <summary>
        /// 
        /// </summary>
        protected void SetupDatabase()
        {
            var path = this.Server.MapPath("~/App_Data/quizwiz.db");

            if (!File.Exists(path))
            {
                SQLiteConnection.CreateFile(path);
                this.ExecuteScript(this.Server.MapPath("~/App_Data/identity.sql"));
                this.ExecuteScript(this.Server.MapPath("~/App_Data/schema.sql"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="script"></param>
        protected void ExecuteScript(string script)
        {
            using (var conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                var lines = File.ReadAllText(script).Split(';');

                foreach (var line in lines)
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = line;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class Global
    {
        public static long bytesSent;
    }
}