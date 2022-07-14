--
-- PostgreSQL database dump
--

ROLLBACK;

BEGIN;

DROP SCHEMA IF EXISTS public CASCADE;
CREATE SCHEMA IF NOT EXISTS public;
SET search_path = public;

CREATE TABLE IF NOT EXISTS countries
(
    Id              serial      NOT NULL PRIMARY KEY,
    Name            text        NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS cities
(
    Id          serial      NOT NULL PRIMARY KEY,
    Name        text        NOT NULL UNIQUE,
    CountryId   integer     NOT NULL,
    FOREIGN KEY (CountryId) REFERENCES countries(Id)
);

CREATE TABLE IF NOT EXISTS factories
(
    Id              serial      NOT NULL PRIMARY KEY,
    Name            text        NOT NULL,
    CityId          integer     NOT NULL,
    Address         text        NOT NULL,
    FOREIGN KEY (CityId) REFERENCES cities(Id)
);

CREATE TABLE IF NOT EXISTS factoryDocs
(
    Id              serial      NOT NULL PRIMARY KEY,
    FactoryId       integer     NOT NULL,
    Document        bytea       NOT NULL,
    FOREIGN KEY (FactoryId) REFERENCES factories(Id)
);

CREATE TABLE IF NOT EXISTS materials
(
    Id              serial          NOT NULL PRIMARY KEY,
    Name            text            NOT NULL,
    FactoryId       integer         NOT NULL,
    Quantity        integer         DEFAULT 0,
    MaterialCost    decimal(10, 2)  NOT NULL,
    FOREIGN KEY (FactoryId) REFERENCES factories(Id)
);

CREATE TABLE IF NOT EXISTS clinics
(
    Id              serial         NOT NULL PRIMARY KEY,
    Name            text           NOT NULL,
    CityId          integer         NOT NULL,
    Address         text,
    Phone           varchar(12),
    FactoryId       integer        NOT NULL,
    FOREIGN KEY (CityId) REFERENCES cities(Id),
    FOREIGN KEY (FactoryId) REFERENCES factories(Id)
);

CREATE TABLE IF NOT EXISTS orderStatus
(
    Id          serial     NOT NULL PRIMARY KEY,
    Name        text       NOT NULL
);

CREATE TABLE IF NOT EXISTS glassesFrames
(
    Id          serial          NOT NULL PRIMARY KEY,
    Name        text            NOT NULL UNIQUE,
    Price       decimal(10, 2)  NOT NULL
);

CREATE TABLE IF NOT EXISTS roles
(
    Id          serial  NOT NULL PRIMARY KEY,
    Name        text    NOT NULL
);

CREATE TABLE IF NOT EXISTS users
(
    Id              serial  NOT NULL PRIMARY KEY,
    Email           text    NOT NULL,
    Password        text    NOT NULL,
    Name            text    NOT NULL,
    Surname         text,
    Patronymic      text,
    RoleId          integer NOT NULL DEFAULT 1,
    FOREIGN KEY (RoleId) REFERENCES roles(Id)
);

CREATE TABLE IF NOT EXISTS contactInfo
(
    Id          serial          NOT NULL PRIMARY KEY,
    UserId      integer         NOT NULL,
    Birthday    date,
    Address     text,
    Phone       varchar(12),
    FOREIGN KEY (UserId) REFERENCES users(Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS orders
(
    Id              serial          NOT NULL PRIMARY KEY,
    UserId          serial          NOT NULL,
    Date            timestamp       DEFAULT now() NOT NULL,
    GlassesFrameId  integer         NOT NULL,
    UserRecipe      text            NOT NULL,
    FullPrice       decimal(10, 2)  NOT NULL,
    OrderStatusId   integer         DEFAULT 1 NOT NULL,
    ClinicId        integer         NOT NULL,
    FOREIGN KEY (UserId) REFERENCES users(Id),
    FOREIGN KEY (GlassesFrameId) REFERENCES glassesFrames(Id),
    FOREIGN KEY (OrderStatusId) REFERENCES orderStatus(id),
    FOREIGN KEY (ClinicId) REFERENCES clinics(Id)
);

CREATE TABLE IF NOT EXISTS orderDocs
(
    Id              serial  NOT NULL PRIMARY KEY,
    OrderId         integer NOT NULL,
    Document        bytea   NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES orders(Id)
);



INSERT INTO countries
    (Name)
VALUES
    ('Россия'),
    ('Казахстан'),
    ('Китай');

INSERT INTO cities
    (Name, CountryId)
VALUES
    ('Москва', 1),
    ('Астана', 2),
    ('Пекин', 3);

INSERT INTO factories
    (Name, Address, CityId)
VALUES
    ('Фабрика линз №1 РФ', 'Ул. Пушкина 29', 1),
    ('Фабрика линз №2 РФ', 'Ул. Ленина 135', 1),
    ('Казахская фабрика имени Назарбаева', 'Ул. Назарбаева 1', 2),
    ('Chinese Factory 1', '364 Rex Terrace St.', 3),
    ('Chinese Factory 2', '910 Juliet Loaf St.', 3);

INSERT INTO factoryDocs
    (FactoryId, Document)
VALUES
    (1, 'ruFactory.txt'),
    (2, 'factoryRF.pdf'),
    (3, 'factoryKazakh.docx');

INSERT INTO materials
    (FactoryId, Name, Quantity, MaterialCost)
VALUES
    (1, 'Glasses', 68, 905.7),
    (1, 'Plastic', 132, 387.2),
    (2, 'Glasses', default, 905.7),
    (3, 'Glasses', 120, 905.7),
    (3, 'Plastic', 57, 387.2);

INSERT INTO clinics
    (CityId, FactoryId, Name, Address, Phone)
VALUES
    (1, 1, 'Очкарик', 'Ул. 1905 года, 51', '+76738240306'),
    (2, 3, 'Глазное яблоко', 'Проезд Гагарина, 44', '+72819670807'),
    (3, 4, 'Оптика', 'Ул. Тараз 109', '+77071127769');

INSERT INTO orderStatus
    (Name)
VALUES
    ('Заказ принят'),
    ('Подготовка материалов'),
    ('Изготовка линз'),
    ('Сборка очков'),
    ('Проверка качества'),
    ('Готово');

INSERT INTO glassesFrames
    (Name, Price)
VALUES
    ('Пластмассовая оправа', 2000),
    ('Алюминивая оправа', 5000),
    ('Деревянная оправа', 12000);

INSERT INTO roles
    (Name)
VALUES
    ('Пользователь'),
    ('Врач'),
    ('Тех. поддержка'),
    ('Модератор'),
    ('Администратор');

COMMIT;