-- Script to create a test user with Admin role for authentication testing
-- Note: This uses plain text password which is NOT secure for production!
-- In production, you should implement proper password hashing (BCrypt, PBKDF2, etc.)

-- Create Admin role if it doesn't exist
INSERT OR IGNORE INTO Roles (Name, Description) 
VALUES ('Admin', 'Administrator with full access');

-- Get the Admin role ID
-- Assuming the first role created will have ID 1, adjust if needed

-- Create a test user
-- Password: admin123 (stored as plain text for testing - CHANGE THIS IN PRODUCTION!)
INSERT OR IGNORE INTO Users (Username, Email, PasswordHash, FirstName, LastName, PhoneNumber, IsActive, CreatedAt, UpdatedAt)
VALUES ('admin', 'admin@example.com', 'admin123', 'Admin', 'User', '555-0000', 1, datetime('now'), datetime('now'));

-- Assign Admin role to the test user
INSERT OR IGNORE INTO UserRoles (UserId, RoleId, AssignedAt)
SELECT 
    (SELECT Id FROM Users WHERE Username = 'admin'),
    (SELECT Id FROM Roles WHERE Name = 'Admin'),
    datetime('now')
WHERE NOT EXISTS (
    SELECT 1 FROM UserRoles 
    WHERE UserId = (SELECT Id FROM Users WHERE Username = 'admin')
    AND RoleId = (SELECT Id FROM Roles WHERE Name = 'Admin')
);

-- Verify the user was created
SELECT 
    u.Id, 
    u.Username, 
    u.Email, 
    u.FirstName, 
    u.LastName, 
    u.IsActive,
    r.Name as RoleName
FROM Users u
LEFT JOIN UserRoles ur ON u.Id = ur.UserId
LEFT JOIN Roles r ON ur.RoleId = r.Id
WHERE u.Username = 'admin';
