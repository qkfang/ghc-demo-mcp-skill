SET IDENTITY_INSERT [customers] ON;
INSERT INTO [customers] ([CustomerId], [FirstName], [LastName], [Email], [PhoneNumber], [CreatedAtUtc]) VALUES
    (1, 'Taylor', 'Nguyen', 'taylor.nguyen@example.com', '+1-555-0101', '2025-01-06T15:30:00'),
    (2, 'Jordan', 'Patel', 'jordan.patel@example.com', '+1-555-0102', '2025-01-08T09:00:00');
SET IDENTITY_INSERT [customers] OFF;
GO

SET IDENTITY_INSERT [movies] ON;
INSERT INTO [movies] ([MovieId], [LegacyMovieId], [Title], [Language], [AvailableTickets], [UnitPrice]) VALUES
    (1, 1, 'Interstellar', 'English', 120, 100.00),
    (2, 2, 'Spirited Away', 'Japanese', 75, 90.00),
    (3, 3, 'The Dark Knight', 'English', 40, 80.00);
SET IDENTITY_INSERT [movies] OFF;
GO

SET IDENTITY_INSERT [orders] ON;
INSERT INTO [orders] ([OrderId], [OrderNumber], [CustomerId], [OrderedAtUtc], [Status], [TotalAmount]) VALUES
    (1, 'ORD-20250110-0001', 1, '2025-01-10T17:00:00', 'Confirmed', 360.00),
    (2, 'ORD-20250111-0002', 1, '2025-01-11T18:15:00', 'Confirmed', 500.00),
    (3, 'ORD-20250112-0003', 2, '2025-01-12T19:45:00', 'Pending', 160.00);
SET IDENTITY_INSERT [orders] OFF;
GO

SET IDENTITY_INSERT [order_line_items] ON;
INSERT INTO [order_line_items] ([OrderLineItemId], [OrderId], [MovieId], [Quantity], [UnitPrice], [LineTotal]) VALUES
    (1, 1, 2, 4, 90.00, 360.00),
    (2, 2, 1, 5, 100.00, 500.00),
    (3, 3, 3, 2, 80.00, 160.00);
SET IDENTITY_INSERT [order_line_items] OFF;
GO
