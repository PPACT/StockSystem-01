CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(20) NOT NULL,
    Password NVARCHAR(20) NOT NULL
);

INSERT INTO Users (Username, Password)
VALUES ('admin', '123456');
