CREATE TABLE IF NOT EXISTS shop_job_assignees (
    JobTicketNo VARCHAR(50) NOT NULL PRIMARY KEY,
    MechanicUserId INT NOT NULL,
    KEY IX_shop_job_assignees_mechanic (MechanicUserId),
    CONSTRAINT FK_shop_job_assignees_job FOREIGN KEY (JobTicketNo)
        REFERENCES shop_jobs(JobTicketNo) ON DELETE CASCADE,
    CONSTRAINT FK_shop_job_assignees_user FOREIGN KEY (MechanicUserId)
        REFERENCES users(id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
 
-- Associate old jobs only when exactly one active mechanic matches the saved name.
INSERT INTO shop_job_assignees (JobTicketNo, MechanicUserId)
SELECT j.JobTicketNo, MIN(u.id)
FROM shop_jobs j
JOIN users u ON u.name = j.AssignedMechanic
JOIN staff s ON s.user_id = u.id AND s.status = 'active'
JOIN roles r ON r.id = s.role_id AND r.name = 'Mechanic'
LEFT JOIN shop_job_assignees a ON a.JobTicketNo = j.JobTicketNo
WHERE a.JobTicketNo IS NULL
GROUP BY j.JobTicketNo
HAVING COUNT(*) = 1
ON DUPLICATE KEY UPDATE MechanicUserId = shop_job_assignees.MechanicUserId;
