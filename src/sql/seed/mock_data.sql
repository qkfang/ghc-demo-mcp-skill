SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.movie_table', N'U') IS NULL OR OBJECT_ID(N'dbo.order_table', N'U') IS NULL
BEGIN
    THROW 50001, 'Run EF Core migrations before applying mock_data.sql.', 1;
END;

IF NOT EXISTS (SELECT 1 FROM dbo.movie_table WHERE m_id IN (1,2,3))
BEGIN
    SET IDENTITY_INSERT dbo.movie_table ON;

    INSERT INTO dbo.movie_table (m_id, m_title, m_genre, ticket_price, m_available, show_time)
    VALUES
        (1, N'Interstellar', N'Sci-Fi', 100.00, 50, '2025-01-15T19:30:00Z'),
        (2, N'Inception', N'Thriller', 100.00, 38, '2025-01-15T21:00:00Z'),
        (3, N'The Dark Knight', N'Action', 120.00, 25, '2025-01-16T20:00:00Z');

    SET IDENTITY_INSERT dbo.movie_table OFF;
END;

IF NOT EXISTS (SELECT 1 FROM dbo.order_table WHERE o_id IN (1,2,3))
BEGIN
    SET IDENTITY_INSERT dbo.order_table ON;

    INSERT INTO dbo.order_table (o_id, m_id, no_tickets, price, ordered_at)
    VALUES
        (1, 1, 2, 200.00, '2025-01-10T10:15:00Z'),
        (2, 2, 1, 100.00, '2025-01-10T11:00:00Z'),
        (3, 3, 4, 480.00, '2025-01-10T12:30:00Z');

    SET IDENTITY_INSERT dbo.order_table OFF;
END;
