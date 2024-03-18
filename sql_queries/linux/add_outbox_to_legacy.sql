create table outbox (
    id int identity(1,1) primary key,
    type nvarchar(100) not null,
    content nvarchar(max) not null check (isjson(content) = 1),
)
