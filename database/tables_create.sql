-- Create the routes table
CREATE TABLE routes (
    route_id SERIAL PRIMARY KEY,
    origin_city_id INT,
    destination_city_id INT,
    departure_date DATE
);

-- Create the flights table
CREATE TABLE flights (
    flight_id SERIAL PRIMARY KEY,
    route_id INT,
    departure_time TIMESTAMP,
    arrival_time TIMESTAMP,
    airline_id INT
);

-- Create the subscriptions table
CREATE TABLE subscriptions (
    origin_city_id INT,
    destination_city_id INT,
    agency_id INT,
    PRIMARY KEY(origin_city_id, destination_city_id, agency_id)
);

-- Define foreign key constraints
ALTER TABLE flights ADD CONSTRAINT fk_routes FOREIGN KEY (route_id) REFERENCES routes(route_id);

CREATE INDEX idx_agency_id ON subscriptions (agency_id);
CREATE INDEX idx_origin_destination_departure ON routes (origin_city_id, destination_city_id, departure_date);