INSERT INTO Venues (name, location, capacity, image_url) 
VALUES
('Grand Hall', 'Downtown City, Main Street', 500, 'https://example.com/grandhall.jpg'),
('Concert Arena', 'Central Park, New York', 10000, 'https://example.com/concertarena.jpg'),
('Beachside Resort', 'Malibu, California', 200, 'https://example.com/beachresort.jpg');

-- Insert Sample Data into Events
INSERT INTO Events ([name], [description], [start_date], [end_date], venue_id)
VALUES
('Music Concert', 'A live music concert featuring popular bands.', '2025-06-10 18:00', '2025-06-10 22:00', 2),
('Tech Conference', 'A conference with industry leaders in technology.', '2025-07-20 09:00', '2025-07-20 18:00', 1),
('Beach Party', 'An outdoor beach party with music and drinks.', '2025-08-05 16:00', '2025-08-05 23:00', 3);

-- Insert Sample Data into Bookings
INSERT INTO Bookings (customer_name, customer_contact, event_id)
VALUES
('Alice Johnson', '555-1234', 1),
('Bob Smith', '555-5678', 2),
('Charlie Lee', '555-8765', 3);

select*from Venues
select*from Bookings
select*from Events