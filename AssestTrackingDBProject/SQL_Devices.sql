CREATE DATABASE DevicesDB;

USE DevicesDB;

CREATE TABLE Devices (
    ID int NOT NULL PRIMARY KEY,
    Name varchar(255) NOT NULL,
    Brand varchar(255) NOT NULL,
    Model varchar(255) NOT NULL,
    Office varchar(255) NOT NULL,
    Purchase varchar(255) NOT NULL,
    Price varchar(255) NOT NULL,
);

INSERT INTO [Devices] (id,Name,Brand,Model,Office,Purchase,price) VALUES (2,'mobil','mb','m','o','05/05/2022','555');

SELECT * from Devices;

SELECT * FROM Devices order by Office,CASE When name='computer' then 1 when name ='mobil' then 2 else 3 END, convert(datetime, Purchase, 101)
SELECT * FROM Devices order by CASE When name='computer' then 1 when name ='mobil' then 2 else 3 END;

DELETE  from devices where id=0;



select c.name from sys.columns c
inner join sys.tables t 
on t.object_id = c.object_id
and t.name = 'Devices' and t.type = 'U'

select c.name from sys.columns inner join sys.tables t on t.object_id = c.object_id and t.name = 'Devices' and t.type = 'U'

SELECT * FROM Devices order by ID
SELECT *, Lower(Office) FROM Devices order by lower(Office),Purchase;