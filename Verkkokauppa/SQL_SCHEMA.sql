CREATE TABLE Products (
    id INTEGER PRIMARY KEY,
    name CHAR(32),
    description TEXT,
    price DOUBLE
);

CREATE TABLE Users (
    id INTEGER PRIMARY KEY,
    phonenumber CHAR(15),
    shippingaddress CHAR(32),
    postcode CHAR(10),
    city CHAR(32),
    state CHAR(32)
);

CREATE TABLE ProductsInShoppingCart (
    id INTEGER PRIMARY KEY,
    finished BOOLEAN,
    user_id INTEGER REFERENCES Users,
    product_id INTEGER REFERENCES Products
);

CREATE TABLE CreditCards (
    id INTEGER PRIMARY KEY,
    user_id INTEGER REFERENCES Users,
    number CHAR(19),
    billingaddress CHAR(32),
    securitynum CHAR(3),
    expirydate DATE
);

CREATE TABLE Orders (
    id INTEGER PRIMARY KEY,
    status CHAR(16),
    discountcode CHAR(12),
    shoppingcart_id INTEGER REFERENCES ProductsInShoppingCart,
    creditcard_id INTEGER REFERENCES CreditCards
);

CREATE TABLE ProductCategories (
    id INTEGER PRIMARY KEY,
    category_name char(16),
);

CREATE TABLE ProductsInCategory (
    category_id INTEGER REFERENCES ProductCategories,
    product_id INTEGER REFERENCES Products
)

CREATE TABLE Reviews (
    id INTEGER PRIMARY KEY,
    user_id INTEGER REFERENCES Users,
    product_id INTEGER REFERENCES Products,
    ratingstars TINYINT(5) UNSIGNED,
    ratingtext TEXT
);

CREATE TABLE Packages (
    id INTEGER PRIMARY KEY,
    price DOUBLE
);

CREATE TABLE PackageProducts (
    package_id INTEGER REFERENCES Packages,
    product_id INTEGER REFERENCES Products
);

CREATE TABLE Warehouses (
    id INTEGER PRIMARY KEY,
    address char(32),
    city char(32),
    phonenumber char(15)
);

CREATE TABLE ProductsInWarehouses (
    warehouse_id INTEGER REFERENCES Warehouses,
    product_id INTEGER REFERENCES Products
);

CREATE TABLE AlsoBought (
    product_a_id INTEGER REFERENCES Products,
    product_b_id INTEGER REFERENCES Products
);

