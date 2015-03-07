namespace QuizWiz.Web
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Threading;
    using System.Web;

    /// <summary>
    /// 
    /// </summary>
    public class CounterModule : IHttpModule
    {
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += context_BeginRequest;
            context.EndRequest += context_EndRequest;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void context_EndRequest(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void context_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            application.Context.Response.Filter = new StreamCounter(application.Context.Response.Filter);

            if (Global.bytesSent > int.Parse(ConfigurationManager.AppSettings["BytesSentLimit"]))
            {
                application.Context.Response.Close();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class StreamCounter : Stream
    {
        private Stream original;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        public StreamCounter(Stream original)
            : base()
        {
            this.original = original;
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool CanRead
        {
            get { return true; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool CanSeek
        {
            get { return true; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool CanWrite
        {
            get { return true; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override long Length
        {
            get { return 0; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override long Position
        {
            get
            {
                try
                {
                    return this.original.Position;
                }
                catch
                {
                    return 0;
                }
            }

            set
            {
                try
                {
                    this.original.Position = value;
                }
                catch
                {
                    return;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public override long Seek(long offset, System.IO.SeekOrigin direction)
        {
            return this.original.Seek(offset, direction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        public override void SetLength(long length)
        {
            this.original.SetLength(length);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Close()
        {
            this.original.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Flush()
        {
            this.original.Flush();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.original.Read(buffer, offset, count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            Interlocked.Add(ref Global.bytesSent, count);
            this.original.Write(buffer, offset, count);
        }
    }
}