namespace QuizWiz
{
    using System;
    using System.Configuration;
    using System.Data.SQLite;
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
                    Global.bytesSent = (long)res;
                }
            }

            this.timer = new Timer(OnTimerElapsed);
            this.timer.Change(TimeSpan.Zero, TimeSpan.FromMinutes(1));
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