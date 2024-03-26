WITH updated AS (
    UPDATE products
    SET weight_in_pounds = @WeightInPounds,
        name = @Name
    WHERE id = @ProductId
    RETURNING *
)
INSERT INTO products (id, weight_in_pounds, name)
SELECT @ProductId, @WeightInPounds, @Name
WHERE NOT EXISTS (SELECT 1 FROM updated);