create table Settings (
    Name text primary key not null,
    Value text null
);
create table Answer (
    AnswerId integer primary key autoincrement,
    Text text null,
    Points integer not null,
    IsOpenEnded boolean not null,
    Question_QuestionId integer null,
    foreign key (Question_QuestionId) references Question(QuestionId)
);
create table Exam (
    ExamId integer primary key autoincrement,
    Name text null,
	Description text null,
    AllowRetries boolean not null,
	Private boolean not null,
	Duration integer not null,
	UserId text null
);
create table Question (
    QuestionId integer primary key autoincrement,
    Text text null,
    OrderIndex integer not null,
    Exam_ExamId integer null,
    foreign key (Exam_ExamId) references Exam(ExamId)
);
create table Response (
    ResponseId integer primary key autoincrement,
    Value text null,
    Answer_AnswerId integer null,
    Question_QuestionId integer null,
    Submission_SubmissionId integer null,
    foreign key (Answer_AnswerId) references Answer(AnswerId),
    foreign key (Question_QuestionId) references Question(QuestionId),
    foreign key (Submission_SubmissionId) references Submission(SubmissionId)
);
create table Submission (
    SubmissionId integer primary key autoincrement,
    Exam_ExamId integer null,
    UserId text null,
	Elapsed integer not null,
	Heartbeat text null,
	Started text null,
	Finished text null,
    foreign key (Exam_ExamId) references Exam(ExamId)
);