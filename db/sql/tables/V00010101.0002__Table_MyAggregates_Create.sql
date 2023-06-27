USE MyDomainSchema;

CREATE TABLE IF NOT EXISTS MyAggregates (
    Id CHAR(36) NOT NULL UNIQUE,
    Version INT NOT NULL DEFAULT 1,
    Name NVARCHAR(100),
    Description NVARCHAR(255),
    CreatedOn DATETIME NOT NULL,
    UpdatedOn DATETIME NOT NULL
);