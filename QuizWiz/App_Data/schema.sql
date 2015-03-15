CREATE TABLE Stats (Name TEXT NOT NULL PRIMARY KEY, Value INTEGER NULL);

CREATE TABLE Settings (
    Name TEXT PRIMARY KEY NOT NULL,
    Value TEXT NULL
);

CREATE TABLE Answer (
    AnswerId INTEGER PRIMARY KEY AUTOINCREMENT,
    Text TEXT NULL,
    Points INTEGER NOT NULL,
    Question_QuestionId INTEGER NULL
);

CREATE TABLE Exam (
    ExamId INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NULL,
	Description TEXT NULL,
    AllowRetries boolean NOT NULL,
	Private boolean NOT NULL,
	Duration INTEGER NOT NULL,
	UserId TEXT NULL
);

CREATE TABLE Question (
    QuestionId INTEGER PRIMARY KEY AUTOINCREMENT,
    Text TEXT NULL,
    OrderIndex INTEGER NOT NULL,
    IsOpenEnded boolean NOT NULL,
    Exam_ExamId INTEGER NULL
);

CREATE TABLE Response (
    ResponseId INTEGER PRIMARY KEY AUTOINCREMENT,
    Value TEXT NULL,
    Answer_AnswerId INTEGER NULL,
    Question_QuestionId INTEGER NULL,
    Submission_SubmissionId INTEGER NULL
);

CREATE TABLE Submission (
    SubmissionId INTEGER PRIMARY KEY AUTOINCREMENT,
    Exam_ExamId INTEGER NULL,
    UserId TEXT NULL,
	Elapsed INTEGER NOT NULL,
	Heartbeat TEXT NULL,
	Started TEXT NULL,
	Finished TEXT NULL
);

CREATE TABLE Invitation (
	InvitationId TEXT PRIMARY KEY,
	UserId TEXT NOT NULL,
	Sent TEXT NOT NULL,
	Accepted TEXT NULL,
	Exam_ExamId INTEGER NOT NULL
);

CREATE TABLE Error (
    ErrorId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    Application TEXT NOT NULL,
    Host TEXT NOT NULL,
    Type TEXT NOT NULL,
    Source TEXT NOT NULL,
    Message TEXT NOT NULL,
    User TEXT NOT NULL,
    StatusCode INTEGER NOT NULL,
    TimeUtc TEXT NOT NULL,
    AllXml TEXT NOT NULL
);