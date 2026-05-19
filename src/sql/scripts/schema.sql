CREATE TABLE [customers] (
    [CustomerId] INT IDENTITY(1,1) NOT NULL,
    [FirstName] NVARCHAR(100) NOT NULL,
    [LastName] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(256) NOT NULL,
    [PhoneNumber] NVARCHAR(32) NULL,
    [CreatedAtUtc] DATETIME2 NOT NULL,
    CONSTRAINT [PK_customers] PRIMARY KEY ([CustomerId])
);
GO

CREATE TABLE [movies] (
    [MovieId] INT IDENTITY(1,1) NOT NULL,
    [LegacyMovieId] INT NOT NULL,
    [Title] NVARCHAR(200) NOT NULL,
    [Language] NVARCHAR(64) NULL,
    [AvailableTickets] INT NOT NULL,
    [UnitPrice] DECIMAL(10,2) NOT NULL,
    CONSTRAINT [PK_movies] PRIMARY KEY ([MovieId])
);
GO

CREATE TABLE [orders] (
    [OrderId] INT IDENTITY(1,1) NOT NULL,
    [OrderNumber] NVARCHAR(30) NOT NULL,
    [CustomerId] INT NOT NULL,
    [OrderedAtUtc] DATETIME2 NOT NULL,
    [Status] NVARCHAR(32) NOT NULL,
    [TotalAmount] DECIMAL(12,2) NOT NULL,
    CONSTRAINT [PK_orders] PRIMARY KEY ([OrderId]),
    CONSTRAINT [FK_orders_customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [customers]([CustomerId])
);
GO

CREATE TABLE [order_line_items] (
    [OrderLineItemId] INT IDENTITY(1,1) NOT NULL,
    [OrderId] INT NOT NULL,
    [MovieId] INT NOT NULL,
    [Quantity] INT NOT NULL,
    [UnitPrice] DECIMAL(10,2) NOT NULL,
    [LineTotal] DECIMAL(12,2) NOT NULL,
    CONSTRAINT [PK_order_line_items] PRIMARY KEY ([OrderLineItemId]),
    CONSTRAINT [FK_order_line_items_orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [orders]([OrderId]) ON DELETE CASCADE,
    CONSTRAINT [FK_order_line_items_movies_MovieId] FOREIGN KEY ([MovieId]) REFERENCES [movies]([MovieId])
);
GO

CREATE UNIQUE INDEX [IX_customers_Email] ON [customers]([Email]);
GO

CREATE UNIQUE INDEX [IX_movies_LegacyMovieId] ON [movies]([LegacyMovieId]);
GO

CREATE UNIQUE INDEX [IX_orders_OrderNumber] ON [orders]([OrderNumber]);
GO

CREATE INDEX [IX_orders_CustomerId] ON [orders]([CustomerId]);
GO

CREATE INDEX [IX_order_line_items_MovieId] ON [order_line_items]([MovieId]);
GO

CREATE UNIQUE INDEX [IX_order_line_items_OrderId_MovieId] ON [order_line_items]([OrderId], [MovieId]);
GO
