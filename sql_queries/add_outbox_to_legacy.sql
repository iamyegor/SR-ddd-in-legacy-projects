create table Outbox (
    Id int identity(1,1) primary key,
    Type nvarchar(100) not null,
    Content nvarchar(max) not null check (isjson(Content) = 1),
)
