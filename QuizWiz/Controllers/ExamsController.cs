﻿namespace QuizWiz.Controllers
{
    using QuizWiz.Filters;
    using QuizWiz.Models;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// 
    /// </summary>
    [InitializeExam]
    [Authorize]
    public class ExamsController : Controller
    {
        private IContextFactory factory = new ContextFactory();

        /// <summary>
        /// 
        /// </summary>
        public ExamsController()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        public ExamsController(IContextFactory factory)
        {
            this.factory = factory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "editor")]
        public ActionResult Edit(long? id)
        {
            using (var db = this.factory.GetExamContext())
            {
                var exam = (from e in db.Exams.Include("Questions.Answers")
                        where id.HasValue ? e.ExamId == id.Value : false
                        select e).SingleOrDefault() ?? new Exam { Name = "", Questions = new List<Question>() };

                return this.View(exam);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exam"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "editor")]
        public ActionResult Edit(Exam exam)
        {
            using (var db = this.factory.GetExamContext())
            {
                var found = db.Exams.Find(exam.ExamId) ?? db.Exams.Add(exam);
                found.UserId = this.User.Identity.Name;
                found.AllowRetries = exam.AllowRetries;
                found.Description = exam.Description;
                found.Duration = exam.Duration;
                found.Name = exam.Name;
                found.Private = exam.Private;

                db.SaveChanges();

                return this.RedirectToAction("Edit", new { id = found.ExamId });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exam"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "editor")]
        public ActionResult Delete(int id)
        {
            using (var db = this.factory.GetExamContext())
            {
                var exam = db.Exams.Find(id);
                db.Exams.Remove(exam);
                db.SaveChanges();
            }

            return new EmptyResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ActionResult Take(long id, string slug)
        {
            using (var db = this.factory.GetExamContext())
            {
                var exam = (from e in db.Exams.Include("Questions.Answers")
                            where e.ExamId == id
                            select e).SingleOrDefault();

                if (exam == null || !exam.IsAvailable())
                {
                    return HttpNotFound("Exam not available.");
                }

                var submission = (from s in db.Submissions.Include("Exam")
                                      .Include("Responses.Question")
                                  where s.Exam.ExamId == id && s.UserId == this.User.Identity.Name
                                  select s).SingleOrDefault() ?? db.Submissions.Add(
                                  new Submission
                                  {
                                      Started = DateTime.UtcNow,
                                      Elapsed = new TimeSpan(0, 0, 0),
                                      Heartbeat = DateTime.UtcNow,
                                      Exam = exam,
                                      UserId = this.User.Identity.Name,
                                      Responses = new List<Response>()
                                  });
                
                if (submission.IsCompleted())
                {
                    return this.RedirectToAction("Finished", "Exams");
                }

                submission.Elapsed += TimeSpan.FromSeconds(60);
                var response = submission.Responses.LastOrDefault();
                var selectedQuestion = exam.Questions[0];

                if (response != null)
                {
                    selectedQuestion = (from q in exam.Questions
                                        where q.OrderIndex > response.Question.OrderIndex
                                        orderby q.OrderIndex
                                        select q).FirstOrDefault() ?? selectedQuestion;
                }

                db.SaveChanges();

                return this.View(new ExamSectionModel
                {
                    Submission = submission,
                    Question = selectedQuestion,
                    Name = exam.Name,
                    Exam = exam
                });
            }
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

            using (var config = this.factory.GetConfigContext())
            {
                body = (from s in config.Settings
                        where s.Name == "InvitationBody"
                        select s.Value).SingleOrDefault() ?? "{0}";

                subject = (from s in config.Settings
                           where s.Name == "InvitationSubject"
                           select s.Value).SingleOrDefault() ?? "quiz invitation";
            }

            Exam exam;

            using (var exams = this.factory.GetExamContext())
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exam"></param>
        /// <param name="recipient"></param>
        /// <returns></returns>
        private string GetInvitationLink(Exam exam, string recipient)
        {
            string code = string.Empty;

            using (HMACMD5 hmac = new HMACMD5(Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["HmacSecret"])))
            {
                code = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", exam.ExamId, recipient))));
            }

            string invitationLink = string.Format("{0}/account/acceptinvitation?examId={1}&email={2}&code={3}",
                Request.Url.Scheme + "://" + Request.Url.Authority, exam.ExamId, HttpUtility.UrlEncode(recipient), HttpUtility.UrlEncode(code));

            return invitationLink;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Finished()
        {
            return this.View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="submissionId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Finished(int submissionId)
        {
            using (var db = this.factory.GetExamContext())
            {
                var submission = db.Submissions.Find(submissionId);
                submission.Finished = DateTime.UtcNow;
                db.SaveChanges();
            }

            return RedirectToAction("Finished");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Status(int submissionId)
        {
            var emptyResponses = new List<Question>();

            using (ExamContext db = this.factory.GetExamContext())
            {
                var submission = (from s in db.Submissions.Include("Responses.Question").Include("Exam")
                                  where s.SubmissionId == submissionId
                                  select s).FirstOrDefault();

                emptyResponses = (from r in submission.Responses
                                  where r.Answer == null && string.IsNullOrWhiteSpace(r.Value)
                                  select r.Question).ToList();


                var unanswered = (from q in submission.Exam.Questions
                                  select q).Except(from r in submission.Responses select r.Question).ToList();

                emptyResponses.AddRange(unanswered.AsEnumerable());

                ViewBag.ExamId = submission.Exam.ExamId;
            }

            ViewBag.MissingQuestions = emptyResponses;
            ViewBag.SubmissionId = submissionId;

            return this.View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult SubmitResponse(int QuestionId, int? AnswerId, int SubmissionId, string value)
        {
            using (ExamContext db = this.factory.GetExamContext())
            {
                var submission = (from s in db.Submissions.Include("Responses.Question").Include("Exam")
                                  where s.SubmissionId == SubmissionId
                                  select s).FirstOrDefault();

                if (submission.IsCompleted())
                {
                    return new HttpStatusCodeResult(300);
                }

                var question = db.Questions.Find(QuestionId);
                var answer = AnswerId.HasValue ? db.Answers.Find(AnswerId) : null;
                var response = (from r in submission.Responses
                                where r.Question.QuestionId == question.QuestionId
                                select r).FirstOrDefault() ?? db.Responses.Add(
                                     new Response
                                    {
                                        Question = question,
                                    });
                response.Answer = answer;
                response.Value = value;
                submission.Responses.Add(response);
                db.SaveChanges();

                var exam = (from e in db.Exams.Include("Questions.Answers")
                            where e.ExamId == submission.Exam.ExamId
                            select e).FirstOrDefault();

                var nextQuestion = (from q in exam.Questions
                                    where q.OrderIndex > question.OrderIndex
                                    orderby q.OrderIndex
                                    select q).FirstOrDefault();

                return nextQuestion != null ?
                    this.Json(new { HasNext = true, OrderIndex = nextQuestion.OrderIndex, Total = exam.Questions.Count }) :
                    this.Json(new { HasNext = false });

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public ActionResult GetQuestion(int orderIndex, int submissionId)
        {
            var model = new ExamQuestionModel();

            using (ExamContext db = this.factory.GetExamContext())
            {
                var submission = (from s in db.Submissions.Include("Responses.Question").Include("Exam")
                                  where s.SubmissionId == submissionId
                                  select s).FirstOrDefault();

                if (submission.IsCompleted())
                {
                    return new HttpStatusCodeResult(300);
                }

                var exam = (from e in db.Exams.Include("Questions.Answers")
                            where e.ExamId == submission.Exam.ExamId
                            select e).SingleOrDefault();

                var question = (from q in exam.Questions
                                select q).OrderBy(x => x.OrderIndex).Skip(orderIndex).Take(1).SingleOrDefault();

                model.Response = (from r in submission.Responses
                                  where r.Question.QuestionId == question.QuestionId
                                  select r).FirstOrDefault();
                model.Question = question;
            }

            return this.Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Heartbeat(int submissionId)
        {
            using (var db = this.factory.GetExamContext())
            {
                var submission = db.Submissions.Find(submissionId);
                submission.Elapsed += TimeSpan.FromSeconds(5);
                submission.Heartbeat = DateTime.UtcNow;
                db.SaveChanges();
            }

            return new EmptyResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult ShowMe()
        {
            return this.View("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Me(int offset, int limit)
        {
            using (var db = this.factory.GetExamContext())
            {
                var exams = (from e in db.Exams
                         where e.UserId == this.User.Identity.Name
                         select e)
                         .OrderBy(a => a.ExamId)
                         .Skip(offset)
                         .Take(limit)
                         .ToList();

                return this.Json(exams, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Search(string q)
        {
            using (var db = this.factory.GetExamContext())
            {
                var exams = (from e in db.Exams
                             where !string.IsNullOrEmpty(q) ? e.Name.ToLower().Contains(q.ToLower()) : true
                             select e).Take(20).ToList();

                return this.Json(exams, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadTest(string name, HttpPostedFileBase submission)
        {
            if (submission != null)
            {
                if (submission.ContentLength > 1000000)
                {
                    ModelState.AddModelError("submission", "The size of the file should not exceed 10 KB");

                    return this.View();
                }

                var supportedTypes = new[] { "zip", "7z", "rar", "gz" };
                var fileExt = Path.GetExtension(submission.FileName).Substring(1);

                if (!supportedTypes.Contains(fileExt))
                {
                    ModelState.AddModelError("submission", "Invalid type. Only the following types (zip, 7z, rar, gz) are supported.");

                    return this.View();
                }

                submission.SaveAs(Path.Combine(ConfigurationManager.AppSettings["FilesRoot"],
                     string.Format("{0}_{1}_{2}", name, this.User.Identity.Name, submission.FileName)));
            }

            return this.RedirectToAction("Index", "Home");
        }
    }
}
