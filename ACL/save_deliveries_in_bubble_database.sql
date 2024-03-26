WITH updated AS (
    UPDATE deliveries
    SET destination_street = @DestinationStreet,
        destination_city = @DestinationCity,
        destination_state = @DestinationState,
        destination_zip_code = @DestinationZipCode
    WHERE id = @Id
        RETURNING id
)
INSERT INTO deliveries (id, destination_street, destination_city, destination_state, destination_zip_code)
SELECT @Id, @DestinationStreet, @DestinationCity, @DestinationState, @DestinationZipCode
WHERE NOT EXISTS (SELECT 1 FROM updated WHERE id = @Id);
