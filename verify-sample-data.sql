-- EventHotelBroker Sample Data Verification Script
-- Run this in MySQL to verify that sample data was seeded correctly

USE eventhotelbroker;

-- Summary counts
SELECT 'SUMMARY' as Section, 
    (SELECT COUNT(*) FROM Categories) as Categories,
    (SELECT COUNT(*) FROM Amenities) as Amenities,
    (SELECT COUNT(*) FROM Hotels) as Hotels,
    (SELECT COUNT(*) FROM Services) as Services,
    (SELECT COUNT(*) FROM Bookings) as Bookings,
    (SELECT COUNT(*) FROM Messages) as Messages,
    (SELECT COUNT(*) FROM AuditLogs) as AuditLogs;

-- Categories
SELECT '--- CATEGORIES ---' as Info;
SELECT Id, Name, Slug, Description FROM Categories ORDER BY Id;

-- Amenities
SELECT '--- AMENITIES ---' as Info;
SELECT Id, Name, Slug, Icon FROM Amenities ORDER BY Id;

-- Hotels by status
SELECT '--- HOTELS BY STATUS ---' as Info;
SELECT 
    IsApproved,
    IsPublished,
    COUNT(*) as Count,
    GROUP_CONCAT(Name SEPARATOR ', ') as Hotels
FROM Hotels 
GROUP BY IsApproved, IsPublished;

-- Hotels summary
SELECT '--- HOTELS SUMMARY ---' as Info;
SELECT 
    Id,
    Name,
    City,
    Capacity,
    PricePerNight,
    IsPublished,
    IsApproved,
    OwnerId
FROM Hotels 
ORDER BY City, Name;

-- Services by status
SELECT '--- SERVICES BY STATUS ---' as Info;
SELECT 
    IsApproved,
    IsPublished,
    COUNT(*) as Count,
    GROUP_CONCAT(Name SEPARATOR ', ') as Services
FROM Services 
GROUP BY IsApproved, IsPublished;

-- Services summary
SELECT '--- SERVICES SUMMARY ---' as Info;
SELECT 
    s.Id,
    s.Name,
    c.Name as Category,
    s.Price,
    s.IsPublished,
    s.IsApproved,
    s.ProviderId
FROM Services s
JOIN Categories c ON s.CategoryId = c.Id
ORDER BY c.Name, s.Name;

-- Bookings by status
SELECT '--- BOOKINGS BY STATUS ---' as Info;
SELECT 
    Status,
    COUNT(*) as Count
FROM Bookings 
GROUP BY Status;

-- Bookings summary
SELECT '--- BOOKINGS SUMMARY ---' as Info;
SELECT 
    b.Id,
    h.Name as Hotel,
    b.UserId,
    b.StartDate,
    b.EndDate,
    b.HeadCount,
    b.Status
FROM Bookings b
JOIN Hotels h ON b.HotelId = h.Id
ORDER BY b.StartDate;

-- Messages summary
SELECT '--- MESSAGES SUMMARY ---' as Info;
SELECT 
    m.Id,
    m.SenderId,
    m.ReceiverId,
    h.Name as Hotel,
    LEFT(m.Content, 50) as ContentPreview,
    m.IsRead,
    m.CreatedAt
FROM Messages m
LEFT JOIN Hotels h ON m.HotelId = h.Id
ORDER BY m.CreatedAt DESC;

-- Hotel amenities count
SELECT '--- HOTEL AMENITIES COUNT ---' as Info;
SELECT 
    h.Name as Hotel,
    COUNT(ha.AmenityId) as AmenityCount,
    GROUP_CONCAT(a.Name SEPARATOR ', ') as Amenities
FROM Hotels h
LEFT JOIN HotelAmenities ha ON h.Id = ha.HotelId
LEFT JOIN Amenities a ON ha.AmenityId = a.Id
GROUP BY h.Id, h.Name
ORDER BY h.Name;

-- Hotel images count
SELECT '--- HOTEL IMAGES COUNT ---' as Info;
SELECT 
    h.Name as Hotel,
    COUNT(hi.Id) as ImageCount
FROM Hotels h
LEFT JOIN HotelImages hi ON h.Id = hi.HotelId
GROUP BY h.Id, h.Name
ORDER BY h.Name;

-- Service images count
SELECT '--- SERVICE IMAGES COUNT ---' as Info;
SELECT 
    s.Name as Service,
    COUNT(si.Id) as ImageCount
FROM Services s
LEFT JOIN ServiceImages si ON s.Id = si.ServiceId
GROUP BY s.Id, s.Name
ORDER BY s.Name;

-- Audit logs summary
SELECT '--- AUDIT LOGS SUMMARY ---' as Info;
SELECT 
    Id,
    UserId,
    Action,
    EntityType,
    EntityId,
    Details,
    CreatedAt
FROM AuditLogs
ORDER BY CreatedAt DESC;

-- Pending approvals
SELECT '--- PENDING APPROVALS ---' as Info;
SELECT 'Hotels' as Type, Name, OwnerId, CreatedAt 
FROM Hotels 
WHERE IsPublished = 1 AND IsApproved = 0
UNION ALL
SELECT 'Services' as Type, Name, ProviderId, CreatedAt 
FROM Services 
WHERE IsPublished = 1 AND IsApproved = 0
ORDER BY CreatedAt DESC;

-- Data integrity checks
SELECT '--- DATA INTEGRITY CHECKS ---' as Info;
SELECT 
    'Hotels without images' as Check,
    COUNT(*) as Count
FROM Hotels h
LEFT JOIN HotelImages hi ON h.Id = hi.HotelId
WHERE hi.Id IS NULL;

SELECT 
    'Hotels without amenities' as Check,
    COUNT(*) as Count
FROM Hotels h
LEFT JOIN HotelAmenities ha ON h.Id = ha.HotelId
WHERE ha.AmenityId IS NULL;

SELECT 
    'Services without images' as Check,
    COUNT(*) as Count
FROM Services s
LEFT JOIN ServiceImages si ON s.Id = si.ServiceId
WHERE si.Id IS NULL;

-- Expected results summary
SELECT '--- EXPECTED VS ACTUAL ---' as Info;
SELECT 
    'Categories' as Entity,
    6 as Expected,
    (SELECT COUNT(*) FROM Categories) as Actual,
    CASE WHEN (SELECT COUNT(*) FROM Categories) = 6 THEN '✓' ELSE '✗' END as Status
UNION ALL
SELECT 
    'Amenities' as Entity,
    12 as Expected,
    (SELECT COUNT(*) FROM Amenities) as Actual,
    CASE WHEN (SELECT COUNT(*) FROM Amenities) = 12 THEN '✓' ELSE '✗' END as Status
UNION ALL
SELECT 
    'Hotels' as Entity,
    13 as Expected,
    (SELECT COUNT(*) FROM Hotels) as Actual,
    CASE WHEN (SELECT COUNT(*) FROM Hotels) >= 13 THEN '✓' ELSE '✗' END as Status
UNION ALL
SELECT 
    'Services' as Entity,
    5 as Expected,
    (SELECT COUNT(*) FROM Services) as Actual,
    CASE WHEN (SELECT COUNT(*) FROM Services) >= 5 THEN '✓' ELSE '✗' END as Status
UNION ALL
SELECT 
    'Bookings' as Entity,
    5 as Expected,
    (SELECT COUNT(*) FROM Bookings) as Actual,
    CASE WHEN (SELECT COUNT(*) FROM Bookings) >= 5 THEN '✓' ELSE '✗' END as Status
UNION ALL
SELECT 
    'Messages' as Entity,
    3 as Expected,
    (SELECT COUNT(*) FROM Messages) as Actual,
    CASE WHEN (SELECT COUNT(*) FROM Messages) >= 3 THEN '✓' ELSE '✗' END as Status;
