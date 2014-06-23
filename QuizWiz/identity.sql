CREATE TABLE [AspNetUsers]
(
    [Id]               text primary key,
    [UserName]             text           NOT NULL,
    [Email]                text          NULL,
    [EmailConfirmed]       integer           NOT NULL,
    [PasswordHash]         text         NULL,
    [SecurityStamp]        text         NULL,
    [PhoneNumber]          text          NULL,
    [PhoneNumberConfirmed] integer           NOT NULL,
    [TwoFactorEnabled]     integer           NOT NULL,
    [LockoutEndDateUtc]    text               NULL,
    [LockoutEnabled]       integer  NOT NULL,
    [AccessFailedCount]    integer  NOT NULL
);
CREATE TABLE [AspNetUserClaims]
(
    [UserID] text,
    [Id] integer primary key autoincrement,
    [ClaimType]  text        NULL,
    [ClaimValue] text        NULL
);
CREATE TABLE [AspNetUserLogins]
(
    [UserID] text,
    [LoginProvider] text NOT NULL,
    [ProviderKey]   text NOT NULL
);
CREATE TABLE [AspNetRoles]
(
    [Id] text          NOT NULL,
    [Name]   text NOT NULL
);
CREATE TABLE [AspNetUserRoles]
(
    [UserId] text NOT NULL,
    [RoleId] text NOT NULL
);

