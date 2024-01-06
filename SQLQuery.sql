CREATE TABLE last_names
(
    id INT NOT NULL IDENTITY(1, 1) PRIMARY KEY,
    last_name NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE first_names
(
    id INT NOT NULL IDENTITY(1, 1) PRIMARY KEY,
    first_name NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE middle_names
(
    id INT NOT NULL IDENTITY(1, 1) PRIMARY KEY,
    middle_name NVARCHAR(50) NOT NULL UNIQUE
);


CREATE TABLE authors
(
    id INT NOT NULL IDENTITY(1, 1) PRIMARY KEY,
    last_name_id INT REFERENCES last_names (id),
    first_name_id INT REFERENCES first_names (id) NOT NULL,
    middle_name_id INT REFERENCES middle_names (id)
);

CREATE TABLE publishers
(
    id INT NOT NULL IDENTITY(1, 1) PRIMARY KEY,
    publisher_name NVARCHAR(50) NOT NULL UNIQUE,
    publisher_address NVARCHAR(50) NOT NULL
)

CREATE TABLE genres
(
    id INT NOT NULL IDENTITY(1, 1) PRIMARY KEY,
    genre_name NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE books
(
    id INT NOT NULL IDENTITY(1, 1) PRIMARY KEY,

    book_name NVARCHAR(50) NOT NULL UNIQUE,
    author_id INT REFERENCES authors (id) NOT NULL,
    publisher_id INT REFERENCES publishers (id) NOT NULL,
    genre_id INT REFERENCES genres (id) NOT NULL,

    quantity INT NOT NULL CHECK (quantity >= 0),

    price INT NOT NULL CHECK (price > 0),
    cost INT NOT NULL CHECK (cost > 0),

    discount_percent INT NOT NULL DEFAULT(0) CHECK (discount_percent >= 0 AND discount_percent <= 100),

    image VARBINARY(MAX)
)

CREATE TABLE archive_books
(
    id INT NOT NULL IDENTITY(1, 1) PRIMARY KEY,

    book_name NVARCHAR(50) NOT NULL UNIQUE,
    author_id INT REFERENCES authors (id) NOT NULL,
    publisher_id INT REFERENCES publishers (id) NOT NULL,
    genre_id INT REFERENCES genres (id) NOT NULL,

    quantity INT NOT NULL CHECK (quantity >= 0),

    price INT NOT NULL CHECK (price > 0),
    cost INT NOT NULL CHECK (cost > 0),

    discount_percent INT NOT NULL DEFAULT(0) CHECK (discount_percent >= 0 AND discount_percent <= 100),

    image VARBINARY(MAX)
);

CREATE TABLE users
(
	id INT NOT NULL IDENTITY(1, 1) PRIMARY KEY,

	login NVARCHAR(50) NOT NULL UNIQUE,
	password NVARCHAR(50) NOT NULL,

    last_name_id INT REFERENCES last_names (id),
    first_name_id INT REFERENCES first_names (id) NOT NULL,
    middle_name_id INT REFERENCES middle_names (id),

    is_admin BIT NOT NULL
)


-- Заполнение таблицы last_names
INSERT INTO last_names (last_name)
VALUES ('Smith'), ('Johnson'), ('Williams'), ('Jones'), ('Brown'), ('Davis'), ('Miller'), ('Wilson'), ('Moore'), ('Taylor'), ('Anderson'), ('Thomas'), ('Jackson'), ('White'), ('Harris'), ('Martin'), ('Thompson'), ('Garcia'), ('Martinez'), ('Robinson');

-- Заполнение таблицы first_names
INSERT INTO first_names (first_name)
VALUES ('James'), ('John'), ('Robert'), ('Michael'), ('William'), ('David'), ('Joseph'), ('Thomas'), ('Charles'), ('Daniel'), ('Jessica'), ('Sarah'), ('Jennifer'), ('Emily'), ('Samantha'), ('Sophia'), ('Olivia'), ('Mia'), ('Charlotte'), ('Amelia');

-- Заполнение таблицы middle_names
INSERT INTO middle_names (middle_name)
VALUES ('Lee'), ('Allen'), ('Edward'), ('Scott'), ('Brian'), ('Christopher'), ('Paul'), ('Mark'), ('Andrew'), ('Steven'), ('Marie'), ('Elizabeth'), ('Nicole'), ('Michelle'), ('Ashley'), ('Alexis'), ('Grace'), ('Ava'), ('Ella'), ('Scarlett');

-- Заполнение таблицы publishers
INSERT INTO publishers (publisher_name, publisher_address)
VALUES ('Penguin Random House', '123 Main Street, New York, USA'), ('HarperCollins', '456 Elm Street, London, UK'), ('Simon & Schuster', '789 Oak Street, Los Angeles, USA'), ('Macmillan Publishers', '321 Maple Street, Sydney, Australia'), ('Hachette Livre', '987 Pine Street, Paris, France');

-- Заполнение таблицы genres
INSERT INTO genres (genre_name)
VALUES ('Science Fiction'), ('Mystery'), ('Romance'), ('Non-fiction'), ('Poetry');

-- Заполнение таблицы authors
INSERT INTO authors (last_name_id, first_name_id, middle_name_id)
VALUES (1, 1, 1), (2, 2, 2), (3, 3, 3), (4, 4, 4), (5, 5, 5), (6, 6, 6), (7, 7, 7), (8, 8, 8), (9, 9, 9), (10, 10, 10), (11, 11, 11), (12, 12, 12), (13, 13, 13), (14, 14, 14), (15, 15, 15), (16, 16, 16), (17, 17, 17), (18, 18, 18);

-- Заполнение таблицы books
INSERT INTO books (book_name, author_id, publisher_id, genre_id, quantity, price, cost, discount_percent)
VALUES 
    ('The Great Gatsby', 1, 1, 1, 10, 100, 50, 0), 
    ('To Kill a Mockingbird', 2, 2, 2, 5, 150, 75, 10), 
    ('Pride and Prejudice', 3, 3, 3, 7, 200, 100, 5),
    ('1984', 4, 4, 4, 3, 50, 25, 0), 
    ('Brave New World', 5, 5, 5, 8, 120, 60, 15),
    ('The Catcher in the Rye', 6, 1, 1, 6, 80, 40, 0),
    ('The Lord of the Rings', 7, 2, 2, 12, 180, 90, 5),
    ('Harry Potter and the Sorcerers Stone', 8, 3, 3, 15, 200, 100, 10),
    ('The Chronicles of Narnia', 9, 4, 4, 9, 90, 45, 0),
    ('To Kill a Mockingbird', 10, 5, 5, 7, 100, 50, 5),
    ('The Hobbit', 11, 1, 1, 11, 150, 75, 0),
    ('The Da Vinci Code', 12, 2, 2, 4, 120, 60, 0),
    ('Gone Girl', 13, 3, 3, 8, 80, 40, 5),
    ('The Girl on the Train', 14, 4, 4, 6, 70, 35, 10),
    ('The Alchemist', 15, 5, 5, 10, 100, 50, 0),
    ('The Fault in Our Stars', 16, 1, 1, 5, 90, 45, 5),
    ('The Hunger Games', 17, 2, 2, 9, 110, 55, 0),
    ('The Kite Runner', 18, 3, 3, 7, 100, 50, 0)

DELETE FROM books;
DELETE FROM archive_books;
DELETE FROM users;

DELETE FROM authors;
DELETE FROM publishers;
DELETE FROM genres;

DELETE FROM last_names;
DELETE FROM first_names;
DELETE FROM middle_names;


SELECT * FROM books;
SELECT * FROM archive_books;

SELECT * FROM last_names;
SELECT * FROM users;

-- ПРИКОЛЬНО БЫЛО БЫ ДОБАВИТЬ БД С ДАТОЙ И ВРЕМЕНЕМ ВХОДА