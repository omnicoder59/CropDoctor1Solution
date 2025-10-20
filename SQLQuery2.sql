SELECT 
    ua.Id,
    anu.Email,  
    ua.ActivityType,
    ua.Details,
    ua.Timestamp
FROM 
    UserActivities AS ua
JOIN 
    AspNetUsers AS anu ON ua.UserId = anu.Id
ORDER BY 
    ua.Timestamp DESC;