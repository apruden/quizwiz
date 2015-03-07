namespace QuizWiz.Controllers
{
    using QuizWiz.Models;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Net.Mail;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// 
    /// </summary>
    public class InvitationController : Controller
    {
        private IContextFactory factory;
        int[] alphabet = Enumerable.Range('a', 26).Concat(Enumerable.Range('A', 26)).Concat(Enumerable.Range(48, 10)).ToArray();

        /// <summary>
        /// 
        /// </summary>
        public InvitationController() : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        public InvitationController(IContextFactory factory) : this()
        {
            ContextFactory = factory;
        }

        /// <summary>
        /// 
        /// </summary>
        public IContextFactory ContextFactory
        {
            get { return factory ?? new ContextFactory();  }

            private set { factory = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exam"></param>
        /// <param name="recipient"></param>
        /// <returns></returns>
        private string GetInvitationLink(Exam exam, string recipient)
        {
            using(var db = ContextFactory.GetExamContext()) {
                var r = new Random();

                var found = db.Exams.Find(exam.ExamId);

                var invitation = (from i in db.Invitations.Include("Exam")
                                  where i.Exam.ExamId == found.ExamId && i.UserId == recipient
                                  select i).SingleOrDefault() ?? db.Invitations.Add(new Invitation
                                  {
                                      InvitationId = GenerateRandomCode(r),
                                      UserId = recipient,
                                      Exam = found,
                                      Sent = DateTime.UtcNow
                                  });

                db.SaveChanges();

                string invitationLink = string.Format("{0}/account/acceptinvitation/{1}",
                    Request.Url.Scheme + "://" + Request.Url.Authority, HttpUtility.UrlEncode(invitation.InvitationId));

                return invitationLink;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private string GenerateRandomCode(Random r)
        {
            return string.Concat((from c in Enumerable.Range(1, 6)
                                  select Convert.ToChar(alphabet[r.Next(alphabet.Count())])).ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recipients"></param>
        /// <returns></returns>
        [Authorize(Roles = "editor")]
        public ActionResult Invite(string recipients, int examId, bool showOnly = false)
        {
            string body, subject;
            string[] recipientsList = (from r in recipients.Split(',')
                                       select r.Trim()).ToArray();

            using (var config = ContextFactory.GetConfigContext())
            {
                body = (from s in config.Settings
                        where s.Name == "InvitationBody"
                        select s.Value).SingleOrDefault() ?? "{0}";

                subject = (from s in config.Settings
                           where s.Name == "InvitationSubject"
                           select s.Value).SingleOrDefault() ?? "quiz invitation";
            }

            Exam exam;

            using (var exams = ContextFactory.GetExamContext())
            {
                exam = exams.Exams.Find(examId);
            }

            if (showOnly)
            {
                string recipient = recipientsList.FirstOrDefault();

                return recipient == null ? Json(new { }) : Json(new { link = this.GetInvitationLink(exam, recipient) });
            }

            using (var client = new SmtpClient())
            {
                foreach (string recipient in recipientsList)
                {
                    var mail = new MailMessage();
                    mail.From = new MailAddress("no-reply@quizwiz.com", ConfigurationManager.AppSettings["InvitationDisplayName"]);
                    mail.To.Add(new MailAddress(recipient));
                    mail.Subject = string.Format(subject, exam.Name);
                    mail.Body = string.Format(body, this.GetInvitationLink(exam, recipient));
                    mail.IsBodyHtml = true;

                    client.Send(mail);
                }
            }

            return Json(new { });
        }
    }
}