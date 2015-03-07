namespace QuizWiz
{
    using Owin;
    using System;
    using System.Configuration;
    using System.Data.SQLite;
    using System.IO;
    using System.Web;

    /// <summary>
    /// 
    /// </summary>
    public partial class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
