CREATE TABLE outbox (
    id INT IDENTITY(1,1) PRIMARY KEY,
    content NVARCHAR(MAX),
    type NVARCHAR(100)
);
